using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int maxLevel;

    public Maze mazePrefab;

    public Player playerPrefab;

    public Enemy enemyPrefab;

    public Pooler bulletPool;

    public MazeRoomSettings roomSettings;

    public Text enemiesLeftTextP1, enemiesLeftTextP2, winText, gameOverText;

    public Image playerOneHB, playerTwoHB;

    public RawImage playerOneHBOutline, playerTwoHBOutline;

    private Maze mazeInstance;

    private Player playerOneInstance, playerTwoInstance;

    private int level = 1;

    private int numEnemies;

    private List<Enemy> enemies = new List<Enemy>();
    private List<IntVector2> enemyStartCoordinates = new List<IntVector2>();

    private void Start()
    {
        StartCoroutine(BeginGame());
    }

    private void Update()
    {
        if (Camera.main.orthographic)
        {
            if (playerOneInstance.health <= 0)
            {
                playerOneHB.gameObject.SetActive(false);
                playerOneHBOutline.gameObject.SetActive(false);
                enemiesLeftTextP1.gameObject.SetActive(false);
                if (playerTwoInstance.health <= 0) gameOverText.gameObject.SetActive(true);
            }
            if (playerTwoInstance.health <= 0)
            {
                playerTwoHB.gameObject.SetActive(false);
                playerTwoHBOutline.gameObject.SetActive(false);
                enemiesLeftTextP2.gameObject.SetActive(false);
                if (playerOneInstance.health <= 0) gameOverText.gameObject.SetActive(true);
            }
            playerOneHB.transform.localScale = new Vector3((float)(playerOneInstance.health) / 10f, playerOneHB.transform.localScale.y, playerOneHB.transform.localScale.z);
            playerTwoHB.transform.localScale = new Vector3((float)(playerTwoInstance.health) / 10f, playerTwoHB.transform.localScale.y, playerTwoHB.transform.localScale.z);
            int enemiesLeft = 0;
            bool allEnemiesDefeated = true;
            foreach (Enemy enemy in enemies)
            {
                if (enemy.isActiveAndEnabled)
                {
                    enemiesLeft++;
                    allEnemiesDefeated = false;
                }
            }
            enemiesLeftTextP1.text = "Enemies Left: " + enemiesLeft;
            enemiesLeftTextP2.text = "Enemies Left: " + enemiesLeft;
            if (allEnemiesDefeated) GoToNextLevel();
        }
    }

    private IEnumerator BeginGame()
    {
        Camera.main.clearFlags = CameraClearFlags.Skybox;
        enemiesLeftTextP1.gameObject.SetActive(false);
        enemiesLeftTextP2.gameObject.SetActive(false);
        playerOneHB.gameObject.SetActive(false);
        playerOneHBOutline.gameObject.SetActive(false);
        playerTwoHB.gameObject.SetActive(false);
        playerTwoHBOutline.gameObject.SetActive(false);

        mazeInstance = Instantiate(mazePrefab) as Maze;
        mazeInstance.size.x = 8 * level;
        mazeInstance.size.z = 8 * level;
        MazeRoomSettings[] mazeSettings = new MazeRoomSettings[level + 1];
        for (int i = 0; i < mazeSettings.Length; i++) mazeSettings[i] = roomSettings;
        mazeInstance.roomSettings = mazeSettings;

        Camera.main.orthographic = false;
        Camera.main.rect = new Rect(0f, 0f, 1f, 1f);
        Camera.main.transform.position = new Vector3(0f, 8 * level, 0f);

        yield return StartCoroutine(mazeInstance.Generate());
        numEnemies = mazeInstance.GetRooms().Count;
        bulletPool.createPool(numEnemies);

        playerOneInstance = Instantiate(playerPrefab) as Player;
        IntVector2 playerOneCoordinates = mazeInstance.RandomCoordinates;
        playerOneInstance.SetLocation(mazeInstance.GetCell(playerOneCoordinates));
        playerOneInstance.playerNumber = 1;
        Camera playerOneCamera = playerOneInstance.GetComponentInChildren<Camera>();
        playerOneCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
        playerOneCamera.ResetProjectionMatrix();

        playerTwoInstance = Instantiate(playerPrefab) as Player;
        IntVector2 playerTwoCoordinates = mazeInstance.RandomCoordinates;
        while (playerTwoCoordinates == playerOneCoordinates) playerTwoCoordinates = mazeInstance.RandomCoordinates;
        playerTwoInstance.SetLocation(mazeInstance.GetCell(playerTwoCoordinates));
        playerTwoInstance.playerNumber = 2;
        playerTwoInstance.gameObject.GetComponentInChildren<AudioListener>().enabled = false;
        Camera playerTwoCamera = playerTwoInstance.GetComponentInChildren<Camera>();
        playerTwoCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
        playerTwoCamera.ResetProjectionMatrix();

        for (int i = 0; i < numEnemies; i++)
        {
            IntVector2 enemyCoordinates = mazeInstance.RandomCoordinates;
            while (enemyCoordinates == playerOneCoordinates || enemyCoordinates == playerTwoCoordinates || enemyStartCoordinates.Contains(enemyCoordinates))
                enemyCoordinates = mazeInstance.RandomCoordinates;
            enemyStartCoordinates.Add(enemyCoordinates);
            Enemy newEnemy = Instantiate(enemyPrefab as Enemy);
            enemies.Add(newEnemy);
            newEnemy.SetLocation(mazeInstance.GetCell(enemyCoordinates));
            newEnemy.bulletPool = bulletPool;
        }

        Camera.main.clearFlags = CameraClearFlags.Depth;
        Camera.main.rect = new Rect(0.375f, 0.375f, 0.25f, 0.25f);
        Camera.main.orthographic = true;
        Camera.main.orthographicSize = mazeInstance.size.x / 2;
        enemiesLeftTextP1.gameObject.SetActive(true);
        enemiesLeftTextP2.gameObject.SetActive(true);
        playerOneHB.gameObject.SetActive(true);
        playerOneHBOutline.gameObject.SetActive(true);
        playerTwoHB.gameObject.SetActive(true);
        playerTwoHBOutline.gameObject.SetActive(true);
    }

    private void GoToNextLevel()
    {
        StopAllCoroutines();
        if (level < maxLevel)
        {
            Destroy(mazeInstance.gameObject);
            if (playerOneInstance != null) Destroy(playerOneInstance.gameObject);
            if (playerTwoInstance != null) Destroy(playerTwoInstance.gameObject);
            foreach (Enemy enemy in enemies) Destroy(enemy.gameObject);
            enemies.Clear();
            enemyStartCoordinates.Clear();
            foreach (GameObject bullet in bulletPool.pool) bullet.SetActive(false);
            level++;
            StartCoroutine(BeginGame());
        }
        else winText.gameObject.SetActive(true);
    }
}
