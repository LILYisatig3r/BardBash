using System;
using System.Collections.Generic;
using UnityEngine;

public class DataTilesMap {

    #region Room Struct
    protected struct Room
    {
        public int left;
        public int right;
        public int top;
        public int bot;

        public int centerX { get { return (left + right) / 2; } }
        public int centerY { get { return (top + bot) / 2; } }

        public bool Collision (Room other)
        {
            if (right >= other.left - 1 &&
                left <= other.right + 1 &&
                bot <= other.top + 1 &&
                top >= other.bot - 1)
                return true;
            return false;
        }

        public bool DeCollide (Room other, Vector2 mapSize)
        {
            int fixer = UnityEngine.Random.Range(0, 4);
            int attempts = 0;
            while (attempts < 7)
            {
                switch (fixer)
                {
                    case 0: // right >= other.left
                        if (attempts < 3)
                        {
                            int diff = right - other.left + 2;
                            if (diff <= left)
                            {
                                left = Mathf.Max(left - diff, 0);
                                right -= diff;
                                return true;
                            }
                        }
                        else if (other.left > 3 && attempts >= 3)
                        {
                            if ((left + 4) <= other.left)
                                right = other.left - 2;
                            else
                            {
                                int diff = right - other.left + 2;
                                left = Mathf.Max(left - diff, 0);
                                right -= diff;
                            }
                            return true;
                        }
                        break;
                    case 1: // left <= other.right
                        if  (attempts < 3)
                        {
                            int diff = other.right - left + 2;
                            if (diff <= mapSize.x - right - 1)
                            {
                                left += diff;
                                right = Mathf.Min(right + diff, (int)mapSize.x - 1);
                                return true;
                            }
                        }
                        else if (other.right + 3 < mapSize.x - 1 && attempts >= 3)
                        {
                            if ((right - 4) >= other.right)
                                left = other.right + 2;
                            else
                            {
                                int diff = other.right - left + 2;
                                left += diff;
                                right = Mathf.Min(right + diff, (int)mapSize.x - 1);
                            }
                            return true;
                        }
                        break;
                    case 2: // bot <= other.top
                        if (attempts < 3)
                        {
                            int diff = other.top - bot + 2;
                            if (diff <= mapSize.y - top - 1)
                            {
                                top = Mathf.Min(top + diff, (int)mapSize.y - 1);
                                bot += diff;
                                return true;
                            }
                        }
                        else if (other.top + 3 < mapSize.y - 1 && attempts >= 3)
                        {
                            if ((top - 4) >= other.top)
                                bot = other.top + 2;
                            else
                            {
                                int diff = other.top - bot + 2;
                                top = Mathf.Min(top + diff, (int)mapSize.y - 1);
                                bot += diff;
                            }
                            return true;
                        }
                        break;
                    case 3: // top >= other.bot
                        if (attempts < 3)
                        {
                            int diff = top - other.bot + 2;
                            if (diff <= bot)
                            {
                                top -= diff;
                                bot = Mathf.Max(bot - diff, 0);
                                return true;
                            }
                        }
                        else if (other.bot > 3 && attempts >= 3)
                        {
                            if ((bot + 4) <= other.bot)
                                top = other.bot - 2;
                            else
                            {
                                int diff = top - other.bot + 2;
                                top -= diff;
                                bot = Mathf.Max(bot - diff, 0);
                            }
                            return true;
                        }
                        break;
                }
                fixer = (fixer + 1) % 4;
                attempts++;
            }
            return false;
        }
    }
    #endregion

    #region Members and Constructor
    private int size_x;
    private int size_y;

    int[,] AS_weights;
    public DataTile[,] tiles { get; set; }
    public Dictionary<Vector2, S_Actor> occupancies;
    List<Room> rooms;

    public DataTilesMap (int w, int h)
    {
        size_x = w;
        size_y = h;
        tiles = new DataTile[w,h];
        AS_weights = new int[w, h];
        for (int H = 0; H < h; H++)
            for (int W = 0; W < w; W++)
            {
                tiles[W, H] = new DataTile();
                AS_weights[W, H] = 2;
            }
        rooms = new List<Room>();
    }
    #endregion

    #region Public Methods
    public DataTile GetTile(int x, int y)
    {
        return tiles[x, y];
    }

