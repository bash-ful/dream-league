using UnityEngine.SceneManagement;
using UnityEngine;

public class FirebaseInit : MonoBehaviour
{
    public static bool firebaseReady;

    void Start()
    {
        CheckIfReady();
    }

    void Update()
    {
        if(firebaseReady == true)
        {
            SceneManager.LoadScene(0);
        }
    }

    public static void CheckIfReady()
    {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            Firebase.DependencyStatus dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {

                Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;
                firebaseReady = true;
                Debug.Log("Firebase is ready for use.");
            }
            else
            {
                firebaseReady = false;
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });
    }
}