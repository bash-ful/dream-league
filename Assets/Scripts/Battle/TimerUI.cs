using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    public TMP_Text DisplayedTimeText;
    public void SetDisplayedTimeText(float timeInSeconds)
    {
        int seconds = (int)timeInSeconds % 60;
        int minutes = (int)timeInSeconds / 60;
        DisplayedTimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
