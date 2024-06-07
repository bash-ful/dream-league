using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoSpin : MonoBehaviour
{
    public void Spin() {
        Animator anim = GetComponent<Animator>();
        anim.SetTrigger("isRoll");
    }
}
