

using System.Collections;
using UnityEngine;

public class FioletovyiChert : BaseEnemy
{
	public float minDistance = 1f;
	public GameObject fireBall;

	private float rayDistance = 4f;
	private bool isShooting = false;

	private void Awake()
	{
		SetState(new IdleState());
	}

	private void Update()
	{
		base.Update();

		Debug.DrawRay(transform.position + Vector3.up * 1.5f, (transform.forward + Vector3.down) * rayDistance, Color.green);
	}

	public override void Follow()
	{
		if (PlatformDetect())
		{
			Vector3 directionAwayFromPlayer = transform.position - mainCamera.position;
			directionAwayFromPlayer.y = 0f;
			transform.rotation = Quaternion.LookRotation(directionAwayFromPlayer, Vector3.up);

			if (!staned) rb.velocity = transform.forward * speed * Time.deltaTime;
		}
	}

	public override void Shoot()
	{
		transform.LookAt(playerAttack.transform.position);

		if (isShooting)
			return;

		StartCoroutine(Shooting());
	}

	private bool PlatformDetect()
	{
		Ray ray = new Ray(transform.position + Vector3.up * 1.5f, transform.forward + Vector3.down);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, rayDistance))
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	private IEnumerator Shooting()
	{
		isShooting = true;

		animator.SetTrigger("Magic");
		yield return new WaitForSeconds(1f);
		FireBallLaunch();
		yield return new WaitForSeconds(1f);

		isShooting = false;
	}

	private void FireBallLaunch()
	{
		Transform CamTrans = Camera.main.transform;
		Vector3 pos = transform.position + transform.forward * 2f + Vector3.up * 1.5f;

		GameObject fireballOb = Instantiate(fireBall, pos, transform.rotation);
	}


}
