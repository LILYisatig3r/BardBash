using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class Rhythm : MonoBehaviour {

    PlayerController pc;
    private float frameDelay = 0;

	void Start () {
        pc = GetComponent<PlayerController>();

        Koreographer.Instance.RegisterForEvents("shoot", OnMusicalShoot);
	}

    void Update()
    {
        if (frameDelay > 0)
        {
            frameDelay -= 1;
        }
        //print(frameDelay);

        //if (Input.GetKeyDown("space") && Time.timeScale != 0f && frameDelay > 0)
        //{
        //    pc.Shoot();
        //}
    }

    void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode == KeyCode.Space && frameDelay > 0)
        {
            pc.Shoot();
            frameDelay = 0;
        }
    }
	
	void OnMusicalShoot(KoreographyEvent e)
    {
        frameDelay = 10;
    }
}
