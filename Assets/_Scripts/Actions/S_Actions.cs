using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Actions : MonoBehaviour {

    public static Dictionary<string, S_Action> actions;

    static S_Actions()
    {
        actions = new Dictionary<string, S_Action>();
        actions.Add("rest", new S_Action(0.25f, Rest));
        actions.Add("playerMove", new S_Action(0.25f, S_PlayerMove));
        actions.Add("Cast", new S_Action(0.5f, S_PlayerCast));
        actions.Add("Dash", new S_Action(0.25f, S_PlayerDash));
        actions.Add("actorMove", new S_Action(0.25f, S_ActorMove));
        actions.Add("Melee", new S_Action(0.25f, S_ActorMelee));
        actions.Add("rogueDash", new S_Action(0.25f, S_RogueDash));
    }

    public static bool TryGetAction(string key, out S_Action value)
    {
        return actions.TryGetValue(key, out value);
    }

    private static void Rest(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        pc.AnimationUpdate();
    }

    private static void S_PlayerMove(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        GameManager gm = (GameManager)args[1];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        Vector3 newPos = new Vector3();
        Vector3 newDirection = pc.directionMemory;
        switch (pc.preppedInput)
        {
            case KeyCode.UpArrow:
                newPos = new Vector3(pc.position.x, pc.position.y, pc.position.z + 1);
                newDirection = new Vector3(0, 0, 1);
                break;
            case KeyCode.RightArrow:
                newPos = new Vector3(pc.position.x + 1, pc.position.y, pc.position.z);
                newDirection = new Vector3(1, 0, 0);
                break;
            case KeyCode.DownArrow:
                newPos = new Vector3(pc.position.x, pc.position.y, pc.position.z - 1);
                newDirection = new Vector3(0, 0, -1);
                break;
            case KeyCode.LeftArrow:
                newPos = new Vector3(pc.position.x - 1, pc.position.y, pc.position.z);
                newDirection = new Vector3(-1, 0, 0);
                break;
        }
        Vector3 dest = gm.ActorMove(a, newPos);
        if (dest.z != -1)
        {
            pc.position = dest;
            pc.StartMove();
        }
        //float scaleX = a.transform.localScale.x;
        SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
        if (newDirection.x != 0 && newDirection.x != pc.directionMemory.x)
            sr.flipX = !sr.flipX;
        //a.transform.localScale = new Vector3(scaleX, a.transform.localScale.y, a.transform.localScale.z + 0.5f);
        pc.directionMemory = newDirection;

        pc.AnimationUpdate();
        pc.preppedInput = KeyCode.None;
    }

    private static void S_PlayerCast(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        Transform p = Instantiate(pc.projectilePrefab) as Transform;
        Projectile projectile = p.GetComponent<Projectile>();
        projectile.Spawn(a.transform.position, pc.directionMemory, pc.GetMagic());

        //S_AnimationController ac = a.GetComponentInChildren<S_AnimationController>();
        S_AnimationController ac = a.transform.GetChild(0).GetComponent<S_AnimationController>();

        ac.SetAnimation("Casting");

        pc.AnimationUpdate();
        pc.preppedInput = KeyCode.None;
    }

    private static void S_PlayerDash(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        GameManager gm = (GameManager)args[1];
        S_PlayerController pc = a.GetComponent<S_PlayerController>();
        Vector3 newPos = new Vector3(pc.position.x + (pc.directionMemory.x * 2), 
            pc.position.y, pc.position.z + (pc.directionMemory.z * 2));
        Vector3 dest = gm.ActorMove(a, newPos);
        if (dest.y != -1)
        {
            pc.position = dest;
            pc.StartMove();
        }
        else
        {
            newPos = new Vector3(pc.position.x + pc.directionMemory.x, pc.position.y, pc.position.z + pc.directionMemory.z);
            dest = gm.ActorMove(a, newPos);
            if (dest.y != -1)
            {
                pc.position = dest;
                pc.StartMove();
            }
        }

        pc.AnimationUpdate();
        pc.preppedInput = KeyCode.None;
    }

    private static void S_ActorMove(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        GameManager gm = (GameManager)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();
        Vector3 newPos = new Vector3();
        Vector3 newDirection = actor.directionMemory;
        switch (actor.preppedInput)
        {
            case KeyCode.UpArrow:
                newPos = new Vector3(actor.position.x, actor.position.y, actor.position.z + 1);
                newDirection = new Vector3(0, 0, 1);
                break;
            case KeyCode.RightArrow:
                newPos = new Vector3(actor.position.x + 1, actor.position.y, actor.position.z);
                newDirection = new Vector3(1, 0, 0);
                break;
            case KeyCode.DownArrow:
                newPos = new Vector3(actor.position.x, actor.position.y, actor.position.z - 1);
                newDirection = new Vector3(0, 0, -1);
                break;
            case KeyCode.LeftArrow:
                newPos = new Vector3(actor.position.x - 1, actor.position.y, actor.position.z);
                newDirection = new Vector3(-1, 0, 0);
                break;
        }
        Vector3 dest = gm.ActorMove(a, newPos);
        if (dest.z != -1)
        {
            actor.position = dest;
            actor.StartMove();
        }
        //float scaleX = a.transform.localScale.x;
        SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
        if (newDirection.x != 0 && newDirection.x != actor.directionMemory.x)
            sr.flipX = (newDirection.x < 0);
        //a.transform.localScale = new Vector3(scaleX, a.transform.localScale.y, a.transform.localScale.z + 0.5f);
        actor.directionMemory = newDirection;

        actor.AnimationUpdate();
        actor.preppedInput = KeyCode.None;
    }

    private static void S_ActorMelee(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        GameManager gm = (GameManager)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();

        Vector3 p = actor.position;
        Vector3 dm = actor.directionMemory;
        Vector3 target = new Vector3(p.x + dm.x, p.y, p.z + dm.z);
        S_Actor a1 = gm.CheckTileOccupant(target);

        if (a1)
            gm.ActorDamaged(a1.gameObject, actor.GetAttack());

        S_AnimationController ac = a.transform.GetChild(0).GetComponent<S_AnimationController>();
        ac.SetAnimation("Melee");
        actor.AnimationUpdate();
        actor.preppedInput = KeyCode.None;
    }

    private static void S_RogueDash(params object[] args)
    {
        GameObject a = (GameObject)args[0];
        GameManager gm = (GameManager)args[1];
        S_Actor actor = a.GetComponent<S_Actor>();

        S_Actor a1, a2;
        Vector3 p = actor.position;
        Vector3 dm = actor.directionMemory;
        Vector3 p1 = new Vector3(p.x + dm.x, p.y, p.z + dm.z);
        Vector3 p2 = new Vector3(p.x + (dm.x * 2), p.y, p.z + (dm.z * 2));
        Vector3 p3 = new Vector3(p.x + (dm.x * 3), p.y, p.z + (dm.z * 3));
        a1 = gm.CheckTileOccupant(p1);
        a2 = gm.CheckTileOccupant(p2);

        Vector3 dest = a2 != null ? gm.ActorMove(a, p3) : gm.ActorMove(a, p2);
        if (dest.y != -1)
        {
            SpriteRenderer sr = a.GetComponentInChildren<SpriteRenderer>();
            actor.position = dest;
            actor.StartMove();
            if (a1)
            {
                gm.ActorDamaged(a1.gameObject, actor.GetAttack() * 0.6f);
                actor.directionMemory = -actor.directionMemory;
                sr.flipX = !sr.flipX;
            }
            if (a2)
            {
                gm.ActorDamaged(a2.gameObject, actor.GetAttack() * 0.6f);
                if (!a1)
                {
                    actor.directionMemory = -actor.directionMemory;
                    sr.flipX = !sr.flipX;
                }
            }
        }
        else
        {
            dest = gm.ActorMove(a, p1);
            if (dest.y != -1)
            {
                actor.position = dest;
                actor.StartMove();
            }
        }

        actor.AnimationUpdate();
        actor.preppedInput = KeyCode.None;
    }
}
