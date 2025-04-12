
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
    public float exploinDamage;
    public GameObject exploinParticals;
    private void Update()
    {
        transform.Translate(transform.forward * speed * Time.deltaTime);

        Collider[] coliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < coliders.Length; i++)
        {
            if (coliders[i] != null && coliders[i].gameObject.GetComponent<BaseEnemy>() != null)
            {
                coliders[i].GetComponent<Rigidbody>().velocity = (transform.position - coliders[i].transform.position).normalized * holeForce * Time.deltaTime * (1 - Vector3.Distance(transform.position, coliders[i].gameObject.transform.position) / radius);
                coliders[i].GetComponent<BaseEnemy>().staned = true;
            }
        }
        if (lifeTime < 0f) Exploin();
        lifeTime -= Time.deltaTime;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null && other.gameObject.GetComponent<BaseEnemy>() != null)
        {
            other.gameObject.GetComponent<BaseEnemy>().Death();
        }
        else if (other.gameObject != null && other.gameObject.GetComponent<PlayerMovement>() == null)
        {
            Exploin();
        }
    }

    void Exploin()
    {
        Collider[] coliders = Physics.OverlapSphere(transform.position, radius);
        for (int i = 0; i < coliders.Length; i++)
        {
            if (coliders[i] != null && coliders[i].gameObject.GetComponent<BaseEnemy>() != null)
            {
                coliders[i].gameObject.GetComponent<BaseEnemy>().Damaged((int)(exploinDamage * (1f - (Vector3.Distance(transform.position, coliders[i].transform.position) / radius))));
            }
        }
        Instantiate(exploinParticals, transform.position, Quaternion.identity);
        Destroy(gameObject);

    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

}
