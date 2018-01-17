using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_BattleRhythm : MonoBehaviour {

    public enum State
    {
        starting,
        playing,
        transitioning,
        preview,
        finished
    }

    [SerializeField] Transform banner;
    private Image bannerPortrait;
    private Text bannerName;
    private bool bannerExtended;

    public State currentState;
    private bool playing;
    private LinkedList<S_Actor> orchestra;
    private int chair;
    private S_Actor lastMusician;
    private S_Actor activeMusician;
    private float remainingMeasures;
    private float previewMeasures;
    private CameraController cameraController;

	void Start () {
        orchestra = new LinkedList<S_Actor>();
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.target = transform;
        previewMeasures = 1f;
        playing = false;

        currentState = State.starting;
        banner.transform.localPosition = new Vector3(-130f, 0f, 0f);
        bannerExtended = false;
        bannerPortrait = banner.transform.GetChild(0).GetComponent<Image>();
        bannerName = banner.transform.GetChild(1).GetComponent<Text>();
        StartCoroutine("StartSequence");
    }

    public void AddMusician(GameObject m)
    {
        S_Actor musician = m.GetComponent<S_Actor>();
        musician.SetClock(25 / musician.GetSpeed());
        LinkedListNode<S_Actor> iterator = orchestra.First;
        while (iterator != null && musician.GetClock() >= iterator.Value.GetClock())
            iterator = iterator.Next;

        if (iterator != null)
            orchestra.AddBefore(iterator, musician);
        else
            orchestra.AddLast(new LinkedListNode<S_Actor>(musician));

        activeMusician = orchestra.First.Value;
    }

    public void PopMusician()
    {
        S_Actor performer = orchestra.First.Value;
        float clockDecrement = performer.GetClock();
        performer.SetClock( 25 / performer.GetSpeed() );
        orchestra.RemoveFirst();
        foreach (S_Actor musician in orchestra)
            musician.SetClock(musician.GetClock() - clockDecrement);

        LinkedListNode<S_Actor> iterator = orchestra.First;
        while (iterator != null && performer.GetClock() >= iterator.Value.GetClock())
            iterator = iterator.Next;
        if (iterator != null)
            orchestra.AddBefore(iterator, performer);
        else
            orchestra.AddLast(new LinkedListNode<S_Actor>(performer));

        activeMusician = orchestra.First.Value;
    }

    public bool RemoveMusician(GameObject m)
    {
        S_Actor musician = m.GetComponent<S_Actor>();
        if (musician.Equals(activeMusician))
        {
            activeMusician = orchestra.First.Value;
            remainingMeasures = activeMusician.GetMeasures();
            currentState = State.transitioning;
            cameraController.target = activeMusician.transform;
            if (bannerExtended)
                Banner();
            StartCoroutine("CameraTransition1");
        }

        return orchestra.Remove(musician);
    }

    public void BeatUpdate()
    {
        switch (currentState)
        {
            case State.playing:

                if (remainingMeasures >= 0)
                {
                    float actionCost = activeMusician.GetActionCost();
                    remainingMeasures -= actionCost;
                }

                if (remainingMeasures <= 0)
                {
                    lastMusician = activeMusician;
                    activeMusician = orchestra.First.Value;
                    remainingMeasures = previewMeasures;
                    PopMusician();

                    currentState = State.transitioning;
                    cameraController.target = null;
                    cameraController.targets = orchestra;
                    StartCoroutine("CameraTransition1");
                    StartCoroutine("Banner");
                }
                break;

            case State.preview:
                if (remainingMeasures >= 0)
                {
                    remainingMeasures -= 0.25f;
                    Debug.Log(remainingMeasures);
                }

                if (remainingMeasures <= 0)
                {
                    remainingMeasures = activeMusician.GetMeasures();
                    currentState = State.transitioning;
                    cameraController.target = activeMusician.transform;
                    cameraController.targets = null;
                    StartCoroutine("CameraTransition2");
                    StartCoroutine("Banner");
                }

                break;

            case State.starting:

                break;

            case State.finished:
                activeMusician = orchestra.First.Value;
                activeMusician.ResetAnimation();
                cameraController.target = activeMusician.transform;
                cameraController.targets = null;
                cameraController.SetCameraSize(currentState);
                break;
        }
    }

    private IEnumerator StartSequence()
    {
        while (orchestra.Count == 0 || !Input.anyKey)
            yield return new WaitForEndOfFrame();

        remainingMeasures = 2;
        cameraController.target = null;
        cameraController.targets = orchestra;
        activeMusician = orchestra.First.Value;
        StartCoroutine("CameraTransition1");
        StartCoroutine("Banner");

    }

    private IEnumerator CameraTransition1()
    {
        while (cameraController.moving)
            yield return new WaitForSeconds(0.1f);
        currentState = State.preview;
        cameraController.SetCameraSize(currentState);
        if (lastMusician)
            lastMusician.ResetAnimation();
    }

    private IEnumerator CameraTransition2()
    {
        while (cameraController.moving)
            yield return new WaitForSeconds(0.1f);
        currentState = State.playing;
        cameraController.SetCameraSize(currentState);
    }

    private IEnumerator Banner()
    {
        if (!bannerExtended)
        {
            bannerExtended = !bannerExtended;
            bannerPortrait.sprite = activeMusician.GetPortrait();
            bannerName.color = activeMusician.GetPrimaryColor();
            bannerName.text = activeMusician.GetActorName();
            while (banner.transform.localPosition.x < 120f)
            {
                banner.transform.localPosition += new Vector3(15f, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            bannerExtended = !bannerExtended;
            while (banner.transform.localPosition.x > -130f)
            {
                banner.transform.localPosition += new Vector3(-10f, 0f, 0f);
                yield return new WaitForEndOfFrame();
            }
        }
    }

    public int GetMusicianCount()
    {
        return orchestra.Count;
    }

    public S_Actor GetActiveMusician()
    {
        return activeMusician;
    }
}
