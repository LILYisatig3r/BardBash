using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlayerRhythm : MonoBehaviour {

    PlayerController pc;
    private float frameDelay = 0;
    private float preFrameDelay = 0;
    private Dictionary<KeyCode, Action> actions;
    private KeyCode preppedAction;

	void Start () {
        pc = GetComponent<PlayerController>();

        Koreographer.Instance.RegisterForEvents("shoot", OnMusicalShoot);
        PlayerController.Used += TryAction;
        ControllerInput.Pressed += TryAction;

        actions = new Dictionary<KeyCode, Action>();
        actions.Add(KeyCode.Space, pc.Shoot);
        actions.Add(KeyCode.J, pc.Dash);
        actions.Add(KeyCode.K, pc.Melee);
	}

    void OnDisable()
    {
        PlayerController.Used -= TryAction;
    }

    private void TryAction(KeyCode action)
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

    void FixedUpdate()
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
            preppedAction = KeyCode.At;
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
