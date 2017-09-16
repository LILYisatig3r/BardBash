using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class Rhythm : MonoBehaviour {

    PlayerController pc;
    private float Timer = 0;

	// Use this for initialization
	void Start () {
        pc = GetComponent<PlayerController>();

        Koreographer.Instance.RegisterForEvents("shoot", OnMusicalShoot);
	}
	
	void OnMusicalShoot(KoreographyEvent e)
    {
        //if (Input.GetKeyDown("space") && Time.timeScale != 0f)
        {
            pc.Shoot();
        }
    }
}
