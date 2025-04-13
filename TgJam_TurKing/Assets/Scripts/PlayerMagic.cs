using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMagic : MonoBehaviour
{
    public int magicCharges;
    public int magicProgress;
    public int maxCharges;
    public GameObject magic;
    public GameObject thunderBolt;
    public Transform[] magicChargeImage;
    public Transform[] magicRecording;
    public static PlayerMagic instance;

    [SerializeField] private Animator animator;

    private GameObject selectedMagic;

    private void Awake()
    {
        instance = this;
    }

    public void GetMagicProgress(int amount)
    {
        if (magicCharges == maxCharges) magicProgress = Mathf.Min(4, magicProgress + amount);
        else
        {
            magicCharges += (int)(magicProgress + amount) / 5;
            magicProgress = (magicProgress + amount) % 5;
        }
        

    }

    void DisplayIcon()
    {
        for(int i=0; i< maxCharges; i++)
        {
            if (i < magicCharges && magicChargeImage[i].localScale == Vector3.zero) magicChargeImage[i].DOScale(Vector3.one, 1f).SetEase(Ease.InExpo);
            else if (i >= magicCharges && magicChargeImage[i].localScale == Vector3.one) magicChargeImage[i].DOScale(Vector3.zero, 1f).SetEase(Ease.InExpo);
        }
        for(int i = 0; i < 4; i++)
        {
            if (i < magicProgress && magicRecording[i].localScale == Vector3.zero) magicRecording[i].DOScale(Vector3.one, 1f).SetEase(Ease.InExpo);
            else if (i >= magicProgress && magicRecording[i].localScale == Vector3.one) magicRecording[i].DOScale(Vector3.zero, 1f).SetEase(Ease.InExpo);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AnimMagic();
            selectedMagic = magic;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            AnimMagic();
            selectedMagic = thunderBolt;
        }
        DisplayIcon();
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
        Instantiate(selectedMagic, Camera.main.transform.position, Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up));
    }
}
