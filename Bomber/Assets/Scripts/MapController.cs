using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private int X, Y;
    private List<Vector2> emptyPositions = new List<Vector2>();
    private List<Vector2> boundingWallList = new List<Vector2>();

    // has to be public here, door only has one object, so do not use objectPool here
    public GameObject doorPrefab;
    private GameObject doorObj;
    // store all objects retrived from objectPool to put it back
    private Dictionary<ObjectType, List<GameObject>> objectPoolDic = new Dictionary<ObjectType, List<GameObject>>();

    public void InitMap(int x, int y, int wallCount, int enemyCount)
    {
        
        ResetMap();

        // do not destroy here in order to improve performance
        /*
        foreach (Transform item in transform)
        {   
            //Destroy(item.gameObject);
        }
        */

        X = x;
        Y = y;
        CreateBoundingWall();
        StoreEmptyPositions();
        //Debug.Log(emptyPositions.Count);
        CreateDestroyableWall(wallCount);
        CreateDoor();
        CreateProps();
        CreateEnemy(enemyCount);
    }

    // Clear: 1. emptyPosList, 2. boundingWallList, 3. objectPoolDictionary 
    // Store: store all objects in ObjectPool
    // Hide: hide all objects in objectPoolDictionary
    private void ResetMap() {
        emptyPositions.Clear();
        boundingWallList.Clear();

        foreach (var entry in objectPoolDic)
        {
            foreach (var obj in entry.Value)
            {
                ObjectPool.Instance.Add(entry.Key, obj);
            }
        }

        // clear Dictionary 
        objectPoolDic.Clear();
    }

    // add all Instantiate() objects to dictionary in order to put it back to objectPool
    private void AddToObjectDictionary(ObjectType type, GameObject obj) 
    {
        if (!objectPoolDic.ContainsKey(type))
        {
            objectPoolDic.Add(type, new List<GameObject>());
        }
        objectPoolDic[type].Add(obj);
    }

    public void CreateBoundingWall()
    {
        for (int x = -X; x < X; x += 2)
        {
            for (int y = -Y; y < Y; y += 2)
            {
                // transform refers to the parent obj of current script (GameController in this case)
                CreateBoundingWallInstance(new Vector2(x, y));

            }
        }

        // top and bottom
        for (int x = -X - 2; x <= X; x++)
        {
            CreateBoundingWallInstance(new Vector2(x, Y));
            CreateBoundingWallInstance(new Vector2(x, -Y - 2));
        }

        // left and right
        for (int y = -Y - 1; y <= Y - 1; y++)
        {
            CreateBoundingWallInstance(new Vector2(-X - 2, y));
            CreateBoundingWallInstance(new Vector2(X, y));
        }
    }

    private void CreateBoundingWallInstance(Vector2 pos)
    {
        GameObject obj = ObjectPool.Instance.Get(ObjectType.BoundingWall, pos);
        boundingWallList.Add(pos);
        AddToObjectDictionary(ObjectType.BoundingWall, obj);
    }

    public bool IsBoundingWall(Vector2 pos)
    {
        return boundingWallList.Contains(pos);
    }

    private void StoreEmptyPositions()
    {
        for (int x = -X - 1; x <= X - 1; x++)
        {
            if ((-X - 1) % 2 == x % 2)
            {
                for (int y = -Y - 1; y <= Y - 1; y++)
                {
                    emptyPositions.Add(new Vector2(x, y));
                }
            }
            else
            {
                for (int y = -Y - 1; y <= Y - 1; y += 2)
                {
                    emptyPositions.Add(new Vector2(x, y));
                }
            }
        }

        // remove (left top corner)
        emptyPositions.Remove(new Vector2(-X - 1, Y - 1));
        emptyPositions.Remove(new Vector2(-X, Y - 1));
        emptyPositions.Remove(new Vector2(-X - 1, Y - 2));
    }

    private void CreateDestroyableWall(int num)
    {
        if (num > emptyPositions.Count * 0.4)
        {
            num = (int)(emptyPositions.Count * 0.4);
        }

        // randomly generate destroyable walls 
        for (int i = 0; i < num; i++)
        {
            int idx = Random.Range(0, emptyPositions.Count);
            GameObject obj = ObjectPool.Instance.Get(ObjectType.Wall, emptyPositions[idx]);
            AddToObjectDictionary(ObjectType.Wall, obj);

            // avoid duplicates
            emptyPositions.RemoveAt(idx);
        }
    }

    private void CreateDoor()
    {
        int idx = Random.Range(0, emptyPositions.Count);
        if (doorObj == null)
        {
            doorObj = Instantiate(doorPrefab, transform);
        }
        doorObj.transform.position = emptyPositions[idx];
        doorObj.GetComponent<Door>().ResetDoor();
        emptyPositions.RemoveAt(idx);
    }

    private void CreateProps()
    {
        int count = Random.Range(0, (int)(2 + emptyPositions.Count * 0.05f));
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, emptyPositions.Count);
            GameObject obj = ObjectPool.Instance.Get(ObjectType.Prop, emptyPositions[idx]);
            AddToObjectDictionary(ObjectType.Prop, obj);
            emptyPositions.RemoveAt(idx);
        }

    }

    public Vector2 GetPlayerPos()
    {
        return new Vector2(-X - 1, Y - 1);
    }

    private void CreateEnemy(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int idx = Random.Range(0, emptyPositions.Count);
            GameObject obj = ObjectPool.Instance.Get(ObjectType.Enemy, emptyPositions[idx]);
            obj.GetComponent<EnemyController>().InitEnemy();
            AddToObjectDictionary(ObjectType.Enemy, obj);
            emptyPositions.RemoveAt(idx);
        }
    }
}
