using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rigid;
    public int HP;
    private Color color;
    private SpriteRenderer spriteRenderer;
    private float speed = 0.1f;
    public GameObject BombPrefab;
    private bool isInjured;
    private int explodeRange;
    private float delayTime;
    private int bombCount;
    private const int FreezeTime = 3;

    public void Init(int explodeRange, int HP, float delayTime, int bombCount)
    {
        this.explodeRange = explodeRange;
        this.HP = HP;
        this.delayTime = delayTime;
        this.bombCount = bombCount;
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    private void Update()
    {
        playerMove();
        CreateBomb();
    }

    private void CreateBomb()
    {
        if (Input.GetKeyDown(KeyCode.Space) && bombCount > 0)
        {
            bombCount--;
            Vector2 pos = new Vector2(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            GameObject bomb = ObjectPool.Instance.Get(ObjectType.Bomb, pos);
            bomb.GetComponent<Bomb>().InitBomb(explodeRange, delayTime, addBombCount);
        }
    }

    private void playerMove()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        //print("h: " + h + " v: " + v);

        anim.SetFloat("Horizontal", h);
        anim.SetFloat("Vertical", v);

        rigid.MovePosition(transform.position + new Vector3(h, v) * speed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   

        if (speed != 0 && collision.CompareTag(TagEnum.FreezeEffect)) 
        {
            StartCoroutine(FreezePlayer());
        }

        // invincible status
        if (isInjured) return;

        if (collision.CompareTag(TagEnum.Enemy) || collision.CompareTag(TagEnum.BombEffect))
        {
            if (HP > 0)
            {
                HP--;
                if (HP >= 1)
                {
                    StartCoroutine("Injured", 2f);
                }
                else
                {
                    // TODO: add die animation & game over, after week 6
                    print("Game Over");
                }
            }

        }
    }

    IEnumerator Injured(float time)
    {
        isInjured = true;
        // 2s -> 4 times 
        for (int i = 0; i < time * 2; i++)
        {
            color.a = 0;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);

            color.a = 1;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(0.25f);
        }
        isInjured = false;
    }

    // used as a parameter to pass 
    private void addBombCount()
    {
        bombCount++;
    }

    IEnumerator FreezePlayer()
    {
        float curSpeed = speed;
        speed = 0;
        yield return new WaitForSeconds(FreezeTime);
        speed = curSpeed;
    }

}