    public int MakeRoom(int x, int y, int width, int height, int minSize)
    {
        List<Room> collisions = new List<Room>(rooms.Count);
        Room r = new Room();
        r.left = x;
        r.right = x + width - 1;
        r.bot = y;
        r.top = y + height - 1;
        foreach (Room room in rooms)
            if (r.Collision(room))
                collisions.Add(room);
        int attempts = 0;
        while (collisions.Count > 0)
        {
            Vector2 mapSize = new Vector2(size_x, size_y);
            foreach (Room room in collisions)
                if (r.Collision(room))
                    if (!r.DeCollide(room, mapSize))
                        return -1;

            collisions = new List<Room>(rooms.Count);
            foreach (Room room in rooms)
                if (r.Collision(room))
                    collisions.Add(room);
            attempts++;
            if (attempts >= 10)
                return -1;
        }
        x = r.left;
        width = r.right + 1 - x;
        y = r.bot;
        height = r.top + 1 - y;

        if (width < minSize || height < minSize)
            return -1;
        //width = width < 3 ? 3 : width;
        //height = height < 3 ? 3 : height;

        for (int TnB = 0; TnB < width; TnB++)
        {
            tiles[TnB + x, y].type = tiles[TnB + x, y + height - 1].type = DataTile.TileType.stone;
            AS_weights[TnB + x, y] = AS_weights[TnB + x, y + height - 1] = 5;
        }

        for (int LnR = 0; LnR < height - 1; LnR++)
        {
            tiles[x, LnR + y].type = tiles[x + width - 1, LnR + y].type = DataTile.TileType.stone;
            AS_weights[x, LnR + y] = AS_weights[x + width - 1, LnR + y] = 5;
        }

        for (int Y = 1; Y < height - 1; Y++)
            for (int X = 1; X < width - 1; X++)
            {
                tiles[X + x, Y + y].type = DataTile.TileType.grass;
                AS_weights[X + x, Y + y] = 1;
            }

        rooms.Add(r);
        return width * height;
    }

    public void BuildCorridors()
    {
        foreach (Room room in rooms)
        {
            int[] corridorChances = { 1, 1, 2, 2, 2, 3, 3, 4 };
            int corridors = corridorChances[UnityEngine.Random.Range(0,8)];
            corridors = Math.Min(corridors, rooms.Count - 1);
            SortedList<float, Room> potentialPaths = new SortedList<float, Room>(new DuplicateKeyComparer<float>());
            foreach (Room goal in rooms)
            {
                float potentialPath = MD(room.centerX, room.centerY, goal.centerX, goal.centerY);
                if (potentialPath != 0)
                    potentialPaths.Add(potentialPath, goal);
            }

            for (int c = 0; c < corridors; c++)
            {
                List<AS_Node> path = A_Star(room, potentialPaths.Values[c]);
                foreach (AS_Node node in path)
                {
                    tiles[node.x, node.y].type = DataTile.TileType.grass;
                    AS_weights[node.x, node.y] = 1;
                    for (int n = 0; n < 4; n++)
                    {
                        if (node.x > 0 && tiles[node.x - 1, node.y].type == DataTile.TileType.water)
                            Stoneify(node.x - 1, node.y);
                        if (node.x < size_x - 1 && tiles[node.x + 1, node.y].type == DataTile.TileType.water)
                            Stoneify(node.x + 1, node.y);
                        if (node.y > 0 && tiles[node.x, node.y - 1].type == DataTile.TileType.water)
                            Stoneify(node.x, node.y - 1);
                        if (node.y < size_y - 1 && tiles[node.x, node.y + 1].type == DataTile.TileType.water)
                            Stoneify(node.x, node.y + 1);

                        if (node.y < size_y - 1 && node.x > 0 && tiles[node.x - 1, node.y + 1].type == DataTile.TileType.water)
                            Stoneify(node.x - 1, node.y + 1);
                        if (node.y < size_y - 1 && node.x < size_x - 1 && tiles[node.x + 1, node.y + 1].type == DataTile.TileType.water)
                            Stoneify(node.x + 1, node.y + 1);
                        if (node.y > 0 && node.x > 0 && tiles[node.x - 1, node.y - 1].type == DataTile.TileType.water)
                            Stoneify(node.x - 1, node.y - 1);
                        if (node.y < size_y - 1 && node.x < size_x - 1 && tiles[node.x + 1, node.y - 1].type == DataTile.TileType.water)
                            Stoneify(node.x + 1, node.y - 1);
                    }
                }
            }
        }
    }

    private void Stoneify(int x, int y)
    {
        tiles[x, y].type = DataTile.TileType.stone;
        AS_weights[x, y] = 5;
    }

    public Stack<Vector3> FindPath(Vector3 a, Vector3 b, List<DataTile.TileType> impassables)
    {
        List<AS_Node> path = A_Star(a, b, impassables);
        if (path == null)
            return null;

        Stack<Vector3> ret = new Stack<Vector3>(path.Count);
        foreach (AS_Node node in path)
        {
            ret.Push(new Vector3(node.x, 0, node.y));
        }
        return ret;
    }
    #endregion

    #region A*
    public class AS_Node
    {
        public AS_Node parent;
        public int x, y;
        public float f, g, h;

