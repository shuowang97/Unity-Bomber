﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public GameObject BombEffectPrefab;
    private int explodeRange;

    public void InitBomb(int range, float delayTime)
    {
        this.explodeRange = range;
        StartCoroutine("explode", delayTime);
    }

    IEnumerator explode(float time)
    {
        yield return new WaitForSeconds(time);
        // TODO: need quaternion ?
        Instantiate(BombEffectPrefab, transform.position, Quaternion.identity);
        // generate based on range
        ExplodeAllPositions(Vector2.up);
        ExplodeAllPositions(Vector2.down);
        ExplodeAllPositions(Vector2.left);
        ExplodeAllPositions(Vector2.right);

        Destroy(gameObject);
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
            GameObject obj = Instantiate(BombEffectPrefab);
            obj.transform.position = pos;
        }
    }


}