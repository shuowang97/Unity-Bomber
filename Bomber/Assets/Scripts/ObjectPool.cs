using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    BoundingWall,
    Wall,
    Prop,
    Bomb,
    Enemy,
    BombEffect
}

// add this tag to Type_Prefab class to show it in unity UI
[System.Serializable]
public class Type_Prefab
{
    public ObjectType type;
    public GameObject prefab;
}

public class ObjectPool : MonoBehaviour
{
    // single instance, collocation with Awake()
    public static ObjectPool Instance;
    public List<Type_Prefab> type_Prefabs = new List<Type_Prefab>();

    private Dictionary<ObjectType, List<GameObject>> dic = new Dictionary<ObjectType, List<GameObject>>();

    private void Awake()
    {
        Instance = this;
    }

    /*
        Using Get() & Add() to avoid creating & destroying all objs
        Get(): find GameObj based on type, if cannot find one, Instantiate a new one
        Add()：recycle all items into dic
    */

    public GameObject Get(ObjectType type, Vector2 pos)
    {
        GameObject temp = null;
        if (!dic.ContainsKey(type))
        {
            dic.Add(type, new List<GameObject>());
        }

        if (dic[type].Count > 1)
        {
            int idx = dic[type].Count - 1;
            temp = dic[type][idx];
            dic[type].RemoveAt(idx);
        }
        else
        {
            GameObject prefab = GetPrefabByType(type);
            if (prefab != null)
            {
                temp = Instantiate(prefab, transform);
            }
        }
        // display the object
        temp.SetActive(true);
        temp.transform.position = pos;
        temp.transform.rotation = Quaternion.identity;
        return temp;
    }

    public void Add(ObjectType type, GameObject obj)
    {
        if (dic.ContainsKey(type) && !dic[type].Contains(obj))
        {
            dic[type].Add(obj);
        }
        // hide the object after putting it in pool
        obj.SetActive(false);
    }

    private GameObject GetPrefabByType(ObjectType type)
    {
        foreach (var item in type_Prefabs)
        {
            if (item.type == type)
            {
                return item.prefab;
            }
        }

        return null;
    }
}
