using System.Collections;
using System.Collections.Generic;
using UnityEngine;


class ButtonTouchStateA
{
    //public bool aStates[] = new bool[];
    //public bool bStates[] = new bool[];
    //public bool eStates[] = new bool[];
    public bool cState = false;

    public float ringDistCalculation(int handPos, int pos) {
        float handDist = 8 - Mathf.Max(pos - handPos, handPos - pos); // 8 - obtuse angle
        return handDist;
    }
    public void matchingHand()
    {
        float rightHandPos = 2.5f;
        float leftHandPos = 7.5f;
    }
}

public class PhysicalRigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNote(int position, int a)
    {

    }
}
