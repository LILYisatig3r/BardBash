﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerInput : MonoBehaviour {

    public delegate void ControllerAction(KeyCode action);
    public static event ControllerAction Pressed;

    void Update()
    {
        if (Input.GetButtonDown("A_Button"))
        {
            Pressed(KeyCode.K);
        }
        if (Input.GetButtonDown("B_Button"))
        {
            Pressed(KeyCode.L);
        }
        if (Input.GetButtonDown("X_Button"))
        {
            Pressed(KeyCode.J);
        }
    }
}
