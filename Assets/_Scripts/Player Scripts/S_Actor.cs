using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Scripts;

public class S_Actor : MonoBehaviour {

    [Header("Actor Stats")]
    [SerializeField] protected string actorName;
    [SerializeField] public E_Team team;

    [Header("Combat Stats")]
    [SerializeField] public float speed;
    [SerializeField] public float maxHp;
    [SerializeField] public float curHp;
    [SerializeField] public float attack;
    [SerializeField] public float piercing;
    [SerializeField] public float defense;
    [SerializeField] public float magic;
    [SerializeField] public float magicPiercing;
    [SerializeField] public float magicDefense;
    [SerializeField] public float measures;

    [Header("Core Stats")]
    [SerializeField] protected float force;
    [SerializeField] protected float finesse;
    [SerializeField] protected float fortitude;
    [SerializeField] protected float intellect;
    [SerializeField] protected float intuition;
    [SerializeField] protected float inveiglement;

    [Header("Temperament Stats")]
    [SerializeField] protected float empathic;
    [SerializeField] protected float rational;
    [SerializeField] protected float untethered;
    [SerializeField] protected float steadfast;
    [SerializeField] protected float energized;
    [SerializeField] protected float peaceful;

    [Header("Abilities")]
    [SerializeField] protected string a1;
    [SerializeField] protected string a2;
    [SerializeField] protected string a3;
    [SerializeField] protected string a4;

    [Header("Other")]
    [SerializeField] protected GameManager gm;
    [SerializeField] protected TextMesh popupText;
    [SerializeField] protected SpriteRenderer popupSprite;
    [SerializeField] protected S_Instrument instrument;
    [SerializeField] protected S_Weapon weapon;


    public S_BattleRhythm battleRhythm;
    public S_AnimationController animationController;
    public float clock;
    public Vector3 position;
    public Vector3 currentDirection;
    public Vector3 directionMemory { get; private set; }
    public Transform directionPointer;
    public KeyBuffer movementInputs;
    public KeyBuffer bufferedMovement;
    public S_Key movementKeys;
    public KeyBuffer actionInputs;
    public KeyBuffer bufferedAction;
    public S_Key actionKeys;
    public KeyCode lastInput;
    public S_Action action;

    private Quaternion upRotation = Quaternion.Euler(90f,90f,90f);
    private Quaternion rightRotation = Quaternion.Euler(90f, 180f, 90f);
    private Quaternion downRotation = Quaternion.Euler(90f, 270f, 90f);
    private Quaternion leftRotation = Quaternion.Euler(90f, 0f, 90f);

    #region Getters and Setters
    public string GetActorName()
    {
        return actorName;
    }

    public float GetSpeed()
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

    public void SetDirectionMemory(Vector3 d)
    {
        directionMemory = d;
        if (Mathf.Abs(d.x) > Mathf.Abs(d.z))
        {
            if (d.x > 0)
                directionPointer.localRotation = rightRotation;
            else if (d.x < 0)
                directionPointer.localRotation = leftRotation;
        }
        else if (Mathf.Abs(d.z) > Mathf.Abs(d.x))
        {
            if (d.z > 0)
                directionPointer.localRotation = upRotation;
            else if (d.z < 0)
                directionPointer.localRotation = downRotation;
        }
    }

    #endregion

    #region External Functions

    public void CombatStatCalculator()
    {
        float[] coreStats = new float[6] { force, finesse, fortitude, intellect, intuition, inveiglement };
        //float[] temperamentStats = new float[6] { empathic, rational, untethered, steadfast, energized, peaceful };
        float[] instrumentStats = instrument.StatCalculator(coreStats);
        float[] weaponStats = weapon.StatCalculator(coreStats);
        attack = weaponStats[0] + force;
        piercing = weaponStats[1] + finesse;
        defense = weaponStats[2] + fortitude;
        magic = instrumentStats[0] + intellect;
        magicDefense = instrumentStats[1] + intuition;
        magicPiercing = instrumentStats[2] + inveiglement;

        maxHp = curHp = 15 * (weaponStats[2] + instrumentStats[1] + fortitude);
    }

