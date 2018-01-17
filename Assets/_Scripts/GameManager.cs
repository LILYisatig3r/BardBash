﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class GameManager : MonoBehaviour {

    [SerializeField] UIManager ui;
    [SerializeField] S_PlayerController player;
    [SerializeField] GraphicsTilesMap map;
    [SerializeField] string trackName;
    private S_BattleRhythm battleRhythm;

    private static Dictionary<GameObject, S_Actor> actors;
    private static GameManager instance = null;

    void Start()
    {
        Koreographer.Instance.RegisterForEvents(trackName, ReceiveBeat);
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        battleRhythm = GetComponent<S_BattleRhythm>();
        instance = this;
        map.BuildMesh();
        actors = new Dictionary<GameObject, S_Actor>();
    }

    public void ActorDamaged(GameObject a, float damage)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
        {
            actors[a] = actor = a.GetComponent<S_Actor>();
        }

        actor.SetCurrentHp(actor.GetCurrentHp() - damage);
        actor.PopupText("" + damage);
        //ui.UpdateActorHealth(actor.GetCurrentHp(), actor.GetMaxHp(), a.tag);
        if (actor.GetCurrentHp() <= 0)
        {
            ActorExitBattle(a);
        }
    }

    public Vector3 ActorMove(GameObject a, Vector3 destination)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = actor = a.GetComponent<S_Actor>();

        DataTile destTile = map.GetTile((int)destination.x, (int)destination.z);
        if (destTile.type == DataTile.TileType.grass && !destTile.occupant)
        {
            map.GetTile((int)actor.position.x, (int)actor.position.z).occupant = null;
            destTile.occupant = actor;
            return destination;
        }
        else
            return new Vector3(-1f, -1f, -1f);
    }

    public Vector3 ActorRandomSpawn(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = actor = a.GetComponent<S_Actor>();

        DataTile spawnPoint = map.GetRandomWalkableTile();
        spawnPoint.occupant = actor;
        return map.OccupyRandomWalkableTile(actor);
    }

    public void AddActor(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();
    }

    public void ActorEnterBattle(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();

        battleRhythm.AddMusician(a);
    }

    public void ActorExitBattle(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();

        if (battleRhythm.RemoveMusician(a) && battleRhythm.GetMusicianCount() <= 1)
            FinishBattle();
        Destroy(a);
    }

    public void ActorFinishAction(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();

        battleRhythm.BeatUpdate();
    }

    private void FinishBattle()
    {
        battleRhythm.currentState = S_BattleRhythm.State.finished;
    }

    void ReceiveBeat(KoreographyEvent e)
    {
        if (battleRhythm.currentState == S_BattleRhythm.State.playing)
            battleRhythm.GetActiveMusician().ReceiveBeat();
        else
            battleRhythm.BeatUpdate();
    }

    public S_Actor CheckTileOccupant(Vector3 tile)
    {
        return map.GetTile((int)tile.x, (int)tile.z).occupant;
    }

    public GraphicsTilesMap GetMap()
    {
        return map;
    }

    public static bool TryGetInstance(out GameManager gm)
    {
        gm = instance;
        if (instance == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
}
