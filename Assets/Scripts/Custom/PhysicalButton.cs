using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalButton : MonoBehaviour
{
    public List<float> timings = new List<float>(); // time that the button is hit
    AudioTimeProvider timeProvider;
    public bool enabled = false;
    private float resetTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        timeProvider = GameObject.Find("AudioTimeProvider").GetComponent<AudioTimeProvider>();
        transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // Disable for now
    }

    // Update is called once per frame
    void Update()
    {
        
        if (!enabled) { return; }

        bool currPressed = false;
        for (int i=0; i<timings.Count; i++)
        {
            var timing = timeProvider.AudioTime - timings[i];
            if (0 < timing && timing < resetTime)
            {
                //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                currPressed = true;
            }
        }
        
        if (!currPressed)
        {
            //transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }


    public void press(float givenTime)
    {
        enabled = true;
        timings.Add(givenTime);
    }
}
