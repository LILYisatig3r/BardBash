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

    [SerializeField] Koreographer koreographer;
    [SerializeField] string trackName;
    [SerializeField] string actorName;
    [SerializeField] float bpm;
    [SerializeField] int frames;

	void Start () {
        //Koreographer.Instance.RegisterForEvents(trackName, SyncAnimation);
        animationName = actorName + "Idle";

        animations = new Dictionary<KeyCode, string>();
        animations.Add(KeyCode.None, actorName + "Idle");
        animations.Add(KeyCode.UpArrow, actorName + "Idle");
        animations.Add(KeyCode.RightArrow, actorName + "Idle");
        animations.Add(KeyCode.DownArrow, actorName + "Idle");
        animations.Add(KeyCode.LeftArrow, actorName + "Idle");
        animations.Add(KeyCode.Q, actorName + "Casting");


        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        float bps = bpm / 60f;
        float fps = GetComponent<Animation>().clip.frameRate;
        float desiredFPS = bps * frames;
        float ratio = desiredFPS / fps;
        animator.speed = desiredFPS / fps;
	}

    void Update()
    {
        if (syncLock >= 2)
        {
            animator.Play(animationName);
            //Koreographer.Instance.UnregisterForAllEvents(this);
            animationName = actorName + "Idle";
            syncLock = 0;
        }
    }

    public void SetActor(string a)
    {
        actorName = a;
    }

    public void SetAnimation(string a)
    {
        animationName = actorName + a;
    }

    public void SyncUnlock()
    {
        syncLock += 1;
    }

    public void ReceiveAction(KeyCode a)
    {
        //pcAction = a;
        string nextAnimation;
        if (animations.TryGetValue(a, out nextAnimation))
            animationName = nextAnimation;
    }

}
