using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class PlayerMovement : MonoBehaviour
{

	public float speed = 10f, jumpForce = 1.5f;
	public float wallRunClimbSpeed = 10f, maxClimbSpeed = 10f, maxSlideSpeed = 10f;
	public float dashForce;
	public float dashingTime;
	public float dashColdown;
	public float slashDistance;
	public float slashingTime;

	public bool onGround;
	public bool canMove;
	public bool onWall;
	public bool isBlocked;
	public bool canDash;


	public Rigidbody rb;
	private Vector3 wallNormal;
	public float timeOnWallMax = 1.5f;
	public float timeOnWall = 1.5f;


	private GameObject previousPlatform;
	private float nowDashingTime = 0f;
	private Vector3 dashDirection;
	[SerializeField] private bool isEverGrounded = true;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	private void Update()
	{
		Jump();
		Dash();

		if (onWall)
		{
			timeOnWall -= Time.deltaTime;

			timeOnWall = Mathf.Max(timeOnWall, -0.1f);
		}
		else
		{
			timeOnWall += Time.deltaTime;

			timeOnWall = Mathf.Min(timeOnWall, timeOnWallMax);
		}
	}

	private void FixedUpdate()
	{
		WASD();
	}

	private void WASD()
	{
		if (isBlocked)
		{
			return;
		}

		if (!onGround && onWall)
		{
			MoveOnWall();
			return;
		}

		rb.useGravity = true;

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 dir = Vector3.ClampMagnitude((transform.right * h + transform.forward * v), 1f);
		dir = dir * Time.fixedDeltaTime * speed + dashDirection * dashForce * (nowDashingTime / dashingTime) * (nowDashingTime / dashingTime);
		if (dir.y == 0f) dir.y = rb.velocity.y;

		//rb.velocity = Vector3.Lerp(rb.velocity, dir, 0.1f);
		if (canMove) rb.velocity = dir;
	}

	private void MoveOnWall()
	{
		if (!onWall)
		{ return; }

		float h = Input.GetAxis("Horizontal");
		float v = Input.GetAxis("Vertical");

		Vector3 playerDir = transform.right * h + transform.forward * v;


		Vector3 wallRight = Vector3.Cross(Vector3.up, wallNormal).normalized;
		Vector3 wallLeft = -wallRight;

		Vector3 camForward = Camera.main.transform.forward;
		Vector3 projectedView = Vector3.ProjectOnPlane(camForward, wallNormal).normalized;

		Vector3 wallRunDirection =
		Vector3.Dot(projectedView, wallRight) > 0 ? wallRight : wallLeft;

		Debug.Log("!CompareNormal(wallRunDirection, playerDir, 180): " + !CompareNormal(wallRunDirection, playerDir, 180));
		Debug.Log("CompareNormal(playerDir, wallNormal, 20): " + CompareNormal(playerDir, wallNormal, 20));
		Debug.Log("playerDir == Vector3.zero: " + (playerDir == Vector3.zero));

		if (!CompareNormal(wallRunDirection, playerDir, 180) || CompareNormal(playerDir, wallNormal, 45) || playerDir == Vector3.zero || Input.GetAxis("Vertical") < 0)
		{
			onWall = false;
			StartCoroutine(WaitWallNormal());
		}

		rb.useGravity = false;

		isEverGrounded = true;

		float verticalInfluence = Mathf.Clamp(camForward.y, -0.3f, 0.3f);
		if (timeOnWall < 0)
		{
			verticalInfluence = -1f;
		}

		Vector3 finalVelocity = wallRunDirection * speed * Time.fixedDeltaTime + Vector3.up * verticalInfluence * wallRunClimbSpeed;
		finalVelocity.y = Mathf.Clamp(finalVelocity.y, -maxSlideSpeed, maxClimbSpeed);

		rb.velocity = finalVelocity;

		
	}

	private void Jump()
	{
		if ((onGround || onWall) && Input.GetKeyDown(KeyCode.Space))
		{
			if (onWall)
			{
				onWall = false;
				
				Vector3 dir = wallNormal * 2 + Vector3.up;
				rb.velocity = dir * jumpForce * 2;
				StartCoroutine(WaitWallNormal());
			}
			else
			{
				Vector3 dir = Vector3.up * 2;
				rb.velocity = dir * jumpForce;
			}
		}

	}

	void Dash()
	{
		float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
		dashDirection = Vector3.ClampMagnitude((Camera.main.transform.right * h + Camera.main.transform.forward * v), 1f);
		isEverGrounded = isEverGrounded | OnGround();
		if (canDash && Input.GetKeyDown(KeyCode.LeftShift) && isEverGrounded)
		{
			RaycastHit hit;
			Physics.BoxCast(Camera.main.transform.position, Vector3.one * 0.8f, dashDirection, out hit);
			if (hit.collider != null && hit.collider.gameObject.GetComponent<BaseEnemy>() != null && hit.distance < slashDistance)
			{
				Slash(hit.collider.gameObject);
				return;
			}
			canDash = false;
			nowDashingTime = dashingTime;
			isEverGrounded = false;
			DashColdown();
		}

		nowDashingTime = Mathf.Clamp(nowDashingTime - Time.deltaTime, 0f, nowDashingTime);
	}

	async Task Slash(GameObject target)
	{
		rb.velocity = (target.transform.position - transform.position).normalized * Vector3.Distance(transform.position, target.transform.position) / slashingTime;
		canMove = false;
		GetComponent<PlayerAttack>().isAttacking = true;
		await Task.Delay((int)(slashingTime * 1000f));
		rb.velocity = Vector3.zero;
		canMove = true;
		GetComponent<PlayerAttack>().isAttacking = false;
	}

	public void MoveBlocking()
	{
		if (!isBlocked)
			StartCoroutine(Block());
	}

	private IEnumerator Block()
	{
		isBlocked = true;
		yield return new WaitForSeconds(0.5f);
		isBlocked = false;
	}
	async Task DashColdown()
	{
		await Task.Delay((int)(dashColdown * 1000f));
		canDash = true;
	}

	private void OnCollisionStay(Collision collision)
	{

		if (collision.collider.tag != "Platform")
		{
			return;
		}

		ContactPoint[] contacts = collision.contacts;
		if (contacts.Length == 0)
		{
			StartCoroutine(WaitGround());
			onWall = false;
		}

		for (int i = 0; i < contacts.Length; i++)
		{

			if (contacts[i].normal.y > 0.66f)
			{
				onGround = true;
				previousPlatform = null;
				
				wallNormal = Vector3.zero;
				onWall = false;
				return;
			}
			else if (contacts[i].normal.y > -0.66f && contacts[i].normal.y < 0.66f)
			{
				if (!onWall && (wallNormal == Vector3.zero || !CompareNormal(wallNormal, contacts[i].normal)))
				{
					float h = Input.GetAxis("Horizontal");
					float v = Input.GetAxis("Vertical");

					Vector3 playerDir = transform.right * h + transform.forward * v;

					if ( playerDir == Vector3.zero || Input.GetAxis("Vertical") < 0 || previousPlatform == collision.gameObject)
					{
						
					}
					else
					{
						onWall = true;
						wallNormal = contacts[i].normal;
						previousPlatform = collision.gameObject; 
					}
				}
				else if (wallNormal != contacts[i].normal && onWall) 
				{

					wallNormal = contacts[i].normal; 
				}

				return;
			}
			else
			{
				StartCoroutine(WaitGround());
				onWall = false;
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		StartCoroutine(WaitGround());

		if (wallNormal != Vector3.zero)
		{
			onWall = false;
			
			Vector3 dir = wallNormal * 2 + Vector3.up;
			rb.velocity = dir * jumpForce * 2;
			StartCoroutine(WaitWallNormal());
		}
		else if(onWall)
		{
			onWall = false;
			
		}
	}
	bool OnGround()
	{
		Ray ray = new Ray(transform.position, Vector3.down * 0.2f);
		RaycastHit hit;
		Physics.Raycast(ray, out hit);
		if(hit.collider != null && Vector3.Distance(transform.position,hit.point) < 0.2f)
		{
			return true;
		}
		return false;
	}

	private bool CompareNormal(Vector3 a, Vector3 b, float toleranceInDegrees = 20f)
	{
		float angle = Vector3.Angle(a.normalized, b.normalized);
		return angle <= toleranceInDegrees;
	}


	IEnumerator WaitWallNormal()
	{
		yield return new WaitForSeconds(0.1f);
		wallNormal = Vector3.zero;
	}

	IEnumerator WaitGround()
	{
		yield return new WaitForSeconds(0.1f);
		onGround = false;
	}
}