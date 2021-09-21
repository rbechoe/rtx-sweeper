using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : Base
{
    [SerializeField]
    private TMP_InputField xTMP;
    [SerializeField]
    private TMP_InputField zTMP;
    [SerializeField]
    private TMP_InputField bombTMP;

    private int xSize;
    private int zSize;
    private int gridSize;
    private int bombAmount;

    public void Easy2D()
    {
        gridSize = 9;
        xSize = 9;
        zSize = 9;
        bombAmount = 10;
        NewGame2D();
    }

    public void Medium2D()
    {
        gridSize = 16;
        xSize = 16;
        zSize = 16;
        bombAmount = 40;
        NewGame2D();
    }

    public void Hard2D()
    {
        gridSize = 30;
        xSize = 30;
        zSize = 30;
        bombAmount = 130;
        NewGame2D();
    }

    public void Custom2D()
    {
        xSize = int.Parse(xTMP.GetComponent<TMP_InputField>().text);
        zSize = int.Parse(zTMP.GetComponent<TMP_InputField>().text);
        bombAmount = int.Parse(bombTMP.GetComponent<TMP_InputField>().text);
        NewGame2D();
    }

    public void Easy3D()
    {
        gridSize = 4;
        bombAmount = 10;
        NewGame3D();
    }

    public void Medium3D()
    {
        gridSize = 6;
        bombAmount = 48;
        NewGame3D();
    }

    public void Hard3D()
    {
        gridSize = 10;
        bombAmount = 250;
        NewGame3D();
    }

    private void NewGame2D()
    {
        TheCreator.Instance.xSize = xSize;
        TheCreator.Instance.zSize = zSize;
        TheCreator.Instance.gridSize = gridSize;
        TheCreator.Instance.bombAmount = bombAmount;
        SceneManager.LoadScene("Garden");
    }

    private void NewGame3D()
    {
        TheCreator.Instance.gridSize = gridSize;
        TheCreator.Instance.bombAmount = bombAmount;
        SceneManager.LoadScene("Universe");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}