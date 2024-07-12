using UnityEngine;

public class TargetFrameRate : MonoBehaviour
{
    private const int TARGET_FRAMERATE = 60;

    // Update is called once per frame
    void Update()
    {
        if (TARGET_FRAMERATE != Application.targetFrameRate) {
            Application.targetFrameRate = TARGET_FRAMERATE;
        }
    }
}
