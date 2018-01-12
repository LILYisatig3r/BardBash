using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public float dampTime = 0.15f;
    public Transform target;
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
    }
}
