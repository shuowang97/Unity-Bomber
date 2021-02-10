using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PropType
{
    HP,
    Speed,
    Bomb,
    Range,
    Time
}

[System.Serializable]
public class PropType_Sprite
{
    // has to have public here
    public PropType propType;
    public Sprite sprite;
}

public class Prop : MonoBehaviour
{
    public PropType_Sprite[] propTypeList;
    private Sprite defaultSprite;
    private SpriteRenderer spriteRenderer;
    private PropType curProp;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if (spriteRenderer.sprite == defaultSprite && collision.CompareTag(TagEnum.BombEffect))
        {
            // change tag for prefab directly => make enemy go through it
            gameObject.tag = "Untagged";
            // change layer for prefab => avoid bugs in FindNewDir() [EnemyController.cs]
            gameObject.layer = 0;

            gameObject.GetComponent<Collider2D>().isTrigger = true;
            int idx = Random.Range(0, propTypeList.Length);
            spriteRenderer.sprite = propTypeList[idx].sprite;
            curProp = propTypeList[idx].propType;

            StartCoroutine(PropAnimation());
        }

        //TODO: Add prop functions after week 6
        if (collision.CompareTag(TagEnum.Player))
        {
            print(curProp);
            switch (curProp)
            {
                case PropType.HP:
                    break;
                case PropType.Speed:
                    break;
                case PropType.Bomb:
                    break;
                case PropType.Range:
                    break;
                case PropType.Time:
                    break;
                default:
                    break;
            }

            ResetProp();
            ObjectPool.Instance.Add(ObjectType.Prop, gameObject);
        }
    }

    private void ResetProp()
    {
        spriteRenderer.sprite = defaultSprite;
        gameObject.tag = "Wall";
        gameObject.layer = 8;
        GetComponent<Collider2D>().isTrigger = false;
    }

    IEnumerator PropAnimation()
    {
        for (int i = 0; i < 2; i++)
        {
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.25f);
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.25f);
        }
    }



}
