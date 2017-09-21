using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;


public class MusicScrollTrack : MonoBehaviour
{

    public Koreography KY;
    public Koreographer KR;
    public List<KoreographyEvent> KEvents;

    public LineRenderer line, line2, line3;
    public int lineOneX, lineTwoX, lineThreeX;
    public int past;

    // Use this for initialization
    void Start()
    {
        KoreographyTrackBase KYT = KY.GetTrackByID("shoot");
        KEvents.AddRange(KYT.GetAllEvents());
        line = gameObject.AddComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineOneX = KEvents[0].StartSample;
        lineTwoX = KEvents[1].StartSample;
        lineThreeX = KEvents[2].StartSample;

        int offSet = Koreographer.GetSampleTime();
        lineOneX -= offSet;
        lineTwoX -= offSet;
        lineThreeX -= offSet;

        //print("One: " + (lineOneX));
        //print("Two: " + (lineTwoX));
        //print("Three: " + (lineThreeX));

        float finalLineOneX = lineOneX/5000f;
        //Debug.Log(finalLineOneX);

        line.positionCount = 2;
        line.SetPosition(0, new Vector3((finalLineOneX), 1, -1));
        line.SetPosition(1, new Vector3((finalLineOneX), 0, -1));
        line.SetWidth(.5f, .5f);
        line.useWorldSpace = true;

        /*
        line.positionCount = 2;
        line.SetPosition(0, new Vector3((lineTwoX) / 1000, 1, -1));
        line.SetPosition(1, new Vector3((lineTwoX) / 1000, 0, -1));
        line.SetWidth(.5f, .5f);
        line.useWorldSpace = true;

        line.positionCount = 2;
        line.SetPosition(0, new Vector3((lineThreeX) / 1000, 1, -1));
        line.SetPosition(1, new Vector3((lineThreeX) / 1000, 0, -1));
        line.SetWidth(.5f, .5f);
        line.useWorldSpace = true;
        */

        if (finalLineOneX < 0)
        {
            //Debug.LogWarning("Removed");
            KEvents.RemoveAt(0);
            //print(KEvents.Count);
        }

    }
}
