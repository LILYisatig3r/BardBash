using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Assets._Scripts;

[CreateAssetMenu(fileName = "Actions", menuName = "Actions", order = 1)]
public class S_Actions : ScriptableObject {

    static S_Actions instance = null;
    public static S_Actions i
    {
        get
        {
            if (!instance)
                instance = Resources.FindObjectsOfTypeAll<S_Actions>().FirstOrDefault();
            return instance;
        }
    }

    public Dictionary<string, S_Action> actions;

    public int meleeHash { get; private set; }
    public int idleHash { get; private set; }
    public int castHash { get; private set; }
    public int dashAttackHash { get; private set; }

    [Space]
    [SerializeField] private Transform MagicBlastAOE;

    public S_Actions()
    {
        actions = new Dictionary<string, S_Action>();
        actions.Add("rest", new S_Action(0.25f, Rest));
        actions.Add("Cast", new S_Action(0.5f, S_PlayerCast));
        actions.Add("Dash", new S_Action(0.25f, S_PlayerDash));
        actions.Add("actorMove", new S_Action(0.25f, S_ActorMove));
        actions.Add("actorDestinationMove", new S_Action(0.25f, S_ActorDestinationMove));
        actions.Add("Melee", new S_Action(0.25f, S_ActorMelee));
        actions.Add("rogueDash", new S_Action(0.25f, S_RogueDash));
        actions.Add("magicBlast", new S_Action(0.25f, S_MagicBlast));
        actions.Add("explosiveEscape", new S_Action(0.25f, S_ExplosiveEscape));

        meleeHash = Animator.StringToHash("Melee");
        idleHash = Animator.StringToHash("Reset");
        castHash = Animator.StringToHash("Cast");
        dashAttackHash = Animator.StringToHash("Dash Attack");

    //MagicBlastAOE = AssetDatabase.LoadAssetAtPath<Transform>("Assets/_Prefabs/Magic Blast AOE.prefab");
    }

    private void OnEnable()
    {
        actions = new Dictionary<string, S_Action>();
        actions.Add("rest", new S_Action(0.25f, Rest));
        actions.Add("Cast", new S_Action(0.5f, S_PlayerCast));
        actions.Add("Dash", new S_Action(0.25f, S_PlayerDash));
        actions.Add("actorMove", new S_Action(0.25f, S_ActorMove));
        actions.Add("actorDestinationMove", new S_Action(0.25f, S_ActorDestinationMove));
        actions.Add("Melee", new S_Action(0.25f, S_ActorMelee));
        actions.Add("rogueDash", new S_Action(0.25f, S_RogueDash));
        actions.Add("magicBlast", new S_Action(0.25f, S_MagicBlast));
        actions.Add("explosiveEscape", new S_Action(0.25f, S_ExplosiveEscape));

        meleeHash = Animator.StringToHash("Melee");
        idleHash = Animator.StringToHash("Reset");
        castHash = Animator.StringToHash("Cast");
        dashAttackHash = Animator.StringToHash("Dash Attack");
    }

    public bool TryGetAction(string key, out S_Action value)
    {
        return actions.TryGetValue(key, out value);
    }

    private void Rest(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        pc.AnimationUpdate();
    }

