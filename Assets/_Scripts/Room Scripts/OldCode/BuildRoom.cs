using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildRoom : MonoBehaviour
{
    public GameObject[] sidewalls;
    public GameObject[] topwalls;
    public GameObject[] bottomwalls;

    private WallInfo leftWall, rightWall; //topWall, bottomWall;

    public BuildRoomManager manager;

    public void BuildRandWalls()
    {
        GameObject go = Instantiate<GameObject>(sidewalls[Random.Range(0, sidewalls.Length - 1)]);
        go.transform.parent = transform;
        go.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rightWall = go.GetComponent<WallInfo>();

        go = Instantiate<GameObject>(sidewalls[Random.Range(0, sidewalls.Length - 1)]);
        go.transform.localScale = new Vector3(-1, 1, 1);
        go.transform.parent = transform;
        go.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rightWall = go.GetComponent<WallInfo>();
    }

    public void BuildWalls()
    {
        GameObject go;
        BuildRoom parent = manager.parents[this];

        // Build Left Wall
        if (parent.transform.position.y == transform.position.y)
        {
            if (parent.rightWall.topDoor && parent.rightWall.bottomDoor)
                go = Instantiate<GameObject>(sidewalls[2], transform.position, transform.rotation, transform);
            else if (parent.rightWall.topDoor)
                go = Instantiate<GameObject>(sidewalls[0], transform.position, transform.rotation, transform);
            else
                go = Instantiate<GameObject>(sidewalls[1], transform.position, transform.rotation, transform);
        }
        else if (parent.transform.position.y > transform.position.y)
        {
            go = Instantiate<GameObject>(sidewalls[0], transform.position, transform.rotation, transform);
        }
        else
        {
            go = Instantiate<GameObject>(sidewalls[1], transform.position, transform.rotation, transform);
        }
        leftWall = go.GetComponent<WallInfo>();

        // Build Right Wall
        go = Instantiate<GameObject>(sidewalls[Random.Range(0, sidewalls.Length-1)]);
        go.transform.localScale = new Vector3(-1, 1, 1);
        go.transform.parent = transform;
        go.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rightWall = go.GetComponent<WallInfo>();
    }

    public void BuildNeighbors()
    {
        GameObject go;
        List<BuildRoom> roomsBuilt = new List<BuildRoom>();

        float right = transform.position.x + BuildRoomManager.ROOM_WIDTH;
        float down = transform.position.y - BuildRoomManager.ROOM_HEIGHT;
        float up = transform.position.y + BuildRoomManager.ROOM_HEIGHT;

        if (rightWall.topDoor && rightWall.bottomDoor && manager.numOfRooms > 1)
        {
            if (Random.Range(0, 100) < 50) { 
                go = manager.BuildRoom(this, right, up, transform.position.z);
                roomsBuilt.Add(go.GetComponent<BuildRoom>());
                
                go = manager.BuildRoom(this, right, down, transform.position.z);
                roomsBuilt.Add(go.GetComponent<BuildRoom>());
            }
            else
            {
                go = manager.BuildRoom(this, right, transform.position.y, transform.position.z);
                roomsBuilt.Add(go.GetComponent<BuildRoom>());
            }
        }
        else if (rightWall.topDoor && rightWall.bottomDoor)
        {
            go = manager.BuildRoom(this, right, transform.position.y, transform.position.z);
            roomsBuilt.Add(go.GetComponent<BuildRoom>());
        }
        else if (rightWall.topDoor)
        {
            if (Random.Range(0, 1) == 0)
            {
                go = manager.BuildRoom(this, transform.position.x + BuildRoomManager.ROOM_WIDTH, transform.position.y + BuildRoomManager.ROOM_HEIGHT, transform.position.z);
            }
            else
            {
                go = manager.BuildRoom(this, transform.position.x + BuildRoomManager.ROOM_WIDTH, transform.position.y, transform.position.z);
            }
            roomsBuilt.Add(go.GetComponent<BuildRoom>());
        }
        else if (rightWall.bottomDoor)
        {
            if (Random.Range(0, 1) == 0)
                go = manager.BuildRoom(this, transform.position.x + BuildRoomManager.ROOM_WIDTH, transform.position.y, transform.position.z);
            else
                go = manager.BuildRoom(this, transform.position.x + BuildRoomManager.ROOM_WIDTH, transform.position.y + BuildRoomManager.ROOM_HEIGHT, transform.position.z);
            roomsBuilt.Add(go.GetComponent<BuildRoom>());
        }

        foreach(BuildRoom br in roomsBuilt)
        {
            br.BuildWalls();
            br.BuildNeighbors();
        }
    }

}
