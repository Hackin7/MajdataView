using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using System;
using UnityEngine.UI;

public class NotesParser : MonoBehaviour
{
    
    public GameObject[] physicalButtons = new GameObject[8];
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void receiveData(string json, float ignoreOffset) {
        Debug.Log(json);

        var loadedData = JsonConvert.DeserializeObject<Majson>(json);
        
        double lastNoteTime = loadedData.timingList.Last().time;

        /*
        enum SimaiNoteType
        {
            Tap, Slide, Hold, Touch, TouchHold
            0,   1,     2,    3,     4
        }
        */
        foreach (var timing in loadedData.timingList) {
            try {
                if (timing.time < ignoreOffset) {
                    continue;
                }
                for (int i = 0; i < timing.noteList.Count; i++)
                {
                    var note = timing.noteList[i];
                    if (note.noteType == SimaiNoteType.Tap)
                    {
                        GameObject.Find("Bot").GetComponent<MaimaiIKRig>().press((float)timing.time, note.startPosition);
                        //physicalButtons[NDCompo.startPosition - 1].GetComponent<PhysicalButton>().press((float)timing.time);
                    }
                    if (note.noteType == SimaiNoteType.Slide)
                    {
                        GameObject.Find("Bot").GetComponent<MaimaiIKRig>().press((float)timing.time, note.startPosition);
                    }
                    if (note.noteType == SimaiNoteType.Hold)
                    {
                        GameObject.Find("Bot").GetComponent<MaimaiIKRig>().press((float)timing.time, note.startPosition);
                        //physicalButtons[NDCompo.startPosition - 1].GetComponent<PhysicalButton>().press((float)timing.time);
                    }
                    if (note.noteType == SimaiNoteType.TouchHold)
                    {
                    }
                    if (note.noteType == SimaiNoteType.Touch)
                    {
                    }
                }

            }catch(Exception e)
            {
                GameObject.Find("ErrText").GetComponent<Text>().text = "在第"+(timing.rawTextPositionY+1 )+"行发现问题：\n"+e.Message;
            }
        }
    }
}
