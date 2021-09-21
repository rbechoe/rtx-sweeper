public class TheCreator : Base
{
    private static TheCreator _instance;

    public int xSize { get; set; }
    public int zSize { get; set; }
    public int gridSize { get; set; }
    public int bombAmount { get; set; }

    public static TheCreator Instance
    {
        get
        {
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            _instance = this;
        }
    }
}