using UnityEngine;

public class TheCreator : MonoBehaviour
{
    private static TheCreator instance;

    public int xSize { get; set; }
    public int zSize { get; set; }
    public int gridSize { get; set; }
    public int bombAmount { get; set; }

    public static TheCreator Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
    }
}