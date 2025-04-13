using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBehaviour : BaseEnemy
{

    private bool isTaran = false;

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
        if (!isTaran) transform.LookAt(mainCamera.position);
        if(!staned) rb.velocity = transform.forward * speed * Time.deltaTime;
    }



    IEnumerator Attacking()
    {
        isTaran = true;
        animator.SetBool("Taran", true);
        speed *= 2.5f;

        yield return new WaitForSeconds(1);

        animator.SetBool("Taran", false);
        speed *= 0.4f;
        isTaran = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag == "Player")
        {
            Vector3 dir = mainCamera.position - transform.position;
            dir.Normalize();

            playerAttack.Damaged((int)damage);
            playerAttack.Knock(dir * attackKnocking);
        }
    }
}

