using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public float radius;
    public float internalRaduis;
    public float holeForce;
    public float speed;
    public float lifeTime;
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);

        Collider[] coliders = Physics.OverlapSphere(transform.position, radius);
        Collider[] inColiders = Physics.OverlapSphere(transform.position, internalRaduis);

        for(int i = 0; i < coliders.Length; i++)
        {
            if (coliders[i] != null && coliders[i].gameObject.GetComponent<BaseEnemy>()  != null)
            {
                coliders[i].GetComponent<Rigidbody>().velocity = (transform.position - coliders[i].transform.position).normalized * holeForce * Time.deltaTime * (1 - Vector3.Distance(transform.position, coliders[i].gameObject.transform.position)/radius);
                coliders[i].GetComponent<BaseEnemy>().staned = true;
            }
        }
        for(int i =0; i<inColiders.Length;i++)
        {
            if (inColiders[i] != null && inColiders[i].gameObject.GetComponent<BaseEnemy>() != null)
            {
                inColiders[i].GetComponent<BaseEnemy>().Death();
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
