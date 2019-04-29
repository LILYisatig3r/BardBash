using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets._Scripts;

public partial class S_BattleRhythm
{
    private class CameraController
    {
        private Camera cam;
        private S_BattleRhythm br;
        private E_CameraState currentState;

        private bool transitioning;
        private float dampTime = 0.15f;
        private Vector3 velocity = Vector3.zero;

        public Transform target;

        public CameraController(S_BattleRhythm battleRhythm, Camera camera)
        {
            cam = camera;
            br = battleRhythm;
        }

        public void Update()
        {
            switch (currentState)
            {
                case E_CameraState.following:
                    Vector3 point = cam.WorldToScreenPoint(target.position);
                    Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
                    Vector3 destination = br.transform.position + delta;
                    Vector3 newPos = Vector3.SmoothDamp(br.transform.position, destination, ref velocity, dampTime);
                    Vector3 diff = newPos - br.transform.position;
                    br.transform.position = newPos;
                    if (transitioning)
                    {
                        if (diff.magnitude <= 0.01f)
                            br.CameraFinishedMovingToNewPlayer();
                    }
                    break;
                case E_CameraState.overview:
                    break;
                case E_CameraState.controllable:
                    break;
            }

        }

        public void ChangeState(E_CameraState newState)
        {
            //switch (currentState)
            //{
            //    case E_CameraState.following:
            //        target = null; break;
            //}
            currentState = newState;
        }

        public void ChangeTarget(Transform newTarget)
        {
            target = newTarget;
            transitioning = true;
        }

        public void GoToOverview()
        {

        }

        public void GoToControllable()
        {

        }

    }

}
