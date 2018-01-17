using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class S_PlayerController : S_Actor {

    #region Members
    private Rigidbody rb;
    //private S_AnimationController ac;

    //public Vector3 directionMemory;
    int latencyDelay = 0;
    float compensatePostBeat = 0;
    float compensatePreBeat = 0;
    //public KeyCode preppedInput;
    //S_Action action;
    Dictionary<KeyCode, S_Action> actions;
    public bool moving = false;

    //[SerializeField] GameManager gm;
    [SerializeField] string trackName;
    //[SerializeField] public Vector3 position;
    [SerializeField] string heroName;
    [SerializeField] public Transform projectilePrefab;
    #endregion

    #region Monobehaviour
    void Start () {
        ac = transform.GetChild(0).GetComponent<S_AnimationController>();
        ac.SetActor(actorName);
        rb = GetComponent<Rigidbody>();
        actions = new Dictionary<KeyCode, S_Action>();

        S_Action rest;
        S_Actions.TryGetAction("rest", out rest);
        actions.Add(KeyCode.None, rest);

        S_Action move;
        S_Actions.TryGetAction("actorMove", out move);
        actions.Add(KeyCode.UpArrow, move);
        actions.Add(KeyCode.RightArrow, move);
        actions.Add(KeyCode.DownArrow, move);
        actions.Add(KeyCode.LeftArrow, move);

        S_Action ability1;
        S_Actions.TryGetAction(a1, out ability1);
        actions.Add(KeyCode.Q, ability1);

        S_Action ability2;
        S_Actions.TryGetAction(a2, out ability2);
        actions.Add(KeyCode.W, ability2);

        S_Action ability3;
        S_Actions.TryGetAction(a2, out ability3);
        actions.Add(KeyCode.E, ability3);

        S_Action ability4;
        S_Actions.TryGetAction(a2, out ability4);
        actions.Add(KeyCode.R, ability4);

        preppedInput = KeyCode.None;

        if (gm != null || GameManager.TryGetInstance(out gm))
            gm.AddActor(gameObject);

        directionMemory = new Vector2(1, 0);
        position = gm.ActorRandomSpawn(gameObject);
        transform.position = gm.GetMap().GridToWorldPosition(position);
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z + 0.5f);

        gm.ActorEnterBattle(gameObject);
    }

    void Update () {
        latencyDelay--;

        if (Input.GetKeyDown(KeyCode.UpArrow) && latencyDelay < 0)
            preppedInput = KeyCode.UpArrow;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && latencyDelay < 0)
            preppedInput = KeyCode.RightArrow;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && latencyDelay < 0)
            preppedInput = KeyCode.DownArrow;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && latencyDelay < 0)
            preppedInput = KeyCode.LeftArrow;
        else if (Input.GetKeyDown(KeyCode.Q) && latencyDelay < 0)
            preppedInput = KeyCode.Q;
        else if (Input.GetKeyDown(KeyCode.W) && latencyDelay < 0)
            preppedInput = KeyCode.W;
        else if (Input.GetKeyDown(KeyCode.Q) && latencyDelay < 0)
            preppedInput = KeyCode.E;
        else if (Input.GetKeyDown(KeyCode.W) && latencyDelay < 0)
            preppedInput = KeyCode.R;
        if (Input.anyKeyDown)
        {
            ac.ReceiveAction(preppedInput);
            compensatePreBeat = 10;
        }

        if (compensatePostBeat > 0 && compensatePreBeat > 0)
        {
            if (actions.TryGetValue(preppedInput, out action))
                action.Invoker(gameObject, gm);
            gm.ActorFinishAction(gameObject);
            compensatePostBeat = 0;
            compensatePreBeat = 0;
        }
        else if (compensatePostBeat == 1 && preppedInput == KeyCode.None)
        {
            if (actions.TryGetValue(preppedInput, out action))
                action.Invoker(gameObject, gm);
            gm.ActorFinishAction(gameObject);
        }

        compensatePostBeat = compensatePostBeat > 0 ? compensatePostBeat - 1 : compensatePostBeat;
        compensatePreBeat = compensatePreBeat > 0 ? compensatePreBeat - 1 : compensatePreBeat;
    }
    #endregion

    #region External functions

    override public void ReceiveBeat()
    {
        compensatePostBeat = 10;
        ac.SyncUnlock();
    }

    override public void AnimationUpdate()
    {
        ac.ReceiveAction(preppedInput);
        ac.SyncUnlock();
    }

    override public float GetActionCost()
    {
        return action != null ? action.cost : 0.25f;
    }
    #endregion

    #region Actions (legacy)
    public void Move(params object[] args)
    {
        Vector3 newPos = new Vector3();
        Vector3 newDirection = directionMemory;
        switch (preppedInput)
        {
            case KeyCode.UpArrow:
                newPos = new Vector3(position.x, position.y, position.z + 1);
                newDirection = new Vector3(0, 0, 1);
                break;
            case KeyCode.RightArrow:
                newPos = new Vector3(position.x + 1, position.y, position.z);
                newDirection = new Vector3(1, 0, 0);
                break;
            case KeyCode.DownArrow:
                newPos = new Vector3(position.x, position.y, position.z - 1);
                newDirection = new Vector3(0, 0, -1);
                break;
            case KeyCode.LeftArrow:
                newPos = new Vector3(position.x - 1, position.y, position.z);
                newDirection = new Vector3(-1, 0, 0);
                break;
        }
        Vector3 dest = gm.ActorMove(gameObject, newPos);
        if (dest.y != -1)
        {
            position = dest;
            moving = true;
            //transform.position = gm.GetMap().GridToWorldPosition(dest);
            //transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z);
        }
        float scaleX = transform.localScale.x;
        if (newDirection.x != 0 && newDirection.x != directionMemory.x)
            scaleX = newDirection.x;
        transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z + 0.5f);
        directionMemory = newDirection;

        speed = 1;
        return;
    }

    public void Shoot(params object[] args)
    {
        //BeatUpdate();
        Transform p = Instantiate(projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        //Vector2 input = rb.velocity.magnitude < 0.1 ? directionMemory : rb.velocity;
        projectile.Spawn(transform.position, directionMemory, magic);
    }

    public void Dash(params object[] args)
    {
        //BeatUpdate();
        speed *= 2;
    }

    private void BeatUpdate()
    {
        if (transform.localScale.x == 1.0f)
        {
            transform.localScale = new Vector3(1.1f, 1.1f);
        }
        else if (transform.localScale.x == 1.1f)
        {
            transform.localScale = new Vector3(1.2f, 1.2f);
        }
        else if (transform.localScale.x == 1.2f)
        {
            transform.localScale = new Vector3(1.15f, 1.15f);
        }
        else if (transform.localScale.x == 1.15f)
        {
            transform.localScale = new Vector3(1.11f, 1.11f);
        }
        else if (transform.localScale.x == 1.11f)
        {
            transform.localScale = new Vector3(1.05f, 1.05f);
        }
        else if (transform.localScale.x == 1.05f)
        {
            transform.localScale = new Vector3(1.0f, 1.0f);
        }
    }
    #endregion
}
