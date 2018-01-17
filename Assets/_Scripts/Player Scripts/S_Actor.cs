using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Actor : MonoBehaviour {

    [Header("Actor Stats")]
    [SerializeField] protected string actorName;
    [SerializeField] protected int speed;
    [SerializeField] public float maxHp;
    [SerializeField] public float curHp;
    [SerializeField] protected float attack;
    [SerializeField] protected float magic;
    [SerializeField] protected float defense;
    [SerializeField] protected float magicDefense;
    [SerializeField] public float measures;

    [Header("Abilities")]
    [SerializeField] protected string a1;
    [SerializeField] protected string a2;
    [SerializeField] protected string a3;
    [SerializeField] protected string a4;

    [Header("Other")]
    [SerializeField] protected GameManager gm;
    [SerializeField] protected TextMesh popupText;
    //[SerializeField] protected S_Instrument instrument;

    protected S_AnimationController ac;
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

    public float GetMaxHp()
    {
        return maxHp;
    }

    public void SetMaxHp(float hp)
    {
        maxHp = hp;
    }

    public float GetCurrentHp()
    {
        return curHp;
    }

    public void SetCurrentHp(float hp)
    {
        curHp = hp;
    }

    public float GetAttack()
    {
        return attack;
    }

    public void SetAttack(float a)
    {
        attack = a;
    }

    public float GetMagic()
    {
        return magic;
    }

    public void SetMagic(float m)
    {
        magic = m;
    }

    public float GetDefense()
    {
        return defense;
    }

    public void SetDefense(float d)
    {
        defense = d;
    }

    public float GetMagicDefense()
    {
        return magicDefense;
    }

    public void SetMagicDefense(float md)
    {
        magicDefense = md;
    }

    public float GetMeasures()
    {
        return measures;
    }

    public void SetMeasures(float m)
    {
        measures = m;
    }

    public float GetClock()
    {
        return clock;
    }

    public void SetClock(float c)
    {
        clock = c;
    }

    virtual public float GetActionCost()
    {
        return measures;
    }

    public string[] GetAbilities()
    {
        return new string[4] { a1, a2, a3, a4 };
    }

    #endregion

    #region External Functions

    public void PopupText(string message)
    {
        TextMesh popup = Instantiate(popupText);
        popup.transform.SetParent(transform);
        popup.text = message;
        popup.color = UnityEngine.Random.ColorHSV();
        popup.transform.position = new Vector3(position.x, position.y + 1f, position.z);
        StartCoroutine("TextMover",popup);
    }

    private IEnumerator TextMover(TextMesh t)
    {
        float n = 10;
        Transform tr = t.transform;
        float x = UnityEngine.Random.Range(tr.position.x - 1f, tr.position.x + 1f);
        float dx = (x - tr.position.x) / n;
        float y = UnityEngine.Random.Range(tr.position.y + 1f, tr.position.y + 2f);
        float dy = (y - tr.position.y) / n;
        for (int i = 0; i < n; i++)
        {
            tr.position = new Vector3(tr.position.x + dx, tr.position.y + dy, tr.position.z);
            yield return new WaitForSeconds(0.05f);
        }
        Destroy(t.gameObject);
        //StopCoroutine("TextMover");
    }

    public void ResetAnimation()
    {
        ac.SetAnimation("Idle");
    }

    virtual public Sprite GetPortrait()
    {
        return ac.GetPortrait();
    }

    virtual public Color GetPrimaryColor()
    {
        return ac.GetPrimaryColor();
    }

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
