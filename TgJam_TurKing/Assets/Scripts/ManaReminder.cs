using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaReminder : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other != null && other.gameObject.GetComponent<PlayerMovement>() != null)
        {
            other.gameObject.GetComponent<PlayerMagic>().magicCharges = 2;
        }
    }
}
