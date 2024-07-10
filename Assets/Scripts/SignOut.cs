using Firebase.Auth;
using UnityEngine;

public class SignOut : MonoBehaviour
{
    public SceneScript sceneScript;

    public void Logout()
    {
        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        auth.SignOut();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("DataSaver");
        foreach (GameObject obj in objs)
        {
            Destroy(obj);
        }
        sceneScript.MoveScene(0);
    }
}
