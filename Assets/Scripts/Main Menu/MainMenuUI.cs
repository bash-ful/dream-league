using System.Collections;
using Firebase.Auth;
using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    public TMP_Text usernameText, dreamCoinAmountText, userIDText;


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

        usernameText.text = $"Welcome, {DataSaver.Instance.dts.userName}!";
        dreamCoinAmountText.text = $"Dream Coins: {DataSaver.Instance.dts.dreamCoinAmount}";
        userIDText.text = $"UID: {FirebaseAuth.DefaultInstance.CurrentUser.UserId}";
    }
}
