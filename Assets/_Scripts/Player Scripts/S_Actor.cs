using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Actor : MonoBehaviour {

    [Header("Actor Stats")]
    [SerializeField] protected string actorName;
    [SerializeField] protected int speed;
    [SerializeField] public int maxHp;
    [SerializeField] public int curHp;
    [SerializeField] protected int attack;
    [SerializeField] protected int defense;
    [SerializeField] public float measures;

    [Header("Other")]
    [SerializeField] protected GameManager gm;
    private float clock;
    public Vector3 position;
    public Vector3 directionMemory;
    public KeyCode preppedInput;
    public S_Action action;

    #region Getters and Setters
    public string GetActorName()
    {
        return actorName;
    }

    public int GetSpeed()
    {
        return speed;
    }

    public void SetSpeed(int s)
    {
        speed = s;
    }

    public int GetMaxHp()
    {
        return maxHp;
    }

    public void SetMaxHp(int hp)
    {
        maxHp = hp;
    }

    public int GetCurrentHp()
    {
        return curHp;
    }

    public void SetCurrentHp(int hp)
    {
        curHp = hp;
    }

    public int GetAttack()
    {
        return attack;
    }

    public void SetAttack(int a)
    {
        attack = a;
    }

    public int GetDefense()
    {
        return defense;
    }

    public void SetDefense(int d)
    {
        defense = d;
    }

    public float GetMeasures()
    {
        return measures;
    }

    public void SetMeasures(float m)
    {
        measures = m;
    }

    virtual public float GetActionCost()
    {
        return measures;
    }

    #endregion

    #region External Functions

    virtual public void ReceiveBeat()
    {
        return;
    }

    public void StartMove()
    {
        StartCoroutine("Mover");
    }

    private IEnumerator Mover()
    {
        bool mv = true;
        while (mv)
        {
            Vector3 dest = gm.GetMap().GridToWorldPosition(position);
            dest = new Vector3(dest.x + 0.5f, dest.y + 0.5f, dest.z + 0.5f);
            float xMovement = 0, zMovement = 0;
            if (dest.x != transform.position.x)
                xMovement = Math.Abs(dest.x - transform.position.x) > 0.1f ? (dest.x - transform.position.x) / 2 : dest.x - transform.position.x;
            else if (dest.z != transform.position.z)
                zMovement = Math.Abs(dest.z - transform.position.z) > 0.1f ? (dest.z - transform.position.z) / 2 : dest.z - transform.position.z;
            else
                mv = false;

            transform.position = new Vector3(transform.position.x + xMovement, transform.position.y, transform.position.z + zMovement);
            yield return new WaitForSeconds(.02f);
        }
        StopCoroutine("Mover");
    }

    virtual public void AnimationUpdate()
    {
        return;
    }

    #endregion
}
