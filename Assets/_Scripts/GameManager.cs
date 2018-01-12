using System.Collections;
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

    public void ActorDamaged(GameObject a, int damage)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
        {
            actors[a] = actor = a.GetComponent<S_Actor>();
        }

        actor.SetCurrentHp(actor.GetCurrentHp() - damage);
        ui.UpdateActorHealth(actor.GetCurrentHp(), actor.GetMaxHp(), a.tag);
        if (actor.GetCurrentHp() <= 0)
        {
            Destroy(actor.gameObject);
        }
    }

    public Vector3 ActorMove(GameObject a, Vector3 destination)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = actor = a.GetComponent<S_Actor>();

        DataTile destTile = map.GetTile((int)destination.x, (int)destination.z);
        if (destTile.type == DataTile.tileType.grass && !destTile.occupied)
        {
            map.GetTile((int)actor.position.x, (int)actor.position.z).occupied = false;
            destTile.occupied = true;
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
        spawnPoint.occupied = true;
        return map.OccupyRandomWalkableTile();
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

    public void ActorFinishAction(GameObject a)
    {
        S_Actor actor;
        if (!actors.TryGetValue(a, out actor))
            actors[a] = a.GetComponent<S_Actor>();

        battleRhythm.BeatUpdate();
    }

    void ReceiveBeat(KoreographyEvent e)
    {
        //foreach (KeyValuePair<GameObject, Actor> kvp in actors)
        //    kvp.Value.ReceiveBeat();

        battleRhythm.GetActiveMusician().ReceiveBeat();
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
