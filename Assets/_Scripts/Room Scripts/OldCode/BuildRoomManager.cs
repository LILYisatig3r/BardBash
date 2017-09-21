using System.Collections.Generic;
using UnityEngine;

public class BuildRoomManager : MonoBehaviour
{
    public GameObject[] roomPrefabs;
    public GameObject startRoomGameObject;
    public int numOfRooms;

    public const float ROOM_WIDTH = 18.5f;
    public const float ROOM_HEIGHT = 5f;

    public Dictionary<BuildRoom, BuildRoom> parents;

    private BuildRoom startRoom;

    private void Awake()
    {
        parents = new Dictionary<BuildRoom, BuildRoom>();
        startRoom = startRoomGameObject.GetComponent<BuildRoom>();
    }

    private void Start()
    {
        startRoom.manager = this;
        startRoom.BuildRandWalls();
        startRoom.BuildNeighbors();
    }

    public GameObject BuildRoom(BuildRoom parent, float x, float y, float z)
    {
        GameObject go = null;
        if (numOfRooms > 0)
        {
            go = Instantiate<GameObject>(roomPrefabs[(int)Random.Range(0, roomPrefabs.Length)]);
            go.transform.parent = transform;
            go.transform.position = new Vector3(x, y, z);

            BuildRoom br = go.GetComponent<BuildRoom>();
            br.manager = this;
            parents.Add(br, parent);


            numOfRooms--;
        }
        return go;
    }
}
