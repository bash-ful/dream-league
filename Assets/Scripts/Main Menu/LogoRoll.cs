using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoRoll : MonoBehaviour
{

    private Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();
        changeToNotRolling();
    }

    public void Roll()
    {
        if (!anim.GetBool("isRolling"))
        {
            anim.SetTrigger("Roll");
        }
    }

    public void changeToRolling()
    {
        anim.SetBool("isRolling", true);
    }

    public void changeToNotRolling() {
        anim.SetBool("isRolling", false);
    }
}
