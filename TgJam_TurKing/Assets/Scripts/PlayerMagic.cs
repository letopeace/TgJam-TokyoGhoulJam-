using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMagic : MonoBehaviour
{
    public int magicCharges;
    public float magicProgress;
    public int maxCharges;
    public GameObject magic;

    [SerializeField] private Animator animator;

    public void GetMagicProgress(float amount)
    {
        if (magicCharges == maxCharges) magicProgress = Mathf.Min(99f, magicProgress + amount);
        else
        {
            magicCharges += (int)(magicProgress + amount) / 100;
            magicProgress = (magicProgress + amount) % 100;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1)) AnimMagic();
    }

    void AnimMagic()
    {
        if(magicCharges != 0)
        {
            magicCharges -= 1;
            animator.SetTrigger("Magic");
        }
    }

    public void UseMagic()
    {
        Instantiate(magic, Camera.main.transform.position,Quaternion.identity);
    }
}
