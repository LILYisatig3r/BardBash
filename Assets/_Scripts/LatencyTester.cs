using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class LatencyTester : MonoBehaviour {

    private List<float> delays;
    private float delay;
	
    void Start()
    {
        delays = new List<float>(100);
        Koreographer.Instance.RegisterForEvents("shoot", OnMusicalShoot);
    }

    void OnDisable()
    {

    }

	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown("X_Button" ) || Input.GetKeyDown(KeyCode.P))
        {
            delays.Add(delay);
            delay = 0;
        }
        delay += Time.deltaTime;
    }

    void OnMusicalShoot(KoreographyEvent e)
    {
        delay = 0;
        float average_delay = 0;
        foreach (float f in delays)
            average_delay += f;
        print(average_delay / delays.Count);
    }
}
