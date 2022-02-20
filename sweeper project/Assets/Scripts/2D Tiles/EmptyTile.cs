using UnityEngine;

namespace Tiles2D
{
    public class EmptyTile : Tile
    {
        protected override void TypeSpecificAction()
        {
            EventSystem<GameObject>.InvokeEvent(EventType.ADD_GOOD_TILE, gameObject);
            Collider[] tiles = Physics.OverlapBox(gameObject.transform.position, new Vector3(1, 1, 1) * 1.25f, Quaternion.identity);
            for (int i = 0; i < tiles.Length; i++)
            {
                tiles[i].GetComponent<Tile>()?.NoBombReveal();
            }
            gridMat.SetColor("_TextureColorTint", new Color(0.25f, 0.5f, 0.5f, 0));
            bombCount = 8;
        }
    }
}
