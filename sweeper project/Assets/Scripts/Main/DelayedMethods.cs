using System.Collections;
using UnityEngine;

public static class DelayedMethods
{
    public static IEnumerator FireMethod(System.Action method, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke();
    }
}

public static class DelayedMethods<T>
{
    public static IEnumerator FireMethod(System.Action<T> method, T arg, float delay = 0)
    {
        yield return new WaitForSeconds(delay);
        method.Invoke(arg);
    }
}
