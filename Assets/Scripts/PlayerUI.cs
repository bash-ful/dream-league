using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public MonsterScript player;
    public Text playerHealth, playerName;

    void Start() {
        playerName.text = player.GetName();
    }
    void Update()
    {
        playerHealth.text = player.GetHealth().ToString();
    }
}
