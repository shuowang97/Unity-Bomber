using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BombType
{
    Normal,
    Freeze,
    Kick
}

[System.Serializable]
public class BombType_Sprite
{
    public BombType bombType;
    public Sprite sprite;
}

public class Bomb : MonoBehaviour
{
    public BombType_Sprite[] bombTypeList;
    private SpriteRenderer spriteRenderer;
    public GameObject BombEffectPrefab;
    private int explodeRange;
    private BombType curBomb;
    private ContactPoint2D[] contactPoint = new ContactPoint2D[1]; // store contact point
    private System.Action addBombCountAction;


    private Animator animator;
    public AnimationClip[] animations;
    private AnimatorOverrideController overrideController;

    private void Start()
    {
        animator = GetComponent<Animator>();

        RuntimeAnimatorController runtimeAnimatorController = animator.runtimeAnimatorController;

        overrideController = new AnimatorOverrideController();
        overrideController.runtimeAnimatorController = runtimeAnimatorController;
    }

    private void Update()
    {
        switch (curBomb)
        {
            case BombType.Normal:
                overrideController["bomb"] = animations[0];
                break;
            case BombType.Freeze:
                overrideController["bomb"] = animations[1];
                break;
            case BombType.Kick:
                overrideController["bomb"] = animations[2];
                break;
            default:
                break;
        }
        animator.runtimeAnimatorController = overrideController;
    }


    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // TODO: important! Pass function as a variable from PlayerController.cs (delegate)
    public void InitBomb(int range, float delayTime, System.Action action)
    {
        ResetBomb();
        addBombCountAction = action;

        int idx = Random.Range(0, bombTypeList.Length);
        spriteRenderer.sprite = bombTypeList[idx].sprite;
        curBomb = bombTypeList[idx].bombType;

        // leave more time to kick the bomb
        if (curBomb == BombType.Kick)
        {
            delayTime = delayTime * 2.0f;
            StartCoroutine(AddFunctionsToKick());
        }

        this.explodeRange = range;
        StartCoroutine("explode", delayTime);
    }

    public void ResetBomb()
    {
        if (gameObject.GetComponent<CircleCollider2D>() != null)
        {
            CircleCollider2D circleCollider2D = gameObject.GetComponent<CircleCollider2D>();
            Destroy(circleCollider2D);
        }
    }

    IEnumerator AddFunctionsToKick()
    {
        yield return new WaitForSeconds(1);

        if (curBomb == BombType.Kick)
        {
            gameObject.AddComponent<CircleCollider2D>();
        }
    }

    IEnumerator explode(float time)
    {
        yield return new WaitForSeconds(time);
        // ObjectPool.Instance.Get(ObjectType.BombEffect, transform.position);
        InstantiateBombEffect(transform.position);

        // generate based on range
        ExplodeAllPositions(Vector2.up);
        ExplodeAllPositions(Vector2.down);
        ExplodeAllPositions(Vector2.left);
        ExplodeAllPositions(Vector2.right);

        ObjectPool.Instance.Add(ObjectType.Bomb, gameObject);

        // called the function from playctrl.cs to add bombCount back
        if (addBombCountAction != null)
        {
            addBombCountAction();
        }
    }

    private void ExplodeAllPositions(Vector2 dir)
    {
        for (int i = 1; i <= explodeRange; i++)
        {
            Vector2 pos = (Vector2)transform.position + dir * i;
            if (GameController.Instance.IsBoundingWall(pos))
            {
                break;
            }
            // ObjectPool.Instance.Get(ObjectType.BombEffect, pos);
            InstantiateBombEffect(pos);
        }
    }

    private void OnCollisionEnter2D(Collision2D clo)
    {
        float x = gameObject.transform.position.x;
        float y = gameObject.transform.position.y;
        Vector2 newPos = new Vector2(x, y);

        if (clo.contacts[0].normal.y <= -0.9) //collide in top
        {
            newPos.y = y - 2;
            while (newPos.y != -GameController.Instance.y - 3 && !GameController.Instance.IsEmptyPosition(newPos) && !IsLeftTopCorner(newPos))
            {
                newPos.y -= 1;
            }
        }
        else if (clo.contacts[0].normal.y >= 0.9) //collide in bottom
        {
            newPos.y = y + 2;
            while (newPos.y != GameController.Instance.y + 1 && !GameController.Instance.IsEmptyPosition(newPos) && !IsLeftTopCorner(newPos))
            {
                newPos.y += 1;
            }
        }
        else if (clo.contacts[0].normal.x <= -0.9) //collide in right
        {
            newPos.x = x - 2;
            while (newPos.x != -GameController.Instance.x - 3 && !GameController.Instance.IsEmptyPosition(newPos) && !IsLeftTopCorner(newPos))
            {
                newPos.x -= 1;
            }
        }
        else if (clo.contacts[0].normal.x >= 0.9) //collide in left
        {
            newPos.x = x + 2;
            while (newPos.x != GameController.Instance.x + 1 && !GameController.Instance.IsEmptyPosition(newPos) && !IsLeftTopCorner(newPos))
            {
                newPos.x += 1;
            }
        }

        gameObject.transform.position = newPos;
    }

    public bool IsLeftTopCorner(Vector2 pos)
    {
        float x = GameController.Instance.x;
        float y = GameController.Instance.y;

        if ((pos.x == (float)(-x - 1) && pos.y == (float)(y - 1)) ||
            (pos.x == (float)(-x) && pos.y == (float)(y - 1)) ||
            (pos.x == (float)(-x - 1) && pos.y == (float)(y - 2)))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // using objectPool to instantiate bombEffect
    // using bombType to set tag
    private void InstantiateBombEffect(Vector2 pos)
    {
        // ObjectPool.Instance.Get(ObjectType.BombEffect, transform.position);
        GameObject effectObj = ObjectPool.Instance.Get(ObjectType.BombEffect, pos);

        // add freeze effect
        if (curBomb == BombType.Freeze)
        {
            effectObj.tag = "FreezeEffect";
        }
        else
        {
            effectObj.tag = "BombEffect";
        }
    }

}
