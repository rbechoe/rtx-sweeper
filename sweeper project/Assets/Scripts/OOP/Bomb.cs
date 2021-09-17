public class Bomb : Tile
{
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "Bomb";
    }
}