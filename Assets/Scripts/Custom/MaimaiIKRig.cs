using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


class ButtonTouchState
{
    public bool[] aStates = new bool[8];
    public bool[] bStates = new bool[8];
    public bool[] eStates = new bool[8];
    public bool cState = false;

    
    public void matchingHand() // Return array of [left, right]
    {
        
    }
}

/*
Queue System
1. Add hitobjects within a certain window
2. Loop through by order
3. lerp depending on ratio

*/


[RequireComponent(typeof(Animator))]

public class MaimaiIKRig : MonoBehaviour
{

    protected Animator animator;
    public bool ikActive = true;
    public Transform leftHandObj = null;
    public Transform rightHandObj = null;

    public GameObject[] physicalButtons = new GameObject[8];


    AudioTimeProvider timeProvider;
    public float resetTime = 0.01f;
    public float lerpRatio = 1f;
    Dictionary<float, ButtonTouchState> timeButtonMatching = new Dictionary<float, ButtonTouchState>();
    // Time Matching
    public int totalTimeIndex = 0;
    Dictionary<float, int> timeIndexMatching = new Dictionary<float, int>();
    Dictionary<int, float> indexTimeMatching = new Dictionary<int, float>();
    Dictionary<int, ButtonTouchState> indexButtonMatching = new Dictionary<int, ButtonTouchState>();
    Dictionary<int, List<GameObject>> indexSlideMatching = new Dictionary<int, List<GameObject>>();


