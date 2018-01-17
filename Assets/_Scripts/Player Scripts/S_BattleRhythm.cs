using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BattleRhythm : MonoBehaviour {

    public enum State
    {
        playing,
        transitioning
    }

    private State currentState;
    private bool playing;
    private LinkedList<S_Actor> orchestra;
    private int chair;
    private S_Actor lastMusician;
    private S_Actor activeMusician;
    private float remainingMeasures;
    private CameraController cameraController;

	void Start () {
        orchestra = new LinkedList<S_Actor>();
        cameraController = Camera.main.GetComponent<CameraController>();
        //currentState = State.playing;
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
            StartCoroutine("CameraTransition");
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
                    remainingMeasures = activeMusician.GetMeasures();
                    orchestra.AddLast(orchestra.First.Value);
                    orchestra.RemoveFirst();

                    currentState = State.transitioning;
                    cameraController.target = activeMusician.transform;
                    StartCoroutine("CameraTransition");
                }
                break;
        }
    }

    private IEnumerator CameraTransition()
    {
        while (cameraController.moving)
            yield return new WaitForSeconds(0.1f);
        currentState = State.playing;
        lastMusician.ResetAnimation();
    }

    public S_Actor GetActiveMusician()
    {
        return activeMusician;
    }
}
