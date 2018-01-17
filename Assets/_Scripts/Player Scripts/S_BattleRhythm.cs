using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BattleRhythm : MonoBehaviour {

    public enum State
    {
        playing,
        transitioning,
        preview,
        finished
    }

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
        previewMeasures = 1f;
        playing = false;
	}

    public void AddMusician(GameObject m)
    {
        S_Actor musician = m.GetComponent<S_Actor>();
        //orchestra.AddLast(musician);
        LinkedListNode<S_Actor> iterator = orchestra.First;
        while (iterator != null && musician.GetSpeed() >= iterator.Value.GetSpeed())
            iterator = iterator.Next;

        if (iterator != null)
            orchestra.AddBefore(iterator, musician);
        else
            orchestra.AddLast(new LinkedListNode<S_Actor>(musician));

        activeMusician = orchestra.First.Value;
        remainingMeasures = activeMusician.GetMeasures();
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
                    orchestra.AddLast(orchestra.First.Value);
                    orchestra.RemoveFirst();

                    currentState = State.transitioning;
                    cameraController.target = null;
                    cameraController.targets = orchestra;
                    StartCoroutine("CameraTransition1");
                }
                break;

            case State.preview:
                if (remainingMeasures >= 0)
                {
                    remainingMeasures -= 0.25f;
                }

                if (remainingMeasures <= 0)
                {
                    remainingMeasures = activeMusician.GetMeasures();
                    currentState = State.transitioning;
                    cameraController.target = activeMusician.transform;
                    cameraController.targets = null;
                    StartCoroutine("CameraTransition2");
                }
                
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

    private IEnumerator CameraTransition1()
    {
        while (cameraController.moving)
            yield return new WaitForSeconds(0.1f);
        currentState = State.preview;
        cameraController.SetCameraSize(currentState);
        lastMusician.ResetAnimation();
    }

    private IEnumerator CameraTransition2()
    {
        while (cameraController.moving)
            yield return new WaitForSeconds(0.1f);
        currentState = State.playing;
        cameraController.SetCameraSize(currentState);
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
