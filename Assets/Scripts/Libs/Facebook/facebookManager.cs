using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;
using GooglePlayGames;
using Facebook.Unity;
using LoginResult = PlayFab.ClientModels.LoginResult;

public class facebookManager
{

    Action callbackInitiaized;
    Action callbackSuccess;
    Action<PlayFabError> callbackFailure;
    bool initialized = false;

    public void OnSignInFacebook(Action callbackInitiaized, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        this.callbackInitiaized = callbackInitiaized;
        this.callbackSuccess = callbackSuccess;
        this.callbackFailure = callbackFailure;

        if (initialized)
        {
            FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
        }
        else
        {
            initialized = true;
            FB.Init(OnFacebookInitialized);
        }
    }

    void OnFacebookInitialized()
    {
        FB.LogInWithReadPermissions(null, OnFacebookLoggedIn);
    }

    public void OnLogout()
    {
        if (initialized)
        {
            FB.LogOut();
        }
    }

    private void OnFacebookLoggedIn(ILoginResult result)
    {
        callbackInitiaized();
        initialized = true;
        if (result == null || string.IsNullOrEmpty(result.Error))
        {
            PlayFabClientAPI.LoginWithFacebook(new LoginWithFacebookRequest { CreateAccount = true, AccessToken = AccessToken.CurrentAccessToken.TokenString }, OnPlayfabFacebookAuthComplete, OnPlayfabFacebookAuthFailed);
        }
        else
        {
            callbackFailure(null);
        }
    }

    private void OnPlayfabFacebookAuthComplete(LoginResult result)
    {
        callbackSuccess();
    }

    private void OnPlayfabFacebookAuthFailed(PlayFabError error)
    {
        callbackFailure(error);
    }


}
