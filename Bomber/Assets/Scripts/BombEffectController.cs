using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombEffectController : MonoBehaviour
{
    // triggered by events in the animation interface
    private void AnimationEnds()
    {
        // Destroy(gameObject);
    }

    // another way
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // 0 refer to base layer 
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);
        // name found in animator interface
        // normalizedTime => process bar 0:begin 1:end
        if (info.normalizedTime >= 1 && info.IsName("bombEffect"))
        {
            Destroy(gameObject)
;
        }
    }

}
