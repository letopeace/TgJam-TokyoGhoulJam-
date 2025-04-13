using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerAttack : MonoBehaviour
{
    public int maxHP;
    public int hp;
    public float damage;
    public float damageIncrease = 10;
    public float attackColdown;
    public float microColdown;
    public Vector3 AttackZoneSize;
    public Vector3 AttackZoneOrigin;
    public float AttackZoneRange;
    public bool isAttacking;
    public int comboNumber;
    public bool canAttack = true;
    public bool attackQueue = false;
    public float KillerHeight;

    private float cdTime;

    [SerializeField] Animator anim;
    [SerializeField] PlayerMovement movement;
    [SerializeField] ParticleSystem particle;
    [SerializeField] Transform[] hpIcons;
    [SerializeField] GameObject BloodExplotion;
    [SerializeField] GameObject GlobleVolume;
    [SerializeField] Transform[] blinkEyes;
    [SerializeField] AudioClip[] attackClip;
    private bool isDead = false;
    

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

        if(transform.position.y < KillerHeight && !isDead)
        {
            Death();
        }
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

        GetComponent<AudioSource>().clip = attackClip[Random.Range(0, attackClip.Length)];
        GetComponent<AudioSource>().Play();

        AttackingTime(animTime);
        if (comboNumber == 2) AttackColdown(attackColdown + animTime);
        else AttackColdown(animTime);
        comboNumber = (comboNumber + 1) % 3;
    }

    public void Attack()
    {
        Collider[] hit;
        hit = Physics.OverlapBox(Camera.main.transform.position + AttackZoneOrigin, AttackZoneSize, Camera.main.transform.rotation);
        for(int i=0; i < hit.Length; i++)
        {
            if (hit[i]!=null && hit[i].gameObject.GetComponent<BaseEnemy>() != null)
            {
                float speed = movement.rb.velocity.magnitude;
                float boost = (speed - 15) * 0.5f;
				boost = (boost / (1 + Mathf.Abs(boost))) * damageIncrease; 
				hit[i].gameObject.GetComponent<BaseEnemy>().Damaged((int)(damage + boost));
                
            }
            if (hit[i] != null && hit[i].gameObject.GetComponent<FireBallBehaviour>() != null)
            {
                FireBallBehaviour fireBall = hit[i].gameObject.GetComponent<FireBallBehaviour>();

                fireBall.Reflection(Camera.main.transform.forward);
			}
        }
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        if (hp < 0f && !isDead)
        {
            Death();
            return;
        }
        Vignette vignette;
        GlobleVolume.GetComponent<Volume>().profile.TryGet(out vignette);
        float current = vignette.intensity.value;
        Tween vignetteTween = DOTween.To(() => current, x => {
            current = x;
            vignette.intensity.value = current;
        }, 0.20f, 0.5f).SetLoops(2, LoopType.Yoyo);
    }

    public void Knock(Vector3 dir)
    {
        movement.MoveBlocking();
        movement.rb.velocity = dir;
        movement.onWall = false;
        movement.timeOnWall = movement.timeOnWallMax;
    }

    public void PlaySlash()
    {
        anim.Play("Slash");
    }

    public void PlayerJump()
    {
        Debug.Log("Jumped");
		anim.Play("Jump");
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
        isDead = true;
        GetComponent<PlayerMovement>().canMove = false;
        GetComponent<PlayerMovement>().canDash = false;
        GetComponent<PlayerCamera>().canLook = false;
        Vignette vignette;
        GlobleVolume.GetComponent<Volume>().profile.TryGet(out vignette);
        float current = vignette.intensity.value;
        Tween vignetteTween = DOTween.To(() => current, x => {
            current = x;
            vignette.intensity.value = current;
        }, 0.35f, 0.6f).SetLoops(2,LoopType.Yoyo);

        Instantiate(BloodExplotion, transform.position, Quaternion.identity);
        Camera.main.transform.DOLocalRotate(Camera.main.transform.eulerAngles + Vector3.forward * 20f, 1.2f).SetEase(Ease.OutElastic);
        blinkEyes[0].DOMoveY(400f, 1.8f);
        blinkEyes[1].DOMoveY(400f, 1.9f).OnComplete(reStartScene);
        

    }

    void reStartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    async Task AttackColdown(float time)
    {
        canAttack = false;
        particle.Play();

        await Task.Delay((int)(time * 1000f));

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
