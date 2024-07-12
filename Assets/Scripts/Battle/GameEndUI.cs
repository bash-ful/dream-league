using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndUI : MonoBehaviour
{
    public void Win(){
        GameObject.Find("Win").SetActive(true);
    }

    public void Lose(){
        GameObject.Find("Lose").SetActive(true);
    }
}
