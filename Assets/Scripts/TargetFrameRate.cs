using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    public int target = 60;

    // Update is called once per frame
    void Update()
    {
        if (target != Application.targetFrameRate) {
            Application.targetFrameRate = target;
        }
    }
}
