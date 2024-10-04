using System;
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

    private IEnumerator InitializeUI()
    {
        while (string.IsNullOrEmpty(DataSaver.Instance.dts.userName))
        {
            yield return null;
        }

        // usernameText.text = $"Welcome, {DataSaver.Instance.dts.userName}!";
        userIDText.text = $"UID: {FirebaseAuth.DefaultInstance.CurrentUser.UserId}";
    }
}
