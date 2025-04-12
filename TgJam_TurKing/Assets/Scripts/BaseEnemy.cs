
using System.Collections;
using UnityEngine;

public class BaseEnemy : MonoBehaviour
{
    public float hp = 30, speed = 10, damage = 1, DetectFarDistance = 30, DetectDistance = 10, DetectNearDistance = 3, distance;
    public float shootCd = 3, attackCd = 1.5f, attackKnocking = 2f;
    public Transform mainCamera;
    public PlayerAttack playerAttack;
    public Rigidbody rb;
    public Animator animator;
    public EnemyState currentState, previousState;
    public GameObject DeadEffect;
    public bool staned = false;
    public int magicCost;
    public GameObject ExplotionEffect;

    protected float currentShootCd = 3, currentAttackCd = 1.5f;

	public void SetState(EnemyState state)
    {
        currentState?.Exit(this);
        currentState = state;
        currentState.Enter(this);
    }

	private void OnEnable()
	{
        if (mainCamera == null)
		mainCamera = Camera.main.transform;

        if(playerAttack == null)
        playerAttack = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAttack>();
	}

	public void Update()
    {
        currentShootCd -= Time.deltaTime;
        currentAttackCd -= Time.deltaTime;
        if (!staned)
        {
            currentState.Update(this);
        }
    }


    public bool FarDetect()
    {
        return (mainCamera.position - transform.position).magnitude < DetectFarDistance && CheckObstacle() && !Detect() && !Near();
    }
    public bool Detect()
    {
        distance = (mainCamera.position - transform.position).magnitude;
        return (mainCamera.position - transform.position).magnitude < DetectDistance && CheckObstacle() && !Near();
    }

    public bool Near()
    {
        return (mainCamera.position - transform.position).magnitude < DetectNearDistance && CheckObstacle();
    }

    private bool CheckObstacle()
    {
        return IsBlocked(transform, mainCamera);
    }

    public virtual void Shoot()
    {
        if (currentShootCd < 0)
        {
            animator.SetTrigger("Shoot");
            playerAttack.Damaged((int)damage);
            currentShootCd = shootCd;
        }
    }

    public virtual void Attack()
    {
        if (currentAttackCd < 0)
        {
            animator.SetTrigger("Attack");
            playerAttack.Damaged((int)damage);
            Knocking();
            currentAttackCd = attackCd;
        }
    }

    public void Knocking()
    {
        Vector3 dir = mainCamera.position - transform.position;
        dir.Normalize();
        playerAttack.Knock(dir * attackKnocking);
    }

    public virtual void Follow()
    {
        Vector3 lookPos = mainCamera.position;
        lookPos.y = transform.position.y;
        transform.LookAt(lookPos);
        Vector3 move = transform.forward * speed * Time.deltaTime;
        move.y = rb.velocity.y;
        rb.velocity = move;
    }

    public void Damaged(int damage)
    {
        hp -= damage;
        Debug.Log("It is hurt such as: " + damage);
        if (hp < 0)
        {
            Death();
            return;
        }

        StartCoroutine(Stun());
    }

    public void Death()
    {
        //Debug.Log(name + " was Dead!");
        PlayerMagic.instance.GetMagicProgress(magicCost);
        Instantiate(ExplotionEffect,transform.position, Quaternion.identity);
        Instantiate(DeadEffect, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    private bool IsBlocked(Transform from, Transform to)
    {
        return Physics.Linecast(from.position, to.position, out RaycastHit hit) && hit.transform != to;
    }

    private IEnumerator Stun()
    {
        previousState = currentState;
        SetState(new Damaged());
        yield return new WaitForSeconds(0.4f);
        SetState(previousState);
    }

    public void DebugLog(string text)
    {
        //Debug.Log(name + " was set state: " + text);    
    }
}
