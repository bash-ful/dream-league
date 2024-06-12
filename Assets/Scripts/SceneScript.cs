using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneScript : MonoBehaviour
{
    public void MoveScene(int sceneID) {
        SceneManager.LoadScene(sceneID);
    }

    public void QuitGame(){
        Application.Quit();
    }
}
