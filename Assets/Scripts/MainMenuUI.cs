using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Text usernameText, dreamCoinAmountText;


    void Start()
    {
        StartCoroutine(InitializeUI());
    }

    IEnumerator InitializeUI()
    {
        while (DataSaver.Instance.dts.userName == "")
        {
            yield return null;
        }

        usernameText.text = DataSaver.Instance.dts.userName;
        dreamCoinAmountText.text = DataSaver.Instance.dts.dreamCoinAmount.ToString();
    }
}
