using System.Collections;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Auth;
using System;
using Firebase.Database;
using TMPro;

public class EmailPassLogin : MonoBehaviour
{
    #region variables
    [Header("Login")]
    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Register")]
    public TMP_InputField registerEmail;
    public TMP_InputField registerUsername;
    public TMP_InputField registerPassword;
    public TMP_InputField registerPasswordConfirm;

    [Header("Extra")]
    public GameObject loadingScreen;
    public DataSaver dataSaver;
    public SceneScript sceneScript;
    public LoginUIManager uiManager;

    #endregion

    #region register 
    public void Register()
    {
        string email = registerEmail.text;
        string username = registerUsername.text;
        string password = registerPassword.text;
        string passwordConfirm = registerPasswordConfirm.text;

        if (!password.Equals(passwordConfirm))
        {
            uiManager.ShowRegisterTextResult("Passwords do not match");
            return;
        }

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordConfirm) || string.IsNullOrEmpty(username))
        {
            uiManager.ShowRegisterTextResult("Please fill all fields");
            return;
        }

        loadingScreen.SetActive(true);

        CheckUsernameExists(username, (exists) =>
        {
            if (exists)
            {
                loadingScreen.SetActive(false);
                uiManager.ShowRegisterTextResult("Username already exists");
                return;
            }

            CreateFirebaseUser(email, password, username);
        });
    }

    private void CreateFirebaseUser(string email, string password, string username)
    {
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
                HandleAuthRegisterError(task.Exception);
                return;
            }

            AuthResult result = task.Result;
            Debug.LogFormat("Firebase user created successfully: {0} ({1})", result.User.DisplayName, result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                uiManager.ShowLoginTextResult("Sign up successful");
            }
            else
            {
                uiManager.ShowRegisterTextResult("Sending verification email...");
                SendEmailVerification();
            }

            dataSaver.dts.userName = username;
            dataSaver.SaveDataFn();
            dataSaver.dts.userName = "";

            uiManager.ClearRegisterText();
        });
    }

    private void HandleAuthRegisterError(AggregateException exception)
    {
        int errorCode = GetErrorCode(exception);
        string errorDisplay = "An error occurred!";
        switch (errorCode)
        {
            case (int)AuthError.InvalidEmail:
                errorDisplay = "Invalid email";
                break;
            case (int)AuthError.EmailAlreadyInUse:
                errorDisplay = "This email is already in use";
                break;
            case (int)AuthError.WeakPassword:
                errorDisplay = "Password must be at least 6 characters long";
                break;
        }
        uiManager.ShowRegisterTextResult(errorDisplay);
        Debug.LogError("UpdateEmailAsync encountered an error: " + exception);
    }

    private void CheckUsernameExists(string username, Action<bool> callback)
    {
        FirebaseDatabase.DefaultInstance.GetReference("users").OrderByChild("userName").EqualTo(username).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error checking username: " + task.Exception);
                callback(false);
                return;
            }

            DataSnapshot snapshot = task.Result;
            callback(snapshot.Exists);
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
            var sendEmailTask = user.SendEmailVerificationAsync();
            yield return new WaitUntil(() => sendEmailTask.IsCompleted);

            if (sendEmailTask.Exception != null)
            {
                uiManager.ShowRegisterTextResult("Error in sending email verification");
            }
            else
            {
                uiManager.ShowRegisterTextResult("Verification email sent successfully");
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
                HandleAuthLoginError(task.Exception);
            }

            AuthResult result = task.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})",
                result.User.DisplayName, result.User.UserId);

            if (result.User.IsEmailVerified)
            {
                uiManager.ShowLoginTextResult("Login Success!");
                dataSaver.LoadDataFn();
                sceneScript.MoveScene(1);

            }
            else
            {
                uiManager.ShowLoginTextResult("Verify your e-mail");
            }
        });
    }

    private void HandleAuthLoginError(AggregateException exception)
    {
        int errorCode = GetErrorCode(exception);
        string errorDisplay;
        switch (errorCode)
        {
            case (int)AuthError.InvalidEmail:
                errorDisplay = "Invalid email";
                break;
            case (int)AuthError.MissingEmail:
                errorDisplay = "Enter your email on the field";
                break;
            case (int)AuthError.UnverifiedEmail:
                errorDisplay = "Please verify your email";
                break;
            case (int)AuthError.MissingPassword:
                errorDisplay = "Enter your password on the field";
                break;
            default:
                errorDisplay = "An error has occured";
                break;

        }
        uiManager.ShowLoginTextResult(errorDisplay);
        return;
    }
    #endregion

    #region extra
    int GetErrorCode(AggregateException exception)
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
            return fbEx.ErrorCode;
        }
        return 0; // Return -1 or another value to indicate no FirebaseException was found
    }
    #endregion
}
