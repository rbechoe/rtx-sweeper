using UnityEngine;

public static class Helpers
{
    public static float RoundToThreeDecimals(float val)
    {
        return Mathf.Round(val * 1000.0f) / 1000.0f;
    }
}
