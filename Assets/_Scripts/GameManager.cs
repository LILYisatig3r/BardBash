using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using Assets._Scripts;
using Rewired;

/// <summary>
/// Contains methods that perform vital game functions
/// </summary>
public class GameManager : MonoBehaviour {

    [SerializeField] Transform banner;
    [SerializeField] S_PlayerController player;
    [SerializeField] GraphicsTilesMap map;
    [SerializeField] string trackName;
    [SerializeField] public S_Actor[] playerCharacters;
    [SerializeField] public S_Actor[] npcCharacters;
    [SerializeField] public S_Actions actions;
    public S_BattleRhythm battleRhythm { get; private set; }
    private Player playerController;

    private const int MoveHorizontal = 0;
    private const int MoveVertical = 1;
    private const int Ability0 = 2;
    private const int Ability1 = 3;
    private const int Ability2 = 4;
    private const int Ability3 = 5;

    private static Dictionary<GameObject, S_Actor> actors;
    private static GameManager instance = null;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        playerController = ReInput.players.GetPlayer(0);
        battleRhythm = GetComponent<S_BattleRhythm>();
        instance = this;
        map.BuildMesh();
        Vector2 mapSize = map.GetSize();
        transform.position = new Vector3(mapSize.x / 2, 0, mapSize.y / 2);
        actors = new Dictionary<GameObject, S_Actor>();
    }

    public void Update()
    {
        //float x = playerController.GetAxis(MoveHorizontal);
        //float y = playerController.GetAxis(MoveVertical);
        //Debug.Log(new Vector2(x, y));

        if (playerController.GetAnyButtonDown())
        {
            if (battleRhythm != null)
                battleRhythm.ReceiveInput(playerController);
        }
    }

    public void AddActor(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();
    }

    public void ActorFinishAction(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();

        battleRhythm.BeatUpdate();
    }

    public void FinishBattle()
    {
    }

    public S_Actor CheckTileOccupant(Vector3 tile)
    {
        return map.GetTile((int)tile.x, (int)tile.z).occupant;
    }

    public DataTile GetTile(Vector3 tile)
    {
        return map.GetTile((int)tile.x, (int)tile.z);
    }

    public Stack<Vector3> GetPath(Vector3 a, Vector3 b)
    {
        return map.GetPath(a, b);
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

    public void PrintOccupants()
    {
        map.PrintOccupants();
    }

}
