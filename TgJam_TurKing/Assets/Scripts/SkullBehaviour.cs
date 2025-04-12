using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBehaviour : BaseEnemy
{
    private void Awake()
    {
        rb.useGravity = false;
        SetState(new IdleState());
    }

    public override void Shoot()
    {
        Follow();
    }

    public override void Attack()
    {
        if (currentAttackCd < 0)
        {
            currentAttackCd = attackCd;
            StartCoroutine(Attacking());
        }
    }

    public override void Follow()
    {
        transform.LookAt(player.position, Vector3.up);
        rb.velocity = transform.forward * speed * Time.deltaTime;
    }


    IEnumerator Attacking()
    {
        animator.SetBool("Taran", true);
        speed *= 2.5f;

        yield return new WaitForSeconds(1);

        animator.SetBool("Taran", false);
        speed *= 0.4f;
    }
}