        public AS_Node(AS_Node p, int x, int y, float f, float g, float h)
        {
            parent = p;
            this.x = x;
            this.y = y;
            this.g = g;
            this.h = h;
            if (f == -1)
                this.f = g + h;
            else
                this.f = f;
        }
    }

    private List<AS_Node> A_Star(Room r1, Room r2)
    {
        int gx = r2.centerX;
        int gy = r2.centerY;
        if (gx == r1.centerX && gy == r1.centerY)
            return null;
        AS_Node start = new AS_Node(null, r1.centerX, r1.centerY, 0, 0, MD(r1.centerX, r1.centerY, gx, gy));
        PriorityQueueAS openList = new PriorityQueueAS(start);
        Dictionary<Vector2, int> closedList = new Dictionary<Vector2, int>();

        while (openList.Count() > 0)
        {
            AS_Node q = openList.Dequeue();

            AS_Node[] successors = new AS_Node[4];
            successors[0] = successors[1] = successors[2] = successors[3] = null;
            if (q.x - 1 >= 0)
                successors[0] = new AS_Node(q, q.x - 1, q.y, -1, q.g + 1 + AS_weights[q.x - 1, q.y], MD(q.x - 1, q.y, gx, gy));
            if (q.x + 1 < size_x)
                successors[1] = new AS_Node(q, q.x + 1, q.y, -1, q.g + 1 + AS_weights[q.x + 1, q.y], MD(q.x + 1, q.y, gx, gy));
            if (q.y - 1 >= 0)
                successors[2] = new AS_Node(q, q.x, q.y - 1, -1, q.g + 1 + AS_weights[q.x, q.y - 1], MD(q.x, q.y - 1, gx, gy));
            if (q.y + 1 < size_y)
                successors[3] = new AS_Node(q, q.x, q.y + 1, -1, q.g + 1 + AS_weights[q.x, q.y + 1], MD(q.x, q.y + 1, gx, gy));

            foreach (AS_Node successor in successors)
            {
                if (successor != null)
                {
                    if (successor.x == gx && successor.y == gy)
                        return Reconstruct(successor);

                    int cost = 0;
                    if (closedList.TryGetValue(new Vector2(successor.x, successor.y), out cost))
                    {
                        if (cost > successor.f)
                            cost = 0;
                    }

                    if (cost <= 0)
                        openList.Enqueue(successor);
                }
            }
            int closedNodeCost;
            Vector2 closedNodePos = new Vector2(q.x, q.y);
            if (!closedList.TryGetValue(closedNodePos, out closedNodeCost))
                closedList[closedNodePos] = (int)q.f;
            else
                if (q.f < closedNodeCost)
                    closedList[closedNodePos] = (int)q.f;
        }
        return null;
    }

    private List<AS_Node> A_Star(Vector3 a, Vector3 b, List<DataTile.TileType> impassables)
    {
        int gx = (int)b.x;
        int gy = (int)b.z;
        if (gx == (int)a.x && gy == (int)a.z)
            return null;

        AS_Node start = new AS_Node(null, (int)a.x, (int)a.z, 0, 0, 0);
        PriorityQueueAS openList = new PriorityQueueAS(start);
        Dictionary<Vector2, int> closedList = new Dictionary<Vector2, int>();

        while (openList.Count() > 0)
        {
            AS_Node q = openList.Dequeue();

            AS_Node[] successors = new AS_Node[4];
            successors[0] = successors[1] = successors[2] = successors[3] = null;
            if (q.x - 1 >= 0)
                successors[0] = new AS_Node(q, q.x - 1, q.y, -1, q.g + 1 + AS_weights[q.x - 1, q.y], MD(q.x - 1, q.y, gx, gy));
            if (q.x + 1 < size_x)
                successors[1] = new AS_Node(q, q.x + 1, q.y, -1, q.g + 1 + AS_weights[q.x + 1, q.y], MD(q.x + 1, q.y, gx, gy));
            if (q.y - 1 >= 0)
                successors[2] = new AS_Node(q, q.x, q.y - 1, -1, q.g + 1 + AS_weights[q.x, q.y - 1], MD(q.x, q.y - 1, gx, gy));
            if (q.y + 1 < size_y)
                successors[3] = new AS_Node(q, q.x, q.y + 1, -1, q.g + 1 + AS_weights[q.x, q.y + 1], MD(q.x, q.y + 1, gx, gy));

            foreach (AS_Node successor in successors)
            {
                if (successor != null)
                {
                    if (successor.x == gx && successor.y == gy)
                        return Reconstruct(successor);

                    int cost = 0;
                    if (closedList.TryGetValue(new Vector2(successor.x, successor.y), out cost))
                    {
                        if (cost > successor.f)
                            cost = 0;
                    }

                    DataTile tile = tiles[successor.x, successor.y];
                    if (cost <= 0 && !impassables.Contains(tile.type) && tile.occupant == null)
                        openList.Enqueue(successor);
                }
            }
            int closedNodeCost;
            Vector2 closedNodePos = new Vector2(q.x, q.y);
            if (!closedList.TryGetValue(closedNodePos, out closedNodeCost))
                closedList[closedNodePos] = (int)q.f;
            else
                if (q.f < closedNodeCost)
                closedList[closedNodePos] = (int)q.f;
        }
        return null;
    }

