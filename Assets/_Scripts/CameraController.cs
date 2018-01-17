using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float dampTime = 0.15f;
    public Transform target;
    public LinkedList<S_Actor> targets;
    public bool moving;

    private Vector3 velocity = Vector3.zero;
    private Camera cam;

    void Start () {
        cam = GetComponent<Camera>();
	}

	private void Update () {
        if (target)
        {
            Vector3 point = cam.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
            Vector3 destination = transform.position + delta;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            Vector3 diff = newPos - transform.position;
            moving = diff.magnitude >= 0.01 ? true : false;
            transform.position = newPos;
        }
        else if (targets != null)
        {
            if (targets.Count == 1)
                Debug.Log("stop here");
            Vector3 average = new Vector3();
            foreach (S_Actor actor in targets)
                average += actor.transform.position;

            average /= targets.Count;
            Vector3 point = cam.WorldToViewportPoint(average);
            Vector3 delta = average - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
            Vector3 diff = newPos - transform.position;
            moving = diff.magnitude >= 0.1 ? true : false;
            transform.position = newPos;
        }
    }

    public void SetCameraSize(S_BattleRhythm.State state)
    {
        if (state == S_BattleRhythm.State.preview)
        {
            float minx = Mathf.Infinity, maxx = 0, minz = Mathf.Infinity, maxz = 0;
            foreach (S_Actor actor in targets)
            {
                Vector3 ap = actor.position;
                minx = Mathf.Min(minx, ap.x);
                maxx = Mathf.Max(maxx, ap.x);
                minz = Mathf.Min(minz, ap.z);
                maxz = Mathf.Max(maxz, ap.z);
            }
            cam.orthographicSize = Mathf.Max((maxx - minx) * 1.41f, maxz - minz) / 3f;
        }
        else if (state == S_BattleRhythm.State.playing)
            cam.orthographicSize = 3;
        else if (state == S_BattleRhythm.State.finished)
            cam.orthographicSize = 5;
    }
}
