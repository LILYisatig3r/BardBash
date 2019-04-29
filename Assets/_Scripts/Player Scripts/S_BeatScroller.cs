using System.Collections.Generic;
using UnityEngine;

public class S_BeatScroller : MonoBehaviour {

    private float[] beatEvents;

    private LinkedList<Transform> beatScrollers;

	void Start ()
	{
	    beatScrollers = new LinkedList<Transform>();
	    beatScrollers.AddLast(transform.GetChild(0));
	    beatScrollers.AddLast(transform.GetChild(1));
	    beatScrollers.AddLast(transform.GetChild(2));
	    beatScrollers.AddLast(transform.GetChild(3));
	    beatScrollers.AddLast(transform.GetChild(4));
	    beatScrollers.AddLast(transform.GetChild(5));
    }

    void Update () {
		
	}
}
