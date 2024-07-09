using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour
{
    public Text text;
    public Image image;

    #region Initialize


    void Awake()
    {
        FB.Init(SetInit, onHideUnity);

        if (!FB.IsInitialized)
        {
            FB.Init(() =>
            {
                if (FB.IsInitialized)
                {
                    FB.ActivateApp();
                }
                else
                {
                    print("Couldn't initialize");
                }
            },
            isGameShown =>
            {
                if (!isGameShown)
                {
                    Time.timeScale = 0;
                }
                else
                {
                    Time.timeScale = 1;
                }
            });
        }
        else
        {
            FB.ActivateApp();
        }
    }

    void SetInit()
    {
        if (FB.IsLoggedIn)
        {
            print("facebook login");
            string s = "client token" + FB.ClientToken + "User ID" + AccessToken.CurrentAccessToken.UserId;
        }
        else
        {
            print("not logged in");
        }
        DealWithFbMenus(FB.IsLoggedIn);
    }

    void onHideUnity(bool isGameShown)
    {
        if (!isGameShown) { Time.timeScale = 0; } else { Time.timeScale = 1; }
    }

    void DealWithFbMenus(bool isLoggedIn) {
        if(isLoggedIn) {
            FB.API("/me?fields=first_name", HttpMethod.GET, DisplayUsername);
            FB.API("/me/picture?type=square&height=128&width=128", HttpMethod.GET, DisplayProfilePic);
        } else {
            print("not logged in bro");
        }
    }

    public void DisplayUsername(IResult result) {
        if(result.Error == null) {
            string name = "" +result.ResultDictionary["first_name"];
            if (text != null) text.text = name;
            text.text = name;
            Debug.Log("" + name);
        } else {
            Debug.Log(result.Error);
        }
    }

    public void DisplayProfilePic(IGraphResult result) {
        if(result.Texture != null) {
            Debug.Log("Profile Pic");
            if (image != null) image.sprite = Sprite.Create(result.Texture, new Rect(0, 0, 128, 128), new Vector2());
        }
    }
    #endregion

     public void Facebook_LogIn() 
    {
        List<string> permissions = new List<string>();
        permissions.Add("public_profile");
        //permissions.Add("user_friends");
        FB.LogInWithReadPermissions(permissions, AuthCallBack);

    }
    void AuthCallBack(IResult result)
    {
        if (FB.IsLoggedIn)
        {
            SetInit();
            //AccessToken class will have session details
            var aToken = AccessToken.CurrentAccessToken;

            print(aToken.UserId);

            foreach (string perm in aToken.Permissions)
            {
                print(perm);
            }
        }
        else
        {
            print("Failed to log in");
        }

    }




    //logout
    public void Facebook_LogOut()
    {
        StartCoroutine(LogOut());
    }
    IEnumerator LogOut() {
        FB.LogOut();
        while (FB.IsLoggedIn)
        {
            print("Logging Out");
            yield return null;
        }
        print("Logout Successful");
       // if (FB_profilePic != null) FB_profilePic.sprite = null;
        if (text != null) text.text = "";
        if (image != null) image.sprite = null;
    }
}
