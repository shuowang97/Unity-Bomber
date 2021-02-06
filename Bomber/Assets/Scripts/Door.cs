using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite doorSprite, defaultSprite;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void ResetDoor()
    {
        spriteRenderer.sprite = defaultSprite;
        GetComponent<Collider2D>().isTrigger = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagEnum.BombEffect))
        {
            spriteRenderer.sprite = doorSprite;
            GetComponent<Collider2D>().isTrigger = true;
        }
        else if (collision.CompareTag(TagEnum.Player))
        {
            // remaining enemies == 0 
            GameController.Instance.GenerateGame();
            print("go to next level");

        }
    }
}
