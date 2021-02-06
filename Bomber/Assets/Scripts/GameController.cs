using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameController works as a middleware to pass functions betw Bomb.cs & MapController.cs 
public class GameController : MonoBehaviour
{
    // public static => single instance, use with awake()
    public static GameController Instance;
    public GameObject playerPrefab;
    private MapController mapController;
    private int gameLevel = 0;
    private GameObject playerObj;

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
        if (Input.GetKeyDown(KeyCode.N))
        {
            GenerateGame();
        }
    }

    public void GenerateGame()
    {
        int x = (int)Mathf.Min((float)(6 + 2 * (gameLevel / 3)), 18f);
        int y = (int)Mathf.Min((float)(3 + 2 * (gameLevel / 3)), 15f);
        int enemyCount = (int)(gameLevel * 1.5f) + 1;
        mapController.InitMap(x, y, x * y, enemyCount);

        if (playerObj == null)
        {
            playerObj = Instantiate(playerPrefab);
        }
        playerObj.transform.position = mapController.GetPlayerPos();
        // TODO：add the restriction for the number of bombs and locations
        playerObj.GetComponent<PlayerController>().Init(1, 3, 1.5f);

        gameLevel++;
    }

    public bool IsBoundingWall(Vector2 pos)
    {
        return mapController.IsBoundingWall(pos);
    }
}
