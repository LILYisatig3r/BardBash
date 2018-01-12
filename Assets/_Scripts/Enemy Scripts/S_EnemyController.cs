using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class S_EnemyController : S_Actor {

    //public Vector2 directionMemory;

    //[SerializeField] GameManager gm;
    [SerializeField] int beatDelay;
    [SerializeField] string trackName;
    //[SerializeField] Vector3 position;

    Dictionary<KeyCode, S_Action> actions;

    void Start () {

        if (gm != null || GameManager.TryGetInstance(out gm))
            gm.AddActor(gameObject);
        actions = new Dictionary<KeyCode, S_Action>();
        ac = GetComponentInChildren<S_AnimationController>();
        ac.SetActor(actorName);

        S_Action move;
        S_Actions.TryGetAction("actorMove", out move);
        actions.Add(KeyCode.UpArrow, move);
        actions.Add(KeyCode.RightArrow, move);
        actions.Add(KeyCode.DownArrow, move);
        actions.Add(KeyCode.LeftArrow, move);

        measures = 2f;

        directionMemory = new Vector3(1, 0, 0);
        position = gm.ActorRandomSpawn(gameObject);
        transform.position = gm.GetMap().GridToWorldPosition(position);
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z + 0.5f);

        gm.ActorEnterBattle(gameObject);
    }

    public override void ReceiveBeat()
    {
        KeyCode[] randomActions = { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.RightArrow, KeyCode.LeftArrow };
        preppedInput = randomActions[UnityEngine.Random.Range(0, 4)];
        if (actions.TryGetValue(preppedInput, out action))
            action.Invoker(gameObject, gm);
        gm.ActorFinishAction(gameObject);
    }

    override public float GetActionCost()
    {
        return action != null ? action.cost : 0.25f;
    }

}