    public void DamageText(string message)
    {
        TextMesh popup = Instantiate(popupText);
        popup.text = message;
        popup.color = UnityEngine.Random.ColorHSV();
        popup.transform.position = new Vector3(position.x, position.y + 1f, position.z);
        popup.GetComponent<S_PopupText>().TextRandomMover();
    }

    public void SmallText(string message, Color color)
    {
        TextMesh popup = Instantiate(popupText);
        popup.text = message;
        popup.color = color;
        popup.characterSize = 0.125f;
        popup.transform.position = new Vector3(position.x, position.y + 1f, position.z);
        popup.GetComponent<S_PopupText>().SmallTextStraightMover();
    }

    public void PopupSprite()
    {
        SpriteRenderer popup = Instantiate(popupSprite);
        popup.transform.position = new Vector3(position.x + 0.5f, position.y + 1f, position.z + 0.5f);
        popup.GetComponent<S_PopupSprite>().SpriteMover();
    }

    public virtual void BeatUpdate()
    {
        //ResetInputs();
    }

    public virtual void ResetInputs()
    {
        //if (preppedInput == KeyCode.None)
        //    ac.SetAnimation("Idle");
        //lastInput = preppedInput;
        //preppedInput = KeyCode.None;
    }

    public virtual void PrepForNewTurn()
    {
        animationController.SetAnimatorTrigger(S_Actions.i.idleHash);
    }

    public virtual void FinishTurn()
    {
        animationController.SetAnimatorTrigger(S_Actions.i.idleHash);
    }

    public void SetAnimatorBool(int param, bool b)
    {
        animationController.SetAnimatorBool(param, b);
    }

    public void SetAnimatorTrigger(int param)
    {
        animationController.SetAnimatorTrigger(param);
    }

    virtual public Sprite GetPortrait()
    {
        return animationController.GetPortrait();
    }

    virtual public Color GetPrimaryColor()
    {
        return animationController.GetPrimaryColor();
    }

    virtual public S_Weapon GetWeapon()
    {
        return weapon;
    }

    virtual public void ReceiveBeat()
    {
        return;
    }

    virtual public void GetKOd()
    {

    }

    public void StartMove()
    {
        StartCoroutine("Mover");
    }

    private IEnumerator Mover()
    {
        bool finished = false;
        float t = 0;
        while (!finished)
        {
            t = t < Mathf.PI ? t + 0.45f : Mathf.PI;

            Vector3 dest = battleRhythm.map.GridToWorldPosition(position);
            dest = new Vector3(dest.x + 0.5f, dest.y + 0.5f, dest.z + 0.5f);
            float xMovement = 0, zMovement = 0;
            if (dest.x != transform.position.x)
                xMovement = Math.Abs(dest.x - transform.position.x) > 0.1f ? (dest.x - transform.position.x) / 4 : dest.x - transform.position.x;
            else if (dest.z != transform.position.z)
                zMovement = Math.Abs(dest.z - transform.position.z) > 0.1f ? (dest.z - transform.position.z) / 4 : dest.z - transform.position.z;
            else
                finished = true && Mathf.Approximately(t, Mathf.PI);

            transform.position = new Vector3(transform.position.x + xMovement, 0.5f + (0.25f * Mathf.Sin(t)), transform.position.z + zMovement);
            yield return new WaitForEndOfFrame();
        }
        StopCoroutine("Mover");
    }

    virtual public void AnimationUpdate()
    {
        return;
    }

    #endregion

    #region Helper Functions

    protected int GetManhattanDistance(S_Actor other)
    {
        return (int) (Math.Abs(position.x - other.position.x) +
                      Math.Abs(position.y - other.position.y) +
                      Math.Abs(position.z - other.position.z));
    }

    protected Vector3 GetShortestCardinalDistance(S_Actor other)
    {
        float x = position.x - other.position.x;
        float z = position.z - other.position.z;
        return x >= z ? new Vector3(Mathf.Sign(x), 0, 0) : new Vector3(0, 0, Mathf.Sign(z));
    }

    //protected int GetShortestCardinalDistanceDir(S_Actor other)
    //{
    //    float x = position.x - other.position.x;
    //    float z = position.z - other.position.z;
    //    return Mathf.Abs(x) <= Mathf.Abs(z) ? x > 0;
    //}

    #endregion
}
