public class Bomb3D : Tile3D
{
    protected override void Start()
    {
        base.Start();
        gameObject.tag = "Bomb";
        gameObject.layer = 11;
    }
}