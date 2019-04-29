using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

public class S_AnimationController : MonoBehaviour {

    private SpriteRenderer sr;
    private Animator animator;

    //private KeyCode pcAction;
    private Dictionary<KeyCode, string> animations;
    private string animationName;
    private short syncLock = 0;

    [SerializeField] Sprite portrait;
    [SerializeField] Color primaryColor;

    [SerializeField] Koreographer koreographer;
    [SerializeField] string trackName;
    [SerializeField] string actorName;
    [SerializeField] public float bpm;
    [SerializeField] int frames;

	void Start () {
        S_Actor pc = GetComponentInParent<S_Actor>();
        animationName = actorName + "Idle";

        animations = new Dictionary<KeyCode, string>();
        animations.Add(KeyCode.None, actorName + "Idle");
        animations.Add(KeyCode.UpArrow, actorName + "Idle");
        animations.Add(KeyCode.RightArrow, actorName + "Idle");
        animations.Add(KeyCode.DownArrow, actorName + "Idle");
        animations.Add(KeyCode.LeftArrow, actorName + "Idle");
        string[] abilities = pc.GetAbilities();
        animations.Add(KeyCode.Q, actorName + abilities[0]);
        animations.Add(KeyCode.W, actorName + abilities[1]);
        animations.Add(KeyCode.E, actorName + abilities[2]);
        animations.Add(KeyCode.R, actorName + abilities[3]);

        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        float bps = bpm / 60f;
        //float fps = GetComponent<Animation>().clip.frameRate;
        //float desiredFPS = bps * frames;
        //float ratio = desiredFPS / fps;
        //animator.speed = desiredFPS / fps;
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
            animator.SetTrigger("Melee");
    }

    public void SetActor(string a)
    {
        actorName = a;
    }

    public void SetAnimatorBool(int param, bool b)
    {
        animator.SetBool(param, b);
    }

    public void SetAnimatorTrigger(int param)
    {
        animator.SetTrigger(param);
    }

    public Color GetPrimaryColor()
    {
        return primaryColor;
    }

    public Sprite GetPortrait()
    {
        return portrait;
    }
}
