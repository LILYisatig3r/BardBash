using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Action {

    public float cost;

    public delegate void ThisAction(params object[] args);
    private ThisAction note;

    public S_Action(float c, ThisAction a)
    {
        cost = c;
        note = a;
    }

    public void Invoker(params object[] args)
    {
        note(args);
    }
}
