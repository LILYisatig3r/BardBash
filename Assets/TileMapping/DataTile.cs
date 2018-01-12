
public class DataTile {
    public enum tileType
    {
        grass = 0,
        water = 1,
        stone = 2,
        magic = 3
    }

    public tileType type { get; set; }
    public bool occupied { get; set; }
    //private bool fog;

    public DataTile()
    {
        type = tileType.water;
        occupied = false;
        //fog = false;
    }

}
