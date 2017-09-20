using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlayerRhythm : MonoBehaviour {

    PlayerController pc;
    private float frameDelay = 0;
    private float preFrameDelay = 0;
    private Dictionary<string, Action> actions;
    private string preppedAction = "";

	void Start () {
        pc = GetComponent<PlayerController>();

        Koreographer.Instance.RegisterForEvents("shoot", OnMusicalShoot);
        PlayerController.Used += TryAction;

        actions = new Dictionary<string, Action>();
        actions.Add("attack", pc.Shoot);
        actions.Add("dash", pc.Dash);
	}

    void OnDisable()
    {
        PlayerController.Used -= TryAction;
    }

    private void TryAction(string action)
    {
        if (frameDelay > 0)
        {
            Action a;
            if (actions.TryGetValue(action, out a))
            {
                a.Invoke();
            }
            frameDelay = 0;
            preFrameDelay = 0;
        }

        else if (preFrameDelay == 0)
        {
            preFrameDelay = 6;
            preppedAction = action;
        }
    }

    void Update()
    {
        if (preFrameDelay > 0 && frameDelay > 0)
        {
            Action a;
            if (actions.TryGetValue(preppedAction, out a))
            {
                a.Invoke();
            }
            frameDelay = 0;
            preFrameDelay = 0;
            preppedAction = "";
        }

        if (frameDelay > 0)
            frameDelay -= 1;
        if (preFrameDelay > 0)
            preFrameDelay -= 1;
    }
	
	void OnMusicalShoot(KoreographyEvent e)
    {
        frameDelay = 6;
    }
}
