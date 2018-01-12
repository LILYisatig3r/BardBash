
public class DataTile {
    public enum tileType
    {
        grass = 0,
        water = 1,
        stone = 2,
        magic = 3
    }

    public DataTile()
    {
        type = tileType.water;
        //fog = false;
    }

    public tileType type { get; set; }
    //private bool fog;
}
