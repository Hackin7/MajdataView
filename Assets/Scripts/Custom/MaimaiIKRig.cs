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

    public float ringDistCalculation(float handPos, float pos)
    {
        float handDist =  Mathf.Max(pos - handPos, handPos - pos); // 8 - obtuse angle
        return handDist;
    }
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
    private float resetTime = 0.01f;
    private float lerpRatio = 1f;
    Dictionary<float, ButtonTouchState> timeButtonMatching = new Dictionary<float, ButtonTouchState>();
    
    public List<float> timings = new List<float>(); // time that the button is hit
    public List<int> buttons = new List<int>();
    
    // Start is called before the first frame update
    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        animator = GetComponent<Animator>();
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
            if (0 < timing && timing < resetTime)
            {
                lerpRatio = 1f;// - Time.deltaTime/resetTime;
                //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                //rightHandObj = physicalButtons[buttons[i] - 1].transform;

                float rightHandPos = 3f; // Lower
                float leftHandPos = 6f;
                List<int> buttonsPressed = new List<int>();
                float[] leftDistMatching = new float[8];
                float[] rightDistMatching = new float[8];

                for (int i = 0; i < 8; i++)
                {
                    if (entry.Value.aStates[i])
                    {
                        leftDistMatching[i] = entry.Value.ringDistCalculation(leftHandPos, i + 1);
                        rightDistMatching[i] = entry.Value.ringDistCalculation(rightHandPos, i + 1);
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
                        leftHandObj = physicalButtons[button1].transform;
                        rightHandObj = null;
                    }
                    else if (rightDist <= leftDist)
                    {
                        leftHandObj = null;
                        rightHandObj = physicalButtons[button1].transform;
                    }
                }else if (buttonsPressed.Count == 2)
                {
                    int button1 = buttonsPressed[0];
                    int button2 = buttonsPressed[1];
                    if (leftDistMatching[button1] < leftDistMatching[button2])
                    {
                        leftHandObj = physicalButtons[button1].transform;
                        rightHandObj = physicalButtons[button2].transform;
                    }
                    else
                    {
                        leftHandObj = physicalButtons[button2].transform;
                        rightHandObj = physicalButtons[button1].transform;
                    }
                }


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
    }
}
