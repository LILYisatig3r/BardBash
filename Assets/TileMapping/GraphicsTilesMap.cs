using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class GraphicsTilesMap : MonoBehaviour {

    #region Members and Constructor
    [Header("Graphical Options")]
    [SerializeField] private int sizeX;
    [SerializeField] private int sizeZ;
    [SerializeField] private float tileSize;
    [SerializeField] private Texture2D terrainTiles;
    [SerializeField] private int tileRes;

    [Header("Map Options")]

    [Tooltip("The map will be generated with a number of rooms less than or equal to this")]
    [SerializeField] private int maxRoomCount;

    [Tooltip("The map will be generated with a ratio of roomsArea/totalArea less than or equal to this")]
    [SerializeField] private float maxMapDensity;

    [Tooltip("Minimum room dimensions including walls. Should not be set lower than 3")]
    [SerializeField] private int minRoomSize;

    [Tooltip("Maximum room dimensions. Higher dimensions will make it much less likely that many rooms will "
        + "fit in the map")]
    [SerializeField] private int maxRoomSize;

    private MeshFilter mf;
    private MeshRenderer mr;
    private MeshCollider mc;

    private DataTilesMap data;

	void Start () {
        maxRoomCount = maxRoomCount < 0 ? 2147483647 : maxRoomCount;
        maxMapDensity = maxMapDensity < 0 ? Mathf.Infinity : maxMapDensity;
        maxMapDensity = maxMapDensity > 1 ? 1 : maxMapDensity;
	}
    #endregion

    #region Monobehavior
    //void Update()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    RaycastHit rch;
    //    if (GetComponent<Collider>().Raycast(ray, out rch, 500f))
    //    {
    //        float x = Mathf.Floor(rch.point.x) / tileSize;
    //        float z = Mathf.Floor(rch.point.z) / tileSize;
    //    }
    //}
    #endregion

    #region Building Tools
    public Color[][] ExtractTiles()
    {
        int tileColumnCount = terrainTiles.width / tileRes;
        int tileRowCount = terrainTiles.height / tileRes;

        Color[][] tiles = new Color[tileColumnCount * tileRowCount][];

        for (int z = 0; z < tileRowCount; z++)
        {
            for (int x = 0; x < tileColumnCount; x++)
            {
                tiles[z * tileColumnCount + x] = terrainTiles.GetPixels(x * tileRes, z * tileRes, tileRes, tileRes);
            }
        }

        return tiles;
    }

    private void BuildTexture()
    {
        Color[][] tiles = ExtractTiles();

        int textureWidth = sizeX * tileRes;
        int textureHeight = sizeZ * tileRes;
        Texture2D texture = new Texture2D(textureWidth, textureHeight);

        for (int i = 0; i < sizeZ; i++)
        {
            for (int j = 0; j < sizeX; j++)
            {
                Color[] p = tiles[((int)data.GetTile(j, i).type * 4) + Random.Range(0,4)];
                texture.SetPixels(j*tileRes, i*tileRes, tileRes, tileRes, p);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        mr.sharedMaterials[0].color = Color.white;
        mr.sharedMaterials[0].mainTexture = texture;
    }

    public void BuildData()
    {
        data = new DataTilesMap(sizeX, sizeZ);
        int w = 0;
        int k = 0;
        float d = 0;
        int mapDimensions = sizeX * sizeZ;
        while (k < maxRoomCount && d < maxMapDensity && w < 5)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);
            int roomX = Random.Range(0, sizeX - roomWidth);
            int roomY = Random.Range(0, sizeZ - roomHeight);
            int finalRoomDimensions = data.MakeRoom(roomX, roomY, roomWidth, roomHeight, minRoomSize);
            if (finalRoomDimensions > 0)
            {
                k++;
                d += (float)finalRoomDimensions / (float)mapDimensions;
            }
            else
                w++;
        }
        data.BuildCorridors();
    }

    public void BuildMesh()
    {
        BuildData();

        int vsizeX = sizeX + 1;
        int vsizeZ = sizeZ + 1;
        int vertCount = vsizeX * vsizeZ;

        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uv = new Vector2[vertCount];

        int[] triangles = new int[(sizeX * sizeZ) * 2 * 3];

        int x, z;
        for (z = 0; z<vsizeZ; z++)
        {
            for (x = 0; x<vsizeX; x++)
            {
                int index = z * vsizeX + x;
                float y = 1;
                //DataTile.TileType thisType = DataTile.TileType.none;
                //DataTile.TileType[] neighbors = new DataTile.TileType[3];
                //neighbors[0] = neighbors[1] = neighbors[2] = DataTile.TileType.none;
                //if (x < vsizeX - 1 && z < vsizeZ - 1)
                //    thisType = data.tiles[x, z].type;
                //if (x > 0 && z < vsizeZ - 1)
                //    neighbors[0] = data.tiles[x - 1, z].type;
                //if (z > 0 && x < vsizeX - 1)
                //    neighbors[2] = data.tiles[x, z - 1].type;
                //if (x > 0 && z > 0)
                //    neighbors[1] = data.tiles[x - 1, z - 1].type;

                //if (thisType != DataTile.TileType.none)
                //{
                //    if (thisType == DataTile.TileType.grass || neighbors[0] == DataTile.TileType.grass
                //        || neighbors[1] == DataTile.TileType.grass || neighbors[2] == DataTile.TileType.grass)
                //        y = 1;
                //    else if (thisType == DataTile.TileType.water && neighbors[0] == DataTile.TileType.water
                //        && neighbors[1] == DataTile.TileType.water && neighbors[2] == DataTile.TileType.water)
                //        y = Random.Range(0f, 0.5f);
                //    else
                //        y = Random.Range(1.1f, 2f);
                //}

                vertices[index] = new Vector3(x * tileSize, y, z * tileSize);
                normals[index] = Vector3.up;
                uv[index] = new Vector2((float)x / sizeX, (float)z / sizeZ);
            }
        }

        for (z = 0; z < sizeZ; z++)
        {
            for (x = 0; x < sizeX; x++)
            {
                int triOffset = (z * sizeX + x) * 6;
                int vertOffset = z * vsizeX + x;

                triangles[triOffset + 0] = vertOffset;
                triangles[triOffset + 1] = vertOffset + vsizeX;
                triangles[triOffset + 2] = vertOffset + vsizeX + 1;

                triangles[triOffset + 3] = vertOffset;
                triangles[triOffset + 4] = vertOffset + vsizeX + 1;
                triangles[triOffset + 5] = vertOffset + 1;
            }
        }

        Mesh m = new Mesh();
        m.vertices = vertices;
        m.triangles = triangles;
        m.normals = normals;
        m.uv = uv;

        mf = GetComponent<MeshFilter>();
        mr = GetComponent<MeshRenderer>();
        mc = GetComponent<MeshCollider>();

        mf.mesh = m;
        mc.sharedMesh = m;
        BuildTexture();

    }
    #endregion

    #region Pubilc Methods
    public Vector3 GridToWorldPosition(Vector3 gridPos)
    {
        return gridPos;
    }

    public DataTile GetRandomWalkableTile()
    {
        List<Vector3> walkables = new List<Vector3>();
        for (int z = 0; z < sizeZ; z++)
            for (int x = 0; x < sizeX; x++)
                if (data.tiles[x, z].type == DataTile.TileType.grass && !data.tiles[x,z].occupant)
                    walkables.Add(new Vector3(x, 0f, z));
        Vector3 tilePosition = walkables[Random.Range(0, walkables.Count)];
        return GetTile((int)tilePosition.x, (int)tilePosition.y);
    }

    public Vector3 OccupyRandomWalkableTile(S_Actor a)
    {
        List<Vector3> walkables = new List<Vector3>();
        for (int z = 0; z < sizeZ; z++)
            for (int x = 0; x < sizeX; x++)
                if (data.tiles[x, z].type == DataTile.TileType.grass && !data.tiles[x, z].occupant)
                    walkables.Add(new Vector3(x, 0f, z));
        Vector3 tilePosition = walkables[Random.Range(0, walkables.Count)];
        GetTile((int)tilePosition.x, (int)tilePosition.y).occupant = a;
        return tilePosition;
    }

    public DataTile GetTile(int x, int y)
    {
        return data.GetTile(x, y);
    }

    #endregion
}
