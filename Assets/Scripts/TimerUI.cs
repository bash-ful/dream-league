using UnityEngine;
using UnityEngine.UI;

public class TimerUI : MonoBehaviour
{
    // Start is called before the first frame update
    public Text DisplayedTimeText;
    // Update is called once per frame
    public void SetDisplayedTimeText(float timeInSeconds)
    {
        int seconds = (int)timeInSeconds % 60;
        int minutes = (int)timeInSeconds / 60;
        DisplayedTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
