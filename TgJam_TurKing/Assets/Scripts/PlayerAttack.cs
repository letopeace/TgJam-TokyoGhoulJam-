using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float damage;
    public float attackColdown;
    public float microColdown;
    public int comboNumber;
    public bool canAttack = true;

    [SerializeField] Animator anim;

    private string[] clipNames = { "PlayerAttact1", "PlayerAttact2", "PlayerAttact3" };
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && canAttack) Attack(); 
    }

    void Attack()
    {
        RuntimeAnimatorController controller = anim.runtimeAnimatorController;
        float animTime = 0f;
        foreach (var clip in controller.animationClips)
        {
            if (clip.name == clipNames[comboNumber]) animTime = clip.length;
        }

        anim.SetTrigger($"Attack{comboNumber+1}");
        if (comboNumber == 2) AttackColdown(attackColdown + animTime);
        else AttackColdown(microColdown + animTime);
        comboNumber = (comboNumber + 1) % 3;
    }

    async Task AttackColdown(float time)
    {
        canAttack = false;
        await Task.Delay(Convert.ToInt32(time * 1000f));
        canAttack = true;
    }
}
