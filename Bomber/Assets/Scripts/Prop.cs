using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    public Sprite[] propSpriteArray;
    public Sprite defaultSprite;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(TagEnum.BombEffect))
        {
            gameObject.GetComponent<Collider2D>().isTrigger = true;
        }
    }
}
