using benjohnson;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("Components")]
    public TurnManager turnManager;

    [HideInInspector] public int stage;

    // Variables
    float timeSinceGameStarted;

    void Start()
    {
        PlayerStats.instance.ResetStats();

        // Load player
        SceneManager.instance.LoadScene(2);

        stage = -1;
        LoadNextStage();

        timeSinceGameStarted = 0;
    }

    void Update()
    {
        timeSinceGameStarted += Time.deltaTime;
    }

    public void DungeonLoaded()
    {
        // Start game
        turnManager.StartTurnManager();
    }

    public void LoadShop()
    {
        // Stop the game
        turnManager.StopTurnManager();

        // Unload dungeon
        SceneManager.instance.UnloadScene(3);

        // Load shop
        SceneManager.instance.LoadScene(4);
    }

    public void LoadNextStage()
    {
        stage++;
        PlayerStats.instance.stage = stage + 1;

        // Unload shop
        SceneManager.instance.UnloadScene(4);

        // Load dungeon
        SceneManager.instance.LoadScene(3);
    }

    public void LoadWinScreen()
    {
        EndTheGame(true);
    }

    public void PlayerKilled()
    {
        EndTheGame(false);
    }

    private void EndTheGame(bool win)
    {
        PlayerStats.instance.win = win;
        PlayerStats.instance.time = (int)timeSinceGameStarted / 60;

        // Stop the game
        turnManager.StopTurnManager();

        // Unload dungeon
        SceneManager.instance.UnloadScene(3);
        // Unload shop
        SceneManager.instance.UnloadScene(4);
        // Unload player
        SceneManager.instance.UnloadScene(2);
        // Unload game
        SceneManager.instance.UnloadScene(1);

        // Load end screen
        SceneManager.instance.LoadScene(5);
    }
}
