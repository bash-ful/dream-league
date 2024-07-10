using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Dialogue
{
    public string name;
    public string dialogue;
    public Sprite characterImage, tutorialImage;
}

[System.Serializable]
public class DialogueList
{
    public Dialogue[] dialogueList;
}

public class DialogueScript : MonoBehaviour
{
    public DialogueList dialogueList;
    public TMP_Text NameText, DialogueText;
    public Image characterImage, tutorialImage;
    public int dialogueIndex = 0;

    public bool dialogueActive = false;

    public void BeginDialogue()
    {
        gameObject.SetActive(true);
        dialogueActive = true;
        StartCoroutine(PlayDialogue());
    }

    IEnumerator PlayDialogue()
    {
        foreach (Dialogue dialogue in dialogueList.dialogueList)
        {
            NameText.text = dialogue.name;
            DialogueText.text = dialogue.dialogue;
            characterImage.sprite = dialogue.characterImage;
            tutorialImage.sprite = dialogue.tutorialImage;

            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            yield return new WaitUntil(() => !Input.GetMouseButton(0));
            dialogueIndex++;
        }
        OnDialogueEnd();
    }

    public void OnDialogueEnd()
    {
        dialogueActive = false;
        gameObject.SetActive(false);
    }
}
