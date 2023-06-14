using PlayFab;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class dashboardManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject UIBlocker;
    public TextMeshProUGUI userName;

    public void Logout()
    {
        UIBlocker.SetActive(true);
        playfabManager.Instance.OnLogout(userSessionManager.Instance.profileUsername, userSessionManager.Instance.profileID, callbackLogoutSuccess, callbackLogoutFailure);
    }

    public void callbackLogoutSuccess()
    {

        SceneManager.LoadScene("Login Page");
        userSessionManager.Instance.resetSession();
        UIBlocker.SetActive(false);
    }

    public void callbackLogoutFailure(PlayFabError error)
    {
        userSessionManager.Instance.resetSession();
        UIBlocker.SetActive(false);
    }


    void Start()
    {
        userName.text = userSessionManager.Instance.profileUsername;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
