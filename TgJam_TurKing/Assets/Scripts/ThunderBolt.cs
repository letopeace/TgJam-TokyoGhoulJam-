using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBolt : MonoBehaviour
{
    public float speed;
    public int maxReflectNumber;
    public float damage;
    public float damagePerReflect;
    public float reflectRadius;
    public float decriseRadiusPerReflect;
    public GameObject thunderChain;
    public float livetime;
    [SerializeField] AudioClip thunderclip;
    private bool wondering = true;
    private int reflectNumber = 0;
    private void Update()
    {
        bool  isReflected = false;
        Collider[] coliders = Physics.OverlapSphere(transform.position, reflectRadius);
        for (int i = 0; i < coliders.Length; i++)
        {
            if (coliders[i] != null && coliders[i].gameObject.GetComponent<BaseEnemy>())
            {
                if (wondering)
                {
                    wondering = false;
                    GetComponent<AudioSource>().clip = thunderclip;
                    GetComponent<AudioSource>().Play();

                }
                isReflected = true;
                Reflect(coliders[i]);
            }
        }
        if (wondering) transform.Translate(Vector3.forward * speed * Time.deltaTime);
        else if (isReflected == false || maxReflectNumber == reflectNumber) Destroy(gameObject);

        livetime -= Time.deltaTime;
        if(livetime < 0f)
        {
            Destroy(gameObject);
        }
    }

    void Reflect(Collider i)
    {
        GameObject chain1 = Instantiate(thunderChain, transform.position, Quaternion.identity);
        GameObject chain2 = Instantiate(thunderChain, i.gameObject.transform.position, Quaternion.identity);
        chain1.transform.LookAt(i.transform.position, Vector3.up);
        chain2.transform.LookAt(transform.position, Vector3.up);
        i.GetComponent<BaseEnemy>().Damaged((int)(damage + damagePerReflect * reflectNumber));
        transform.position = i.transform.position;
        reflectNumber += 1;
        reflectRadius *= decriseRadiusPerReflect;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != null && other.gameObject.GetComponent<BaseEnemy>() == null && other.gameObject.GetComponent<PlayerMovement>() == null) Destroy(gameObject);
    }
}
