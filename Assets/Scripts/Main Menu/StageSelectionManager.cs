using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class StageSelectButton
{
    public Button stageSelectButton;
    public TMP_Text stageIdText;

    public int stageID;
}
[System.Serializable]
public class StageSelectButtons
{
    public StageSelectButton[] stageSelectButtons;
}
public class StageSelectionManager : MonoBehaviour
{
    public StageSelectButtons stageSelectButtons;
    public GameObject StageSelectionPanel;
    public SceneScript sceneScript;

    public void ShowStageSelector(int locationIndex)
    {
        int stageSelectButtonsCount = stageSelectButtons.stageSelectButtons.Length;
        int stageNumber;
        for (int i = 0; i < stageSelectButtonsCount; i++)
        {
            stageNumber = i + (4 * locationIndex) + 1;
            stageSelectButtons.stageSelectButtons[i].stageID = stageNumber;
            stageSelectButtons.stageSelectButtons[i].stageIdText.text = stageNumber.ToString();
        }
        StageSelectionPanel.SetActive(true);
    }

    public void OnStageButtonClick(int buttonIndex)
    {
        sceneScript.MoveScene(stageSelectButtons.stageSelectButtons[buttonIndex].stageID + 1);
    }
}
