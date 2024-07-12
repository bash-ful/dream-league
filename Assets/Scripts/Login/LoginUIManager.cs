using TMPro;
using UnityEngine;

public class LoginUIManager : MonoBehaviour
{
    #region variables
    [Header("Login")]
    public TMP_Text loginResultText;

    public TMP_InputField loginEmail;
    public TMP_InputField loginPassword;

    [Header("Register")]
    public TMP_Text registerResultText;
    public TMP_InputField registerEmail;
    public TMP_InputField registerUsername;
    public TMP_InputField registerPassword;
    public TMP_InputField registerPasswordConfirm;

    #endregion

    public void ShowRegisterTextResult(string message)
    {
        registerResultText.text = message;
    }

    public void ShowLoginTextResult(string message)
    {
        loginResultText.text = message;
    }

    public void ClearRegisterText()
    {
        registerEmail.text = "";
        registerUsername.text = "";
        registerPassword.text = "";
        registerPasswordConfirm.text = "";
    }
    public void ClearLoginText()
    {
        loginEmail.text = "";
        loginPassword.text = "";
    }

}
