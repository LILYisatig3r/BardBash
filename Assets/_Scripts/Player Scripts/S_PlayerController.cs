using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using Rewired;
using Assets._Scripts;

public class S_PlayerController : S_Actor {

    #region Hidden Variables
    private Rigidbody rb;
    int latencyDelay = 0;
    float compensatePostBeat = 0;
    float compensatePreBeat = 0;
    private int bufferFrames;
    private int _bufferFrames;
    Dictionary<S_Key, S_Action> actions;
    private bool moving = false;
    private Player controller;

    private const int MoveLeft = 0;
    private const int MoveRight = 1;
    private const int MoveUp = 2;
    private const int MoveDown = 3;
    private const int Ability0 = 4;
    private const int Ability1 = 5;
    private const int Ability2 = 6;
    private const int Ability3 = 7;

    #endregion

    #region Inspector Variables
    [SerializeField] string trackName;
    [SerializeField] string heroName;
    [SerializeField] public Transform projectilePrefab;
    #endregion

    #region Monobehaviour
    void Start () {
        animationController = transform.GetChild(0).GetComponent<S_AnimationController>();
        animationController.SetActor(actorName);
        rb = GetComponent<Rigidbody>();
        actions = new Dictionary<S_Key, S_Action>();
        controller = ReInput.players.GetPlayer(0);

        //S_Action rest;
        //S_Actions.TryGetAction("rest", out rest);
        //actions.Add(S_Key.None, rest);

        S_Action move;
        S_Actions.i.TryGetAction("actorMove", out move);
        actions.Add(S_Key.Up, move);
        actions.Add(S_Key.Right, move);
        actions.Add(S_Key.Down, move);
        actions.Add(S_Key.Left, move);

        S_Action ability1;
        S_Actions.i.TryGetAction(a1, out ability1);
        actions.Add(S_Key.Q, ability1);

        S_Action ability2;
        S_Actions.i.TryGetAction(a2, out ability2);
        actions.Add(S_Key.W, ability2);

        S_Action ability3;
        S_Actions.i.TryGetAction(a3, out ability3);
        actions.Add(S_Key.E, ability3);

        S_Action ability4;
        S_Actions.i.TryGetAction(a4, out ability4);
        actions.Add(S_Key.R, ability4);

        movementInputs = new KeyBuffer(2);
        bufferedMovement = new KeyBuffer(2);
        actionInputs = new KeyBuffer(4);
        bufferedAction = new KeyBuffer(4);
        _bufferFrames = 3;

        if (gm != null || GameManager.TryGetInstance(out gm))
            gm.AddActor(gameObject);
        battleRhythm = gm.battleRhythm;

        SetDirectionMemory(new Vector3(0f, 0f, -1f));
        position = battleRhythm.MusicianRandomSpawn(this);
        transform.position = battleRhythm.map.GridToWorldPosition(position);
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z + 0.5f);

        animationController.bpm = battleRhythm.GetBPM();

        //br.AddMusician(this);
    }

    void Update()
    {
        if ((bufferedMovement.Count != 0 || bufferedAction.Count != 0) &&
            bufferFrames-- != _bufferFrames)
        {
            GetInputs();
            bufferedMovement.AddFrom(movementInputs);
            bufferedAction.AddFrom(actionInputs);

            if (bufferFrames <= 0)
            {
                actionKeys = bufferedAction.Or();
                movementKeys = bufferedMovement.Or();
                if (actions.TryGetValue(actionKeys, out action))
                {
                    action.Invoker(gameObject, battleRhythm);
                    ResetInputs();
                }
                else if (actions.TryGetValue(movementKeys, out action))
                {
                    action.Invoker(gameObject, battleRhythm);
                    ResetInputs();
                }
            }
        }
    }

    #endregion

    #region External functions

    public bool TryAction()
    {
        GetInputs();
        if (actionInputs.Count != 0)
        {
            // If only an action key is held, buffer it for x frames
            actionInputs.Copy(ref bufferedAction);
            bufferFrames = _bufferFrames;
        }
        if (movementInputs.Count != 0)
        {
            // If only a movement key is held, buffer it for x frames
            movementInputs.Copy(ref bufferedMovement);
            bufferFrames = _bufferFrames;
        }
        return (actionInputs.Count != 0 || movementInputs.Count != 0);
    }

    private void GetInputs()
    {
        if (controller.GetAnyButtonDown())
        {
            int i = 0;
            movementInputs.Clear();
            try
            {
                if (controller.GetButtonDown(MoveLeft))
                    movementInputs.Add(S_Key.Left);
                else if (controller.GetButtonDown(MoveRight))
                    movementInputs.Add(S_Key.Right);
                else if (controller.GetButtonDown(MoveUp))
                    movementInputs.Add(S_Key.Up);
                else if (controller.GetButtonDown(MoveDown))
                    movementInputs.Add(S_Key.Down);
            }
            catch (IndexOutOfRangeException e) { }

            i = 0;
            actionInputs.Clear();
            try
            {
                if (controller.GetButtonDown(Ability0))
                    actionInputs.Add(S_Key.Q);
                else if (controller.GetButtonDown(Ability1))
                    actionInputs.Add(S_Key.W);
                else if (controller.GetButtonDown(Ability2))
                    actionInputs.Add(S_Key.E);
                else if (controller.GetButtonDown(Ability3))
                    actionInputs.Add(S_Key.R);
            }
            catch (IndexOutOfRangeException e) { }
        }
    }

    public override void ResetInputs()
    {
        base.ResetInputs();
        movementInputs.Clear();
        bufferedMovement.Clear();
        actionInputs.Clear();
        bufferedAction.Clear();
    }

    override public void ReceiveBeat()
    {
        compensatePostBeat = 10;
    }

    override public void AnimationUpdate()
    {
    }

    override public float GetActionCost()
    {
        return action != null ? action.cost : 0.25f;
    }
    #endregion
}
