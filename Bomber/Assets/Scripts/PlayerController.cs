using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rigid;
    private int HP;
    private Color color;
    private SpriteRenderer spriteRenderer;
    private const float speed = 0.1f;
    public GameObject BombPrefab;
    private bool isInjured;

    private int explodeRange;
    private float delayTime;

    public void Init(int explodeRange, int HP, float delayTime)
    {
        this.explodeRange = explodeRange;
        this.HP = HP;
        this.delayTime = delayTime;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject bomb = Instantiate(BombPrefab);
            bomb.transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y));
            bomb.GetComponent<Bomb>().InitBomb(explodeRange, delayTime);
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
        // invincible status
        if (isInjured) return;

        if (collision.CompareTag(TagEnum.Enemy) || collision.CompareTag(TagEnum.BombEffect))
        {
            HP--;
            StartCoroutine("Injured", 2f);
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
}
