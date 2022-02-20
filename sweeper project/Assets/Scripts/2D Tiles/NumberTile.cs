using UnityEngine;

namespace Tiles2D
{
    public class NumberTile : Tile
    {
        protected override void TypeSpecificAction()
        {
            EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
            defaultCol = Color.grey;
            gridMat.SetColor("_TextureColorTint", defaultCol);
            ShowBombAmount();
        }
    }
}
