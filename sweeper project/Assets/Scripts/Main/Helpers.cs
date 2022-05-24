using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    // round to three decimals
    public static float RoundToThreeDecimals(float val)
    {
        return Mathf.Round(val * 1000.0f) / 1000.0f;
    }

    // check for nested childs to add to gameobject collection
    public static void NestedChildToGob<T>(Transform trans, List<GameObject> collection) where T : MonoBehaviour
    {
        foreach (Transform child in trans)
        {
            if (child.childCount > 0) NestedChildToGob<T>(child, collection);

            if (child.GetComponent<T>()) collection.Add(child.gameObject);
        }
    }

    // recursive parent lookup
    public static Transform FindInParent<T>(Transform trans)
    {
        if (trans.GetComponent<T>() != null)
        {
            return trans;
        }
        else if (trans.parent != null)
        {
            return FindInParent<T>(trans.parent);
        }
        else
        {
            return null;
        }
    }
}
