
public class DataTile {
    public enum TileType
    {
        grass = 0,
        water = 1,
        stone = 2,
        magic = 3
    }

    public TileType type { get; set; }
    //public bool occupied { get; set; }
    public S_Actor occupant { get; set; }
    //private bool fog;

    public DataTile()
    {
        type = TileType.water;
        //occupied = false;
        occupant = null;
        //fog = false;
    }

}
