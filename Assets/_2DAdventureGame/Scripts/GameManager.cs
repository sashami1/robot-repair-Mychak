using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public PlayerController player;
    public UIHandler uiHandler;

    private EnemyController[] enemies;
    private bool isGameEnded = false;

    void Start()
    {
        enemies = FindObjectsByType<EnemyController>(FindObjectsSortMode.None);
    }

    void Update()
    {
        if (isGameEnded) return;

        // Поразка: здоров'я гравця дорівнює 0
        if (player != null && player.health <= 0)
        {
            isGameEnded = true;
            if (uiHandler != null) uiHandler.DisplayLoseScreen();
            Invoke(nameof(ReloadScene), 3f);
        }
        // Перемога: усі роботи на карті відремонтовані
        else if (AllEnemiesFixed())
        {
            isGameEnded = true;
            if (uiHandler != null) uiHandler.DisplayWinScreen();
            Invoke(nameof(ReloadScene), 3f);
        }
    }

    bool AllEnemiesFixed()
    {
        if (enemies == null || enemies.Length == 0) return false;

        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && enemy.isBroken) return false;
        }
        return true;
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}