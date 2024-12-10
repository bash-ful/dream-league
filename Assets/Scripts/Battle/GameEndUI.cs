using UnityEngine;

// I want to kill myself
public class GameEndUI : MonoBehaviour
{
    public void Win(){
        GameObject.Find("Win").SetActive(true);
    }

    public void Lose(){
        GameObject.Find("Lose").SetActive(true);
    }
}
