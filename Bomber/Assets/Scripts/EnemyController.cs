using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rigid;
    private float speed = 0.05f;
    private float rayDistance = 0.7f;
    private SpriteRenderer spriteRenderer;
    private Color color;
    private bool movable = true;

    // 0-up 1-down 2-left 3-right
    // private int dirId = 0;
    private Vector2 curDirection = new Vector2();

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    // when retrive old objs from objectPool, some object's values need to be reset
    public void InitEnemy()
    {
        color.a = 1;
        spriteRenderer.color = color;
        movable = true;
        GetDirection(Random.Range(0, 4));
    }

    private void Update()
    {
        if (movable)
        {
            rigid.MovePosition((Vector2)transform.position + curDirection * speed);
        }
        else
        {
            FindNewDir();
        }
    }

    private void GetDirection(int dir)
    {
        switch (dir)
        {
            case 0:
                curDirection = Vector2.up;
                break;
            case 1:
                curDirection = Vector2.down;
                break;
            case 2:
                curDirection = Vector2.left;
                break;
            case 3:
                curDirection = Vector2.right;
                break;
            default:
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagEnum.BombEffect))
        {
            ObjectPool.Instance.Add(ObjectType.Enemy, gameObject);
        }

        // transparent effect when 2 enemies meet
        if (collision.CompareTag(TagEnum.Enemy))
        {
            color.a = 0.3f;
            spriteRenderer.color = color;
        }

        if (!collision.CompareTag(TagEnum.BoundingWall) && !collision.CompareTag(TagEnum.Wall))
        {
            return;
        }

        // handle position offsets
        Vector2 correctPos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
        transform.position = correctPos;

        FindNewDir();

    }

    private void FindNewDir()
    {
        List<int> dirList = new List<int>();
        // 4th para is for layermask:
        //                           1 << 8 -> only 8th layer, 0 << 8 -> ignore 8th layer 
        if (Physics2D.Raycast(transform.position, Vector2.up, rayDistance, 1 << 8) == false)
        {
            dirList.Add(0);
        }
        if (Physics2D.Raycast(transform.position, Vector2.down, rayDistance, 1 << 8) == false)
        {
            dirList.Add(1);
        }
        if (Physics2D.Raycast(transform.position, Vector2.left, rayDistance, 1 << 8) == false)
        {
            dirList.Add(2);
        }
        if (Physics2D.Raycast(transform.position, Vector2.right, rayDistance, 1 << 8) == false)
        {
            dirList.Add(3);
        }

        // print(dirList.Count);
        if (dirList.Count > 0)
        {
            movable = true;
            int idx = Random.Range(0, dirList.Count);
            GetDirection(dirList[idx]);
        }
        else
        {
            movable = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(TagEnum.Enemy))
        {
            color.a = 1;
            spriteRenderer.color = color;
        }
    }

    // private void OnTriggerStay2D(Collider2D collision)
    // {
    //     if (collision.CompareTag("Enemy"))
    //     {
    //         color.a = 0.3f;
    //         spriteRenderer.color = color;
    //     }
    // }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, 1 * rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -1 * rayDistance, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(1 * rayDistance, 0, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-1 * rayDistance, 0, 0));
    }


}
