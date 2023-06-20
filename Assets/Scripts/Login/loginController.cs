using UnityEngine;
using TMPro;
using PlayFab;
using EasyUI.Popup;
using UnityEngine.SceneManagement;

public class loginController : MonoBehaviour
{
    [Header("Utilities")]
    public GameObject UIBlocker;
    public GameObject RegisterUI;
    public GameObject LoginUI;


    [Header("Login Screen")]
    public TMP_InputField LoginEmailField;
    public TMP_InputField LoginPasswordField;

    [Header("Register Screen")]
    public TMP_InputField RegisterEmailField;
    public TMP_InputField RegisterPasswordwordField;
    


    public void OnTryLogin()
    {
        string email = LoginEmailField.text;
        string password = LoginPasswordField.text;

        UIBlocker.SetActive(true);
        playfabManager.Instance.OnTryLogin(email, password, callbackLoginSuccess, callbackLoginFailure);
    }

    public void OnDummyLogin()
    {
        SceneManager.LoadScene("gameplay");
    }

    public void OnTryRegisterNewAccount()
    {
        string email = RegisterEmailField.text;
        string password = RegisterPasswordwordField.text;

        UIBlocker.SetActive(true);
        playfabManager.Instance.OnTryRegisterNewAccount(email, password, callbackRegisterationSuccess, callbackRegisterationFailure);
    }

    public void callbackLoginSuccess(string email, string playfabID )
    {
        SceneManager.LoadScene("gameplay");
        userSessionManager.Instance.initialize(email, playfabID);
        UIBlocker.SetActive(false);
    }

    public void OnSignGmail()
    {
        UIBlocker.SetActive(true);
        playfabManager.Instance.OnSignGmail(callbackGmailSuccess, callbackGmailFailure);
    }

    public void OnSignFacebook()
    {
        UIBlocker.SetActive(true);
        playfabManager.Instance.OnSignInFacebook(callbackFacebookInitialized, callbackFacebookSuccess, callbackFacebookFailure);
    }

    public void callbackLoginFailure(PlayFabError error)
    {
        Popup.Show("Oops!", error.ErrorMessage, "Dismiss", PopupColor.Red);
        UIBlocker.SetActive(false);
    }

    public void callbackRegisterationSuccess()
    {
        SceneManager.LoadScene("gameplay");
        UIBlocker.SetActive(false);
    }

    public void callbackRegisterationFailure(PlayFabError error)
    {
        Popup.Show("Oops!", error.ErrorMessage, "Dismiss", PopupColor.Red);
        UIBlocker.SetActive(false);
    }

    public void callbackGmailSuccess()
    {
        userSessionManager.Instance.initialize("gmail_session", "none");
        SceneManager.LoadScene("gameplay");
        UIBlocker.SetActive(false);
    }

    public void callbackGmailFailure(PlayFabError error)
    {
        UIBlocker.SetActive(false);
        if (error == null)
        {
            Popup.Show("Oops!", "Initialization Failed", "Dismiss", PopupColor.Red);
        }
        else
        {
            Popup.Show("Oops!", error.ErrorMessage, "Dismiss", PopupColor.Red);
        }
    }

    public void callbackFacebookInitialized()
    {
        UIBlocker.SetActive(false);
    }

    public void callbackFacebookSuccess()
    {
        SceneManager.LoadScene("gameplay");
        userSessionManager.Instance.initialize("facebook_session", "none");
        UIBlocker.SetActive(false);
    }

    public void callbackFacebookFailure(PlayFabError error)
    {
        Popup.Show("Oops!", error.ErrorMessage, "Dismiss", PopupColor.Red);
        UIBlocker.SetActive(false);
    }

    public void onRegisterUI()
    {
        RegisterUI.SetActive(true);
        LoginUI.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {

    }

}
