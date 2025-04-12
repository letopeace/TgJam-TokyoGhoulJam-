using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorScript : MonoBehaviour
{
    public PlayerMagic playerMagic;


    public void UseMagic()
    {
        playerMagic.UseMagic();
    }
}
