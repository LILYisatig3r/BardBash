using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_On_Enter : MonoBehaviour
{
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            cam.GetComponent<CameraController>().target = transform.parent;
        }
    }
}