    public List<float> timings = new List<float>(); // time that the button is hit
    public List<int> buttons = new List<int>();

    
    // Start is called before the first frame update
    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        animator = GetComponent<Animator>();
    }

    public float ringDistCalculation(float handPos, float pos)
    {
        float handDist = Mathf.Max(pos - handPos, handPos - pos); // 8 - obtuse angle
        return handDist;
    }

    List<int> CalculateHandPosition(ButtonTouchState state)
    {
        List<int> buttonsToPress = new List<int>();

        float rightHandPos = 3f; // Lower
        float leftHandPos = 6f;
        List<int> buttonsPressed = new List<int>();
        float[] leftDistMatching = new float[8];
        float[] rightDistMatching = new float[8];

        // Calculate distance
        for (int i = 0; i < 8; i++)
        {
            if (state.aStates[i])
            {
                leftDistMatching[i] = ringDistCalculation(leftHandPos, i + 1);
                rightDistMatching[i] = ringDistCalculation(rightHandPos, i + 1);
                buttonsPressed.Add(i);
            }
        }

        // Decide which hand goes where
        if (buttonsPressed.Count == 1)
        {
            int button1 = buttonsPressed[0];
            float leftDist = leftDistMatching[buttonsPressed[0]];
            float rightDist = rightDistMatching[buttonsPressed[0]];
            if (leftDist < rightDist)
            {
                buttonsToPress.Add(button1);
                buttonsToPress.Add(-1);
                //leftHandObj = physicalButtons[button1].transform;
                //rightHandObj = null;
            }
            else if (rightDist <= leftDist)
            {
                buttonsToPress.Add(-1);
                buttonsToPress.Add(button1);
                //leftHandObj = null;
                //rightHandObj = physicalButtons[button1].transform;
            }
        }
        else if (buttonsPressed.Count == 2)
        {
            int button1 = buttonsPressed[0];
            int button2 = buttonsPressed[1];
            if (leftDistMatching[button1] < leftDistMatching[button2])
            {

                buttonsToPress.Add(button1);
                buttonsToPress.Add(button2);
                //leftHandObj = physicalButtons[button1].transform;
                //rightHandObj = physicalButtons[button2].transform;
            }
            else
            {
                buttonsToPress.Add(button2);
                buttonsToPress.Add(button1);
                //leftHandObj = physicalButtons[button2].transform;
                //rightHandObj = physicalButtons[button1].transform;
            }
        }else {
            buttonsToPress.Add(-1);
            buttonsToPress.Add(-1);
        }
        return buttonsToPress;
    }

    // Update is called once per frame
    void Update()
    {
        bool currPressed = false;
        /*
        for (int i = 0; i < timings.Count; i++)
        {
            var timing = timeProvider.AudioTime - timings[i];
            if (0 < timing && timing < resetTime)
            {
                //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                rightHandObj = physicalButtons[buttons[i]-1].transform;
                currPressed = true;
            }
        }
        */

        foreach (KeyValuePair<float, ButtonTouchState> entry in timeButtonMatching)
        {
            // do something with entry.Value or entry.Key
            var timing = timeProvider.AudioTime - entry.Key;
            if (0 < timing)//-2 < timing && timing < 0) // && timing < resetTime) // If pressed
            {
                lerpRatio = 1f;

                // Decide which hand goes where
                List<int> handPos = CalculateHandPosition(entry.Value);
                Vector3 leftPositionNew = handPos[0] != -1 ? physicalButtons[handPos[0]].transform.position : physicalButtons[3].transform.position;
                Vector3 rightPositionNew = handPos[1] != -1 ? physicalButtons[handPos[1]].transform.position : physicalButtons[6].transform.position;

                //leftHandObj = handPos[0] != -1 ? physicalButtons[handPos[0]].transform : physicalButtons[3].transform;
                //rightHandObj = handPos[1] != -1 ? physicalButtons[handPos[1]].transform: physicalButtons[6].transform;

                // Get States
                int currentTimeIndex = timeIndexMatching[entry.Key];
                int prevTimeIndex = Math.Max(0, currentTimeIndex - 1);
                int nextTimeIndex = Math.Min(currentTimeIndex, totalTimeIndex);
                // Debug.Log(prevTimeIndex);
                // Debug.Log(nextTimeIndex);
                ButtonTouchState prevEntry = indexButtonMatching[prevTimeIndex];
                ButtonTouchState nextEntry = indexButtonMatching[nextTimeIndex];


                // Get old Hand Position
                float prevTime = indexTimeMatching[prevTimeIndex];
                List<int> prevHandPos = CalculateHandPosition(prevEntry);
                Vector3 leftPositionOld = prevHandPos[0] != -1 ? physicalButtons[prevHandPos[0]].transform.position : physicalButtons[3].transform.position;
                Vector3 rightPositionOld = prevHandPos[1] != -1 ? physicalButtons[prevHandPos[1]].transform.position : physicalButtons[6].transform.position;

                // Get new Hand Position
                float nextTime = indexTimeMatching[nextTimeIndex];
                List<int> nextHandPos = CalculateHandPosition(nextEntry);
                Vector3 leftPositionNewer = nextHandPos[0] != -1 ? physicalButtons[nextHandPos[0]].transform.position : physicalButtons[3].transform.position;
                Vector3 rightPositionNewer = nextHandPos[1] != -1 ? physicalButtons[nextHandPos[1]].transform.position : physicalButtons[6].transform.position;

                if (indexSlideMatching.ContainsKey(nextTimeIndex) && indexSlideMatching[nextTimeIndex] != null) {
                    // Debug.LogFormat("Slide");
                    if ( indexSlideMatching[nextTimeIndex][0]){
                        leftPositionNew = indexSlideMatching[nextTimeIndex][0].transform.position;
                    }
                    if (indexSlideMatching[nextTimeIndex].Count > 1 && indexSlideMatching[nextTimeIndex][1]) {
                        rightPositionNew = indexSlideMatching[nextTimeIndex][1].transform.position;
                    }
                }

                // Calculate Position
                float lerp = timing / (nextTime - entry.Key);
                // float lerp = 1; //timing / (prevTime - entry.Key);
                // if (!(0<lerp && lerp < 1)) { continue;  }
                // Debug.Log(lerp);
                Vector3 leftPosition = Vector3.Lerp(leftPositionOld, leftPositionNew, lerp);
                Vector3 rightPosition = Vector3.Lerp(rightPositionOld, rightPositionNew, lerp);
                leftHandObj.position = leftPosition;
                rightHandObj.position = rightPosition;
            } else {
                //Debug.LogFormat("Exited");
                break; // Exit early, prevent terminating
            }

        }
    }


    //a callback for calculating IK
    void OnAnimatorIK()
    {
        if (animator)
        {
            //if the IK is active, set the position and rotation directly to the goal.
            if (ikActive)
            {
                // Set the right hand target position and rotation, if one has been assigned
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, lerpRatio); //1);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, lerpRatio); // 1);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandObj.rotation);
                }

                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, lerpRatio); // 1);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, lerpRatio); // 1);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.position);
                    animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.rotation);
                }
            }
            //if the IK is not active, set the position and rotation of the hand and head back to the original position
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
                //animator.SetLookAtWeight(0);

                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
                animator.SetLookAtWeight(0);
            }
        }
    }


    public void press(float givenTime, int pos)
    {
        timings.Add(givenTime);
        buttons.Add(pos);
        if (!timeButtonMatching.ContainsKey(givenTime))
        {
            timeButtonMatching[givenTime] = new ButtonTouchState();
        }

        timeButtonMatching[givenTime].aStates[pos-1] = true;

        timeIndexMatching[givenTime] = totalTimeIndex;
        indexTimeMatching[totalTimeIndex] = givenTime;
        indexButtonMatching[totalTimeIndex] = timeButtonMatching[givenTime];
        totalTimeIndex += 1;
    }

    public void slide(float givenTime, int pos, GameObject slideobj)
    {
        timings.Add(givenTime);
        buttons.Add(pos);
        if (!timeButtonMatching.ContainsKey(givenTime))
        {
            timeButtonMatching[givenTime] = new ButtonTouchState();
        }

        timeButtonMatching[givenTime].aStates[pos-1] = true;

        timeIndexMatching[givenTime] = totalTimeIndex;
        indexTimeMatching[totalTimeIndex] = givenTime;
        indexButtonMatching[totalTimeIndex] = timeButtonMatching[givenTime];
        if (!indexSlideMatching.ContainsKey(totalTimeIndex)){
            indexSlideMatching[totalTimeIndex] = new List<GameObject>();
        }
        indexSlideMatching[totalTimeIndex].Add(slideobj);
        totalTimeIndex += 1;
    }
}

