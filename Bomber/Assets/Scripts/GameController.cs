using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameController works as a middleware to pass functions betw Bomb.cs & MapController.cs 
public class GameController : MonoBehaviour
{
    // public static => single instance, use with awake()
    public static GameController Instance;
    public GameObject playerPrefab;
    private int gameLevel = 0;
    private int time;
    private float deltaTime;
    private PlayerController playerController;
    private MapController mapController;
    private GameObject playerObj;
    public int x, y;
    public int enemyCount;


    private void Awake()
    {
        // single instance
        Instance = this;
    }

    private void Start()
    {
        mapController = GetComponent<MapController>();
        GenerateGame();
    }

    //TODO: delete it later, used for testing only
    private void Update()
    {
        CountDownHelper();
        UIController.Instance.RefreshInfo(playerController.HP, gameLevel, time, enemyCount);

        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateGame();
        }
    }

    public void GenerateGame()
    {
        x = (int)Mathf.Min((float)(6 + 2 * (gameLevel / 3)), 18f);
        y = (int)Mathf.Min((float)(3 + 2 * (gameLevel / 3)), 15f);
        enemyCount = (int)(gameLevel * 1.5f) + 1;
        mapController.InitMap(x, y, x * y, enemyCount);
        time = 120 + enemyCount * 30;

        if (playerObj == null)
        {
            playerObj = Instantiate(playerPrefab);
            playerController = playerObj.GetComponent<PlayerController>();
            playerController.Init(1, 3, 1.5f, 3);
        }
        playerObj.transform.position = mapController.GetPlayerPos();

        // TODO：add the restriction for the number of bombs and locations
        gameLevel++;
    }

    public bool IsBoundingWall(Vector2 pos)
    {
        return mapController.IsBoundingWall(pos);
    }

    public bool IsEmptyPosition(Vector2 pos)
    {
        return mapController.IsEmptyPosition(pos);
    }

    public void AddEmptyPos(Vector2 pos)
    {
        mapController.AddEmptyPos(pos);
    }

    private void CountDownHelper()
    {
        if (time <= 0)
        {
            // TODO: add game over scene, after week 6
            return;
        }

        deltaTime += Time.deltaTime;
        if (deltaTime >= 1.0f)
        {
            time--;
            deltaTime = 0;
        }
    }
}
