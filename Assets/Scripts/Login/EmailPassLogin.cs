using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Extensions;
using Firebase.Auth;
using System;

public class EmailPassLogin : MonoBehaviour
{
    #region variables
    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;
    public GameObject mainMenuPanel;
    [Header("Login")]
    public InputField loginEmail;
    public InputField loginPassword;

    [Header("Register")]
    public InputField registerEmail;
    public InputField registerUsername;
    public InputField registerPassword;
    public InputField registerPasswordConfirm;

    [Header("Extra")]
    public GameObject loadingScreen;
    public Text loginText, registerText;
    public DataSaver dataSaver;
    public SceneScript sceneScript;

    #endregion



    #region register 
    public void Register()
    {
        string email = registerEmail.text;
        string password = registerPassword.text;
        string passwordConfirm = registerPasswordConfirm.text;
        if (!password.Equals(passwordConfirm))
        {
            ShowRegisterTextResult("Passwords do not match");
            return;
        }

        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            loadingScreen.SetActive(false);

            if (task.IsCanceled)
            {
                Debug.LogError("SignInWithEmailAndPasswordAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                if (CheckError(task.Exception, (int)AuthError.EmailAlreadyInUse))
                {
                    ShowRegisterTextResult("Email already in use");
                }
                Debug.LogError("UpdateEmailAsync encountered an error: " + task.Exception);
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);


            if (result.User.IsEmailVerified)
            {
                ShowLoginTextResult("Sign up successful");
            }
            else
            {
                ShowRegisterTextResult("Sending verification email...");
                SendEmailVerification();
            }

            dataSaver.dts.userName = registerUsername.text;
            dataSaver.SaveDataFn();
            dataSaver.dts.userName = "";

            ClearRegisterInputFields();
        });
    }

    public void SendEmailVerification()
    {
        StartCoroutine(SendEmailForVerificationAsync());
    }

    IEnumerator SendEmailForVerificationAsync()
    {
        FirebaseUser user = FirebaseAuth.DefaultInstance.CurrentUser;
        if (user != null)
        {
            Debug.Log("Sending verification email...");
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception != null)
            {
                // Debug.LogError("Failed to send verification email: " + sendEmailTask.Exception);
                ShowRegisterTextResult("Error in sending email verification");
            }
            else
            {
                ShowRegisterTextResult("Verification email sent successfully");
            }
        }
    }

    #endregion

    #region Login
    public void Login()
    {
        loadingScreen.SetActive(true);

        FirebaseAuth auth = FirebaseAuth.DefaultInstance;
        string email = loginEmail.text;
        string password = loginPassword.text;

        Credential credential = EmailAuthProvider.GetCredential(email, password);
        auth.SignInAndRetrieveDataWithCredentialAsync(credential).ContinueWithOnMainThread(task =>
        {
            loadingScreen.SetActive(false);

            if (task.IsCanceled)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAndRetrieveDataWithCredentialAsync encountered an error: " + task.Exception.ToString());
                ShowLoginTextResult("An error has occured!");
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                ShowLoginTextResult("Login Success!");
                dataSaver.LoadDataFn();
                sceneScript.MoveScene(1);
                // loginPanel.SetActive(false);
                // registerPanel.SetActive(false);
                // mainMenuPanel.SetActive(true);

            }
            else
            {
                ShowLoginTextResult("Please verify your e-mail!");
            }
        });
    }
    #endregion

    #region extra
    void ShowLoginTextResult(string msg)
    {
        loginText.text = msg;
    }

    void ShowRegisterTextResult(string msg)
    {
        registerText.text = msg;
    }

    void ClearRegisterInputFields()
    {
        registerEmail.text = "";
        registerUsername.text = "";
        registerPassword.text = "";
        registerPasswordConfirm.text = "";
    }

    bool CheckError(AggregateException exception, int firebaseExceptionCode)
    {
        Firebase.FirebaseException fbEx = null;
        foreach (Exception e in exception.Flatten().InnerExceptions)
        {
            fbEx = e as Firebase.FirebaseException;
            if (fbEx != null)
                break;
        }

        if (fbEx != null)
        {
            if (fbEx.ErrorCode == firebaseExceptionCode)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        return false;
    }
    #endregion
}