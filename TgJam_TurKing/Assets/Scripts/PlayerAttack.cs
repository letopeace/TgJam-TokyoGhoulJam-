using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float hp;
    public float damage;
    public float attackColdown;
    public float microColdown;
    public Vector3 AttackZoneSize;
    public Vector3 AttackZoneOrigin;
    public float AttackZoneRange;
    public bool isAttacking;
    public int comboNumber;
    public bool canAttack = true;

    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement movement;
    [SerializeField] ParticleSystem particles;


    private string[] clipNames = { "PlayerAttact1", "PlayerAttact2", "PlayerAttact3" };
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) AnimAttack();
        if (isAttacking) Attack();
    }

    void AnimAttack()
    {
        RuntimeAnimatorController controller = anim.runtimeAnimatorController;
        float animTime = 0f;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipNames[comboNumber]) animTime = clip.length;
        }

        anim.SetTrigger($"Attack{comboNumber+1}");
        AttackingTime(0.18f);
        if (comboNumber == 2) AttackColdown(attackColdown + animTime);
        else AttackColdown(microColdown + animTime);
        comboNumber = (comboNumber + 1) % 3;
    }

    public void Attack()
    {
        RaycastHit[] hit;
        hit = Physics.BoxCastAll(Camera.main.transform.position + AttackZoneOrigin,AttackZoneSize, Camera.main.transform.forward, Camera.main.transform.rotation);
        for(int i=0; i < hit.Length; i++)
        {
            if (hit[i].collider!=null && hit[i].collider.gameObject.GetComponent<BaseEnemy>() != null)
            {
                hit[i].collider.gameObject.GetComponent<BaseEnemy>().Damaged((int)damage);
                isAttacking = false;
            }
        }
    }

    public void Damaged(float damage)
    {
        hp -= damage;
        Debug.Log("Damaged, hp: " + hp);
        if (hp < 0f)
        {
            Death();
        }
    }

    public void Knock(Vector3 dir)
    {
        movement.MoveBlocking();
        movement.rb.velocity = dir;
        movement.onWall = false;
        movement.timeOnWall = movement.timeOnWallMax;
    }

    private void Death()
    {

    }

    async Task AttackColdown(float time)
    {
        canAttack = false;
        particles.Play();
        await Task.Delay(Convert.ToInt32(time * 1000f));
        particles.Stop();
        canAttack = true;
    }
    async Task AttackingTime(float time)
    {
        isAttacking = true;
        await Task.Delay((int)(time * 1000f));
        isAttacking = false;
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Gizmos.color = Color.red;
        Vector3 direction = Camera.main.transform.forward;
        Vector3 origin = Camera.main.transform.position + Vector3.up * AttackZoneSize.y * 0.5f + AttackZoneOrigin;

        Matrix4x4 rotationMatrix = Matrix4x4.TRS(origin + direction * AttackZoneRange * 0.5f, Camera.main.transform.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, AttackZoneSize);
    }

}
