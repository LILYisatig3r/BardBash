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
    [SerializeField] int aggroDistance;
    private bool directionSwitch;
    //[SerializeField] Vector3 position;

    Dictionary<KeyCode, S_Action> actions;

    S_Action move;
    S_Action melee;
    S_Action skip;

    Vector3[] unitDirections;
    Vector3[] cardinals;

    private Stack<Vector3> path;

    void Start () {

        if (gm != null || GameManager.TryGetInstance(out gm))
            gm.AddActor(gameObject);
        battleRhythm = gm.battleRhythm;
        actions = new Dictionary<KeyCode, S_Action>();
        animationController = transform.GetChild(0).GetComponent<S_AnimationController>();
        animationController.SetActor(actorName);

        S_Actions.i.TryGetAction("actorDestinationMove", out move);
        S_Actions.i.TryGetAction("Melee", out melee);
        S_Actions.i.TryGetAction("Skip", out skip);

        SetDirectionMemory(new Vector3(0f, 0f, -1f));
        position = battleRhythm.MusicianRandomSpawn(this);
        transform.position = battleRhythm.map.GridToWorldPosition(position);
        transform.position = new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z + 0.5f);

        unitDirections = new Vector3[4];
        unitDirections[0] = new Vector3(0, 0, 1);
        unitDirections[1] = new Vector3(1, 0, 0);
        unitDirections[2] = new Vector3(0, 0, -1);
        unitDirections[3] = new Vector3(-1, 0, 0);

        cardinals = new Vector3[4];

        //br.AddMusician(this);
    }

    public override void BeatUpdate()
    {
        base.BeatUpdate();
        DecideAction();
    }

    public override void PrepForNewTurn()
    {
        base.PrepForNewTurn();
        //directionMemory = Vector3.zero;
        directionSwitch = true;
        PathToNearestPlayer();
    }

    override public float GetActionCost()
    {
        return action != null ? action.cost : 0.25f;
    }

    public void DecideAction()
    {
        // Try to attack a Player in a neighbouring tile
        S_Actor occupant;
        for (int i = 0; i < 4; i++)
        {
            cardinals[i] = position + unitDirections[i];
            occupant = gm.CheckTileOccupant(cardinals[i]);
            if (occupant is S_PlayerController)
            {
                SetDirectionMemory(unitDirections[i]);
                melee.Invoker(gameObject, battleRhythm);
                return;
            }
        }

        if (path != null && path.Count > 1)
        {
            Vector3 dest = path.Pop();
            currentDirection = dest - position;
            move.Invoker(gameObject, battleRhythm);
        }

        else
        {
            skip.Invoker(gameObject, battleRhythm);
        }

        //    // If you're not moving, pick the shortest plane of distance that isn't 0. You are now moving.
        //    //
        //    // If you're moving, try to keep moving in the same direction:
        //    //  If you can, do so. **
        //    //  If you can't:
        //    //      Try to move in the other plane:
        //    //          If you can, do so. **
        //    //          If you can't, do nothing. **
        //}
    }

    private void PathToNearestPlayer()
    {
        int minDistance = aggroDistance;
        int distance;
        S_Actor aggroedPlayer = null;
        foreach (S_Actor player in battleRhythm.players)
        {
            distance = GetManhattanDistance(player);
            if (distance < minDistance)
            {
                minDistance = distance;
                aggroedPlayer = player;
            }
        }

        if (aggroedPlayer != null)
        {
            path = gm.GetPath(position, aggroedPlayer.position, new List<DataTile.TileType>(){DataTile.TileType.stone});
        }
    }

}
