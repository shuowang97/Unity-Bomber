using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagEnum.BombEffect))
        {   
            GameController.Instance.AddEmptyPos(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y));
            ObjectPool.Instance.Add(ObjectType.Wall, gameObject);
        }
    }
}
