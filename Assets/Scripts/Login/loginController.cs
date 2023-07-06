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
    public GameObject ForgotUI;
    public GameObject OnSignupGmail;
    public GameObject OnSignupApple;
    public GameObject video;
    public GameObject videosmall;


    [Header("Login Screen")]
    public TMP_InputField LoginEmailField;
    public TMP_InputField LoginPasswordField;

    [Header("Register Screen")]
    public TMP_InputField RegisterEmailFieldFirstName;
    public TMP_InputField RegisterEmailFieldLastName;
    public TMP_InputField RegisterEmailField;
    public TMP_InputField RegisterPasswordwordField;
    public TMP_InputField ForgotEmailField;


    private void Start()
    {
        playfabManager.Instance.OnServerInitialized();
        #if !UNITY_IOS
            OnSignupApple.SetActive(false);
        #else
        #endif

#if UNITY_EDITOR
    // Running in Unity Editor
    Debug.Log("Running in Unity Editor. Unable to determine device type.");
#elif UNITY_ANDROID
    // Running on Android device
    if (SystemInfo.deviceModel.Contains("pad") || SystemInfo.deviceModel.Contains("tablet"))
    {
        // Android tablet
	        videosmall.SetActive(true);
    }
    else
    {
        // Android phone
	        video.SetActive(true);
    }
#elif UNITY_IOS
    // Running on iOS device
    if (SystemInfo.deviceModel.Contains("iPad"))
    {
        // iPad
	        videosmall.SetActive(true);
    }
    else
    {
        // iPhone
	        video.SetActive(true);
    }
#else
    // Unsupported platform
	        videosmall.SetActive(true);
#endif

    }

    public void OnTryLogin()
    {
        //SceneManager.LoadScene("gameplay");

        string email = LoginEmailField.text;
        string password = LoginPasswordField.text;
        PlayFabSettings.TitleId = "9AA0E";
        UIBlocker.SetActive(true);
        playfabManager.Instance.OnTryLogin(email, password, callbackLoginSuccess, 			callbackLoginFailure);
    }

    public void OnPrivacyPolicy()
    {
        Application.OpenURL("https://366degreefit.com/privacy-policy");
    }

    public void OnDummyLogin()
    {
        SceneManager.LoadScene("gameplay");
    }

    public void OnTryRegisterNewAccount()
    {
        string email = RegisterEmailField.text;
        string password = RegisterPasswordwordField.text;
        string fname = RegisterEmailFieldFirstName.text;
        string lname = RegisterEmailFieldLastName.text;

        UIBlocker.SetActive(true);
        playfabManager.Instance.OnTryRegisterNewAccount(email, password, fname, lname, callbackRegisterationSuccess, callbackRegisterationFailure);
    }

    public void showForgotUI()
    {
        ForgotUI.SetActive(true);
    }

    public void closeRegister()
    {
        LoginUI.SetActive(true);
        RegisterUI.SetActive(false);
    }

    public void onForgotPassword()
    {
        UIBlocker.SetActive(true);
        string email = ForgotEmailField.text;
        playfabManager.Instance.InitiatePasswordRecovery(email, callbackForgotSuccess, callbackForgotFailure);
    }

    public void callbackLoginSuccess(string email, string playfabID)
    {
        SceneManager.LoadScene("gameplay");
        userSessionManager.Instance.initialize(email, playfabID);
        UIBlocker.SetActive(false);
    }

    public void closeResetPassword()
    {
        ForgotUI.SetActive(false);
        LoginUI.SetActive(true);
    }

    public void OnSignGmail()
    {
	#if !UNITY_IOS
        UIBlocker.SetActive(true);
        playfabManager.Instance.OnSignGmail(callbackGmailSuccess, callbackGmailFailure, 	callbackLoginSuccess, callbackLoginFailure);
	#else
	#endif
    }

    public void OnSignIOS()
    {
        #if UNITY_IOS
           UIBlocker.SetActive(true);
           playfabManager.Instance.OnSignIOS(callbackGmailSuccess, callbackGmailFailure, callbackLoginSuccess, callbackLoginFailure);
        #else
        #endif
    }

    public void OnSignFacebook()
    {
        UIBlocker.SetActive(true);
        Popup.Show("Password reset", "check your email address", "Dismiss");
        UIBlocker.SetActive(false);
    }

    public void callbackLoginFailure(PlayFabError error)
    {
        Popup.Show("Oops!", error.ErrorMessage, "Dismiss");
        UIBlocker.SetActive(false);
    }

    public void callbackRegisterationSuccess()
    {
        SceneManager.LoadScene("gameplay");
        UIBlocker.SetActive(false);
    }

    public void callbackRegisterationFailure(PlayFabError error)
    {
        Popup.Show("Oops!", error.ErrorMessage, "Dismiss");
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
            Popup.Show("Oops!", "Initialization Failed", "Dismiss");
        }
        else
        {
            Popup.Show("Oops!", error.ErrorMessage, "Dismiss");
        }
    }

    public void callbackForgotSuccess()
    {
        LoginUI.SetActive(true);
        ForgotUI.SetActive(false);
        Popup.Show("Email Sent", "please check your email for further instruction", "Dismiss");
        UIBlocker.SetActive(false);
    }

    public void callbackForgotFailure(PlayFabError error)
    {
        UIBlocker.SetActive(false);
        ForgotEmailField.text = "";
        if (error == null)
        {
            Popup.Show("Oops!", "Initialization Failed", "Dismiss");
        }
        else
        {
            Popup.Show("Oops!", error.ErrorMessage, "Dismiss");
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
        Popup.Show("Oops!", error.ErrorMessage, "Dismiss");
        UIBlocker.SetActive(false);
    }

    public void onRegisterUI()
    {
        RegisterUI.SetActive(true);
        LoginUI.SetActive(false);
    }

    void Update()
    {

    }

}
