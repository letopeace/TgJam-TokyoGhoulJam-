using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int maxHP;
    public int hp;
    public float damage;
    public float attackColdown;
    public float microColdown;
    public Vector3 AttackZoneSize;
    public Vector3 AttackZoneOrigin;
    public float AttackZoneRange;
    public bool isAttacking;
    public int comboNumber;
    public bool canAttack = true;
    public bool attackQueue = false;
    private float cdTime;

    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement movement;
    [SerializeField] ParticleSystem particle;
    [SerializeField] Transform[] hpIcons;


    private string[] clipNames = { "PlayerAttact1", "PlayerAttact2", "PlayerAttact3" };
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack)
            {
                AnimAttack();
            }
            else
            {
                attackQueue = true;
            }

        }


        if (isAttacking) Attack();

        if (canAttack)
            cdTime += Time.deltaTime;
        DisplayIcon();
    }

    void AnimAttack()
    {
        if (cdTime > 0.1f)
            comboNumber = 0;
        cdTime = 0;

        RuntimeAnimatorController controller = anim.runtimeAnimatorController;
        float animTime = 0f;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipNames[comboNumber]) animTime = clip.length;
        }
        anim.Play(clipNames[comboNumber]);  
        
        AttackingTime(animTime);
        if (comboNumber == 2) AttackColdown(attackColdown + animTime);
        else AttackColdown(animTime);
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

    public void Damaged(int damage)
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

    void DisplayIcon()
    {
        for(int i = 0; i < maxHP; i++)
        {
            if (i < hp) hpIcons[i].DOScale(Vector3.one, 1f);
            else hpIcons[i].DOScale(Vector3.zero, 1f);
        }
    }

    private void Death()
    {

    }

    async Task AttackColdown(float time)
    {
        canAttack = false;
        particle.Play();

        await Task.Delay(Convert.ToInt32(time * 1000f));

        particle.Stop();
        canAttack = true;


        if (attackQueue)
        {
            attackQueue = false;
            AnimAttack();
        }
    }
    async Task AttackingTime(float time)
    {
        await Task.Delay((int)(time * 1000f * 0.1f));

        isAttacking = true;
        await Task.Delay((int)(time * 1000f * 0.8f));
        isAttacking = false;

        await Task.Delay((int)(time * 1000f * 0.1f));
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