    private List<AS_Node> Reconstruct(AS_Node goal)
    {
        AS_Node iterator = goal;
        List<AS_Node> ret = new List<AS_Node>();
        ret.Add(goal);
        while (iterator.parent != null)
        {
            ret.Add(iterator.parent);
            iterator = iterator.parent;
        }
        return ret;
    }

    private float MD(float x1, float y1, float x2, float y2)
    {
        return Mathf.Abs(x2 - x1) + Mathf.Abs(y2 - y1);
    }
    #endregion
}

public class PriorityQueueAS
{
    DataTilesMap.AS_Node[] items;
    Dictionary<Vector2, int> positions;
    int queueCount;

    public PriorityQueueAS(DataTilesMap.AS_Node head)
    {
        items = new DataTilesMap.AS_Node[100];
        items[0] = null;
        items[1] = head;
        queueCount = 1;

        positions = new Dictionary<Vector2, int>();
        positions.Add(new Vector2(head.x, head.y), 1);
    }

    public void Enqueue(DataTilesMap.AS_Node item)
    {
        int position;
        if (!positions.TryGetValue(new Vector2(item.x, item.y), out position))
        {
            queueCount++;
            if (queueCount >= items.Length / 2)
                ResizeItems();
            items[queueCount] = item;
            positions.Add(new Vector2(item.x, item.y), queueCount);
            PercolateUp(queueCount);
        }
        else if (items[position].f > item.f)
        {
            items[position] = item;
            PercolateUp(position);
        }
    }

    public DataTilesMap.AS_Node Dequeue()
    {
        DataTilesMap.AS_Node ret = items[1];
        items[1] = items[queueCount];
        items[queueCount] = null;
        positions.Remove(new Vector2(ret.x, ret.y));
        if (queueCount > 1)
            positions[new Vector2(items[1].x, items[1].y)] = 1;
        PercolateDown();
        queueCount--;
        return ret;
    }

    public DataTilesMap.AS_Node Peek()
    {
        return items[1];
    }

    public int Count()
    {
        return queueCount;
    }

    private void PercolateUp(int index)
    {
        while (items[index/2] != null)
        {
            DataTilesMap.AS_Node child = items[index];
            DataTilesMap.AS_Node parent = items[index / 2];
            if (child.f < parent.f)
            {
                items[index] = parent;
                positions[new Vector2(parent.x, parent.y)] = index;
                items[index / 2] = child;
                positions[new Vector2(child.x, child.y)] = index / 2;
                index = index / 2;
            }
            else
                return;
        }
    }
    
    private void PercolateDown()
    {
        int index = 1;
        int replaceIndex = 1;
        while (index == replaceIndex)
        {
            DataTilesMap.AS_Node child_L = items[(index * 2)];
            DataTilesMap.AS_Node child_R = items[(index * 2) + 1];
            DataTilesMap.AS_Node parent = items[index];
            if (child_L != null && child_R != null)
                replaceIndex = child_L.f < child_R.f ? (index * 2) : (index * 2) + 1;
            else if (child_L != null)
                replaceIndex = index * 2;
            else
                return;
            DataTilesMap.AS_Node child = items[replaceIndex];
            if (child.f < parent.f)
            {
                items[replaceIndex] = parent;
                positions[new Vector2(parent.x, parent.y)] = replaceIndex;
                items[index] = child;
                positions[new Vector2(child.x, child.y)] = index;
                index = replaceIndex;
            }
        }
    }

    private void ResizeItems()
    {
        DataTilesMap.AS_Node[] newItems = new DataTilesMap.AS_Node[items.Length * 2];
        for (int i = 0; i < queueCount; i++)
            newItems[i] = items[i];
        items = newItems;
    }
}

/// <summary>
/// Comparer for comparing two keys, handling equality as beeing greater
/// Use this Comparer e.g. with SortedLists or SortedDictionaries, that don't allow duplicate keys
/// </summary>
/// <typeparam name="TKey"></typeparam>
public class DuplicateKeyComparer<TKey>
                :
             IComparer<TKey> where TKey : IComparable
{
    #region IComparer<TKey> Members

    public int Compare(TKey x, TKey y)
    {
        int result = x.CompareTo(y);

        if (result == 0)
            return 1;   // Handle equality as beeing greater
        else
            return result;
    }

    #endregion
}
