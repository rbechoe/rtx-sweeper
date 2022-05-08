using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Settings")]
    public int gridSize;
    public int bombAmount = 10;

    private int goodTiles;

    public float timer { get; private set; }
    public bool gameActive { get; set; }

    private void OnEnable()
    {
        EventSystem.AddListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.AddListener(EventType.GAME_LOSE, StartGame);
    }

    private void OnDisable()
    {
        EventSystem.RemoveListener(EventType.PREPARE_GAME, StartGame);
        EventSystem.RemoveListener(EventType.GAME_LOSE, StartGame);
    }

    private void Update()
    {
        if (gameActive)
        {
            timer += Time.deltaTime;
        }
    }

    public void AddGoodTile3D()
    {
        goodTiles++;
        CheckForVictory3D();
    }

    private void CheckForVictory3D()
    {
        if (goodTiles == Mathf.Pow(gridSize, 3) - bombAmount)
        {
            EndGame();
            EventSystem.InvokeEvent(EventType.WIN_GAME);
        }
    }

    public void AddGoodTile2D()
    {
        goodTiles++;
        CheckForVictory2D();
    }

    private void CheckForVictory2D()
    {
        if (goodTiles == Mathf.Pow(gridSize, 2) - bombAmount)
        {
            EndGame();
            EventSystem.InvokeEvent(EventType.WIN_GAME);
        }
    }

    private void StartGame()
    {
        EventSystem.InvokeEvent(EventType.COUNT_BOMBS);
        EventSystem.InvokeEvent(EventType.PICK_TILE);
        EventSystem.InvokeEvent(EventType.START_GAME);
        EventSystem<int>.InvokeEvent(EventType.BOMB_UPDATE, bombAmount);
        timer = 0;
        goodTiles = 0;
        gameActive = true;
    }

    private void StopTimer()
    {
        gameActive = false;
    }

    public void EndGame()
    {
        gameActive = false;
        EventSystem.InvokeEvent(EventType.END_GAME);
    }
}