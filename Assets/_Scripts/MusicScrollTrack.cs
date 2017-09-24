using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;


public class MusicScrollTrack : MonoBehaviour
{

    public Koreography KY;
    public List<KoreographyEvent> KEvents;

    public int lineOneX;
    public int whichBeat;
    public GameObject parent;
    public string trackName;

    // Use this for initialization
    void Start()
    {
        KoreographyTrackBase KYT = KY.GetTrackByID(trackName);
        KEvents.AddRange(KYT.GetAllEvents());
        parent = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        float verticalOffset = Screen.height / 100;

        //If whichBeat is set to -1 then it's the still beat in the center of the screen that others hit.
        if (whichBeat == -1)
        {
            Vector3 firstSpritePos = this.transform.parent.position;
            firstSpritePos.z = -1;
            firstSpritePos.y = firstSpritePos.y - 3;
            this.transform.position = firstSpritePos;
            return;
        }
        int closestBeat = KEvents[0].StartSample;
        lineOneX = KEvents[whichBeat].StartSample;

        int offSet = Koreographer.GetSampleTime();

        closestBeat -= offSet;
        lineOneX -= offSet;

        float finalLineOneX = lineOneX/5000f;

        Vector3 spritePos = parent.transform.position;



        //THIS STILL NEEDS SPITE OFFSET
        spritePos.x = spritePos.x + finalLineOneX;
        spritePos.y = spritePos.y - 3;
        spritePos.z = -1;

        this.transform.position = spritePos;

        if (closestBeat < 0)
        {
            KEvents.RemoveAt(0);
        }

    }
}
