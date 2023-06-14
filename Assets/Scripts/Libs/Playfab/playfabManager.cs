using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;
using GooglePlayGames;
using Facebook.Unity;
using LoginResult = PlayFab.ClientModels.LoginResult;

public class playfabManager : GenericSingletonClass<playfabManager>
{

    private facebookManager _facebookManager = new facebookManager();
    private googleManager _googleManager = new googleManager();

    public void OnTryLogin(string email, string password, Action<string, string> callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        LoginWithEmailAddressRequest req = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(req,
        res =>
        {
            callbackSuccess(email, res.PlayFabId);
        },
        err =>
        {
            callbackFailure(err);
        });
    }

    public void OnLogout(string username, string playfabID, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        _facebookManager.OnLogout();
        callbackSuccess();
    }

    public void OnSignGmail(Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        _googleManager.OnSignGmail(callbackSuccess, callbackFailure);
    }

    public void OnTryRegisterNewAccount(string email, string password, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        RegisterPlayFabUserRequest req = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(req,
        res =>
        {
            callbackSuccess();
        },
        err =>
        {
            callbackFailure(err);
        });

    }

    public void OnSignInFacebook(Action callbackInitialized, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        _facebookManager.OnSignInFacebook(callbackInitialized, callbackSuccess, callbackFailure);
    }

    void Start()
    {
    }

    void Update()
    {
        
    }

}
