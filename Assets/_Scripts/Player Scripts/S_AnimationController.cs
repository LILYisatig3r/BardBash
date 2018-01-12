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
    [SerializeField] float bpm;
    [SerializeField] int frames;

	void Start () {
        //Koreographer.Instance.RegisterForEvents(trackName, SyncAnimation);
        animationName = "a_Wiz1";

        animations = new Dictionary<KeyCode, string>();
        animations.Add(KeyCode.None, "a_Wiz1");
        animations.Add(KeyCode.UpArrow, "a_Wiz1");
        animations.Add(KeyCode.RightArrow, "a_Wiz1");
        animations.Add(KeyCode.DownArrow, "a_Wiz1");
        animations.Add(KeyCode.LeftArrow, "a_Wiz1");
        animations.Add(KeyCode.Q, "a_WizCasting");


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
            animationName = "a_Wiz1";
            syncLock = 0;
        }
    }

    public void SetAnimation(string a)
    {
        animationName = a;
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
