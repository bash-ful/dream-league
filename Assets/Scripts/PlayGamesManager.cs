using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.UI;

public class PlayGamesManager : MonoBehaviour
{
    public Text StatusText;

    public void SignIn()
    {
        PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
    }

    internal void ProcessAuthentication(SignInStatus status)
    {
        if (status == SignInStatus.Success)
        {
            string name = PlayGamesPlatform.Instance.GetUserDisplayName();
            string id = PlayGamesPlatform.Instance.GetUserId();

            StatusText.text = $"Welcome, {name}! USER_ID: {id}!";
        }
        else
        {
            StatusText.text = "unsuccessful";
        }
    }
}
