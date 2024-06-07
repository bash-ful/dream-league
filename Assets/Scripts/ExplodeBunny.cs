using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeBunny : MonoBehaviour
{

    public void Boom() {
        Animator anim = GetComponent<Animator>();
        anim.SetBool("isExplode", true);
    }
}
