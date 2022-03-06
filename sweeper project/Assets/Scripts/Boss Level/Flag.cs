using UnityEngine;

namespace BossTiles
{
    public class Flag : MonoBehaviour
    {
        private void OnMouseOver()
        {
            if (Input.GetMouseButtonUp(1))
            {
                EventSystem<GameObject>.InvokeEvent(EventType.REMOVE_FLAG, gameObject);
            }
        }
    }
}
