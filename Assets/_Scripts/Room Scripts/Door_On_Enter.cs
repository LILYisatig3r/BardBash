using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door_On_Enter : MonoBehaviour
{
    private Room_Manager manager;

    private void Awake()
    {
        manager = transform.parent.GetComponent<Room_Manager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            manager.ActivateRoom();
        }
    }
}
