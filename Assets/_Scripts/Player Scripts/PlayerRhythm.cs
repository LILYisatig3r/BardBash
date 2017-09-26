using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class PlayerRhythm : MonoBehaviour {

    public string trackName;

    PlayerController pc;
    float frameDelay = 0;
    float preFrameDelay = 0;
    Dictionary<KeyCode, Action> actions;
    KeyCode preppedAction;

    string casting = "";
    Dictionary<string, Action> spells;

	void Start () {
        pc = GetComponent<PlayerController>();

        Koreographer.Instance.RegisterForEvents(trackName, OnMusicalShoot);
        PlayerController.Pressed += TryAction;

        actions = new Dictionary<KeyCode, Action>();
        actions.Add(KeyCode.K, pc.Shoot);
        actions.Add(KeyCode.L, pc.Dash);
        actions.Add(KeyCode.J, pc.Melee);
        actions.Add(KeyCode.I, pc.Cast);

        spells = new Dictionary<string, Action>();
        spells.Add("JLJ", pc.SpellBlast);
	}

    void OnDisable()
    {
        PlayerController.Pressed -= TryAction;
    }

    private void TryAction(KeyCode action)
    {
        if (frameDelay > 0)
        {
            if (!pc.casting)
            {
                Action a;
                if (actions.TryGetValue(action, out a))
                    a.Invoke();
            }
            else
            {
                if (action.Equals(KeyCode.I))
                {
                    pc.casting = false;
                    casting = "";
                }
                else
                {
                    print(action.ToString());
                    casting += action.ToString();
                    Action a;
                    if (casting.Length >= 3)
                    {
                        if (spells.TryGetValue(casting, out a))
                            a.Invoke();
                        casting = "";
                        pc.casting = false;
                    }
                }
            }
            frameDelay = 0;
            preFrameDelay = 0;
        }

        else if (preFrameDelay == 0)
        {
            preFrameDelay = 6;
            if (action.Equals(KeyCode.I))
            {
                //print("elif");
                pc.casting = false;
                casting = "";
            }
            else
                preppedAction = action;
        }

        else
        {
            print("el");
            pc.casting = false;
            casting = "";
        }
    }

    void FixedUpdate()
    {
        if (preFrameDelay > 0 && frameDelay > 0)
        {
            if (!pc.casting)
            {
                Action a;
                if (actions.TryGetValue(preppedAction, out a))
                    a.Invoke();
            }
            else
            {
                if (preppedAction.Equals(KeyCode.I))
                {
                    pc.casting = false;
                    casting = "";
                }
                else
                {
                    casting += preppedAction.ToString();
                    print(casting);
                    Action a;
                    if (casting.Length >= 3)
                    {
                        if (spells.TryGetValue(casting, out a))
                            a.Invoke();
                        casting = "";
                        pc.casting = false;
                    }
                }
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