    private void S_PlayerCast(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        Transform p = Instantiate(pc.projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        projectile.Spawn(pc, pc.directionMemory, pc.GetMagic());

        pc.SetAnimatorTrigger(castHash);
    }

    private void S_MagicBlast(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_Actor actor = a.GetComponent<S_Actor>();
        S_BattleRhythm br = (S_BattleRhythm)args[1];

        Vector3 detonationPoint = new Vector3();
        Vector3 testPoint;
        Vector3 p = actor.position;
        Vector3 attackDir = actor.directionMemory;
        if (actor.movementKeys != S_Key.None)
        {
            SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
            switch (actor.movementKeys)
            {
                case S_Key.Left:
                    attackDir = new Vector3(-1, 0, 0);
                    sr.flipX = true; break;
                case S_Key.Right:
                    attackDir = new Vector3(1, 0, 0);
                    sr.flipX = false; break;
                case S_Key.Up:
                    attackDir = new Vector3(0, 0, 1); break;
                case S_Key.Down:
                    attackDir = new Vector3(0, 0, -1); break;
            }
        }
        actor.SetDirectionMemory(attackDir);

        int i = 1;
        while (detonationPoint == Vector3.zero)
        {
            testPoint = new Vector3(p.x + (attackDir.x * i), p.y, p.z + (attackDir.z * i));
            DataTile testTile = br.GetTile(testPoint);
            if (testTile.occupant != null || i == 8)
                detonationPoint = testPoint;
            if (testTile.type != DataTile.TileType.grass)
                detonationPoint = new Vector3(p.x + (attackDir.x * --i), p.y, p.z + (attackDir.z * --i));
            i++;
        }

        int X = detonationPoint.x > 0 ? (int)detonationPoint.x - 1 : 0;
        int Z = detonationPoint.z > 0 ? (int)detonationPoint.z - 1 : 0;
        int hits = 0;
        for (int x = X; x < Mathf.Min(detonationPoint.x + 2, br.map.GetSize().x - 1); x++)
        {
            for (int z = Z; z < Mathf.Min(detonationPoint.z + 2, br.map.GetSize().y - 1); z++)
            {
                S_Actor occupant = br.GetTile(new Vector3(x, 0f, z)).occupant;
                if (occupant != null)
                {
                    br.MusicianDamaged(occupant, actor, actor.magic * 0.55f, true);
                    hits = Mathf.Min(hits + 1, 5);
                }
            }
        }

        Transform mb = Instantiate(MagicBlastAOE);
        mb.position = new Vector3(detonationPoint.x + 0.5f, detonationPoint.y, detonationPoint.z + 0.5f);

        actor.SetAnimatorTrigger(castHash);
        br.ShakeCamera(0.1f, 1.5f);
        br.PlaySoundInQuickSuccession(S_SoundEffectsSO.Instance.GetRandomBashSoundEffect(), hits);
    }

    private void S_ExplosiveEscape(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_Actor actor = a.GetComponent<S_Actor>();
        S_BattleRhythm br = (S_BattleRhythm)args[1];

        int X = actor.position.x > 0 ? (int)actor.position.x - 1 : 0;
        int Z = actor.position.z > 0 ? (int)actor.position.z - 1 : 0;
        for (int x = X; x < Mathf.Min(actor.position.x + 2, br.map.GetSize().x - 1); x++)
        {
            for (int z = Z; z < Mathf.Min(actor.position.z + 2, br.map.GetSize().y - 1); z++)
            {
                S_Actor occupant = br.GetTile(new Vector3(x, 0f, z)).occupant;
                if (occupant != null && !occupant.GetActorName().Equals(actor.GetActorName())) // && x != actor.position.x && z != actor.position.z)
                    br.MusicianDamaged(occupant, actor, actor.magic / 2f, true);
            }
        }

        Vector3 destinationPoint = actor.position;
        Vector3 testPoint;
        Vector3 p = actor.position;
        Vector3 dm = actor.directionMemory;

        int i = 5;
        while (destinationPoint == actor.position && i > 0)
        {
            testPoint = new Vector3(p.x - (dm.x * i), p.y, p.z - (dm.z * i));
            DataTile testTile = br.GetTile(testPoint);
            if (testTile.occupant == null && testTile.type == DataTile.TileType.grass)
                destinationPoint = testPoint;
            i--;
        }

        actor.SetAnimatorTrigger(castHash);
        br.ShakeCamera(0.1f, 1f);
        br.MusicianMove(actor, destinationPoint);
        actor.position = destinationPoint;
        actor.StartMove();
        //actor.AnimationUpdate();
    }

    private void S_PlayerDash(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_BattleRhythm br = (S_BattleRhythm)args[1];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        Vector3 newPos = new Vector3(pc.position.x + (pc.directionMemory.x * 2), 
            pc.position.y, pc.position.z + (pc.directionMemory.z * 2));
        Vector3 dest = br.MusicianMove(pc, newPos);
        if (dest.y != -1)
        {
            pc.position = dest;
            pc.StartMove();
        }
        else
        {
            newPos = new Vector3(pc.position.x + pc.directionMemory.x, pc.position.y, pc.position.z + pc.directionMemory.z);
            dest = br.MusicianMove(pc, newPos);
            if (dest.y != -1)
            {
                pc.position = dest;
                pc.StartMove();
            }
        }

        pc.SetAnimatorTrigger(idleHash);
    }

    private void S_ActorMove(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_BattleRhythm br = (S_BattleRhythm)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();
        Vector3 newPos = new Vector3();
        Vector3 newDirection = actor.directionMemory;
        SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
        switch (actor.movementKeys)
        {
            case S_Key.Up:
                newPos = new Vector3(actor.position.x, actor.position.y, actor.position.z + 1);
                newDirection = new Vector3(0, 0, 1);
                break;
            case S_Key.Right:
                newPos = new Vector3(actor.position.x + 1, actor.position.y, actor.position.z);
                newDirection = new Vector3(1, 0, 0);
                sr.flipX = false;
                break;
            case S_Key.Down:
                newPos = new Vector3(actor.position.x, actor.position.y, actor.position.z - 1);
                newDirection = new Vector3(0, 0, -1);
                break;
            case S_Key.Left:
                newPos = new Vector3(actor.position.x - 1, actor.position.y, actor.position.z);
                newDirection = new Vector3(-1, 0, 0);
                sr.flipX = true;
                break;
        }
        Vector3 dest = br.MusicianMove(actor, newPos);
        if (dest.z != -1)
        {
            actor.position = dest;
            actor.StartMove();
        }
        actor.SetDirectionMemory(newDirection);

        actor.SetAnimatorTrigger(idleHash);
    }

    private void S_ActorDestinationMove(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_BattleRhythm br = (S_BattleRhythm)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();
        Vector3 newDirection = actor.currentDirection;
        Vector3 newPos = actor.position + newDirection;
        SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
        if (Mathf.Abs(newDirection.x) > Mathf.Abs(newDirection.z))
        {
            if (newDirection.x > 0)
                sr.flipX = false;
            else if (newDirection.x < 0)
                sr.flipX = true;
        }
        Vector3 dest = br.MusicianMove(actor, newPos);
        if (dest.z != -1)
        {
            actor.position = dest;
            actor.StartMove();
        }
        actor.SetDirectionMemory(newDirection);

        actor.SetAnimatorTrigger(idleHash);
    }

    private void S_ActorMelee(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_BattleRhythm br = (S_BattleRhythm)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();

        Vector3 p = actor.position;
        Vector3 attackDir = actor.directionMemory;
        if (actor.movementKeys != S_Key.None)
        {
            SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
            switch (actor.movementKeys)
            {
                case S_Key.Left:
                    attackDir = new Vector3(-1, 0, 0);
                    sr.flipX = true; break;
                case S_Key.Right:
                    attackDir = new Vector3(1, 0, 0);
                    sr.flipX = false; break;
                case S_Key.Up:
                    attackDir = new Vector3(0, 0, 1); break;
                case S_Key.Down:
                    attackDir = new Vector3(0, 0, -1); break;
            }
        }
        actor.SetDirectionMemory(attackDir);
        Vector3 target = new Vector3(p.x + attackDir.x, p.y, p.z + attackDir.z);
        S_Actor a1 = br.CheckTileOccupant(target);

        if (a1 && actor.team != a1.team)
        {
            float damage = attackDir == a1.directionMemory ? actor.GetAttack() * 1.5f : actor.GetAttack();
            br.MusicianDamaged(a1, actor, damage, false);
            br.PlaySound(actor.GetWeapon().GetMeleeSound());
            br.ShakeCamera(0.1f, 1f);
        }

        actor.SetAnimatorTrigger(meleeHash);
    }

    private void S_RogueDash(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_BattleRhythm br = (S_BattleRhythm)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();

        S_Actor a1, a2;
        Vector3 p = actor.position;
        Vector3 dm = actor.directionMemory;
        Vector3 p1 = new Vector3(p.x + dm.x, p.y, p.z + dm.z);
        Vector3 p2 = new Vector3(p.x + (dm.x * 2), p.y, p.z + (dm.z * 2));
        Vector3 p3 = new Vector3(p.x + (dm.x * 3), p.y, p.z + (dm.z * 3));
        a1 = br.CheckTileOccupant(p1);
        a2 = br.CheckTileOccupant(p2);
        bool hitActor = false;

        Vector3 dest = a2 != null ? br.MusicianMove(actor, p3) : br.MusicianMove(actor, p2);
        if (dest.y != -1)
        {
            SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
            actor.position = dest;
            actor.StartMove();
            if (a1 && a1.team != actor.team)
            {
                br.MusicianDamaged(a1, actor, actor.GetAttack() * 0.6f, false);
                actor.SetDirectionMemory(-actor.directionMemory);
                sr.flipX = !sr.flipX;
                hitActor = true;
                actor.SetAnimatorTrigger(dashAttackHash);
                br.ShakeCamera(0.1f, 1f);
                br.PlaySound(S_SoundEffectsSO.Instance.GetRandomShingSoundEffect());
            }
            if (a2 && a2.team != actor.team)
            {
                br.MusicianDamaged(a2, actor, actor.GetAttack() * 0.6f, false);
                if (!hitActor)
                {
                    actor.SetDirectionMemory(-actor.directionMemory);
                    sr.flipX = !sr.flipX;
                    hitActor = true;
                    actor.SetAnimatorTrigger(dashAttackHash);
                    br.ShakeCamera(0.1f, 1f);
                    br.PlaySound(S_SoundEffectsSO.Instance.GetRandomShingSoundEffect());
                }
            }

            if (!hitActor)
                actor.SetAnimatorTrigger(idleHash);
        }
        else
        {
            dest = br.MusicianMove(actor, p1);
            if (dest.y != -1)
            {
                actor.position = dest;
                actor.StartMove();
                actor.SetAnimatorTrigger(idleHash);
            }
        }

    }
}
