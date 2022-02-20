namespace Tiles2D
{
    public class BombTile : Tile
    {
        protected override void TypeSpecificAction()
        {
            EventSystem.InvokeEvent(EventType.GAME_LOSE);
        }

        protected override void StartSettings()
        {
            gameObject.tag = "Bomb";
            gameObject.layer = 11;
        }
    }
}
