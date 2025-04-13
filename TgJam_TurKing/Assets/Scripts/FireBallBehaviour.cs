using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallBehaviour : MonoBehaviour
{
	public float speed = 5f; 
	public int damage = 2;
	public float radius = 15f;
	public float targetStrengh = 0.8f;

	private Rigidbody rb;

	private bool isReflected = false;
	private Transform target = null;


	private void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		rb.useGravity = false;
	}

	private void Update()
	{
		if (isReflected)
		{
			DetectEnemies();
			FollowToEnemy();
		}
		rb.velocity = transform.forward * speed * Time.deltaTime;
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other?.tag == "Player")
		{
			PlayerAttack player = other.gameObject.GetComponent<PlayerAttack>();
			player.Damaged(damage);
		}
		else if (other?.tag == "Enemy")
		{
			BaseEnemy enemy = other.gameObject.GetComponent<BaseEnemy>();
			enemy.Damaged(damage);
		}
		
		Destroy(gameObject);
	}

	public void Reflection(Vector3 dir)
	{
		if (isReflected) { return; }
		transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
		speed = speed * 1.3f;
		isReflected = true;
	}

	private void DetectEnemies()
	{
		Transform newTarget = null;

		Vector3 center = transform.position;

		Collider[] hitColliders = Physics.OverlapSphere(center, radius);

		foreach (Collider col in hitColliders)
		{
			if (col == target) return;
			
			if (col.tag == "Enemy")
			{
				newTarget = col.transform;
			}
		}

		if (newTarget == null)
		{
			target = null;
		}else
		{
			target = newTarget;
		}
	}

	private void FollowToEnemy()
	{
		if (target != null)
		{
			Vector3 dir = Vector3.Lerp(transform.forward, target.position - transform.position, targetStrengh * Time.deltaTime);
			dir.Normalize();

			transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
		}
	}
}
