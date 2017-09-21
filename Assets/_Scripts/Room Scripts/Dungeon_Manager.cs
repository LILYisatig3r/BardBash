using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon_Manager : MonoBehaviour
{

    public Room_Manager activeRoom;


    internal void DeactivateActiveRoom()
    {
        if (activeRoom != null)
            activeRoom.DeactivateRoom();
    }
}
