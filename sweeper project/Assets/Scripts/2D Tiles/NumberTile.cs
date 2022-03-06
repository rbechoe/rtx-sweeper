using UnityEngine;

namespace Tiles2D
{
    public class NumberTile : Tile
    {
        protected override void TypeSpecificAction()
        {
            EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
            gridMat.SetColor("_TextureColorTint", defaultCol);
            ShowBombAmount();
        }
    }
}
