using PlayFab;
using System;
using GooglePlayGames;
using PlayFab.ClientModels;
using UnityEngine;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class googleManager
{

    public googleManager()
    {
        /*PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = true;
        PlayGamesPlatform.Activate();*/
    }

    public void OnSignGmail(Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        Social.localUser.Authenticate((bool success) => {
            if (success)
            {
                var serverAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
                PlayFabClientAPI.LoginWithGoogleAccount(new LoginWithGoogleAccountRequest()
                {
                    TitleId = "9AA0E",
                    ServerAuthCode = serverAuthCode, 
                    CreateAccount = true
                },
                res =>
                {
                    callbackSuccess();
                },
                err =>
                {
                    callbackFailure(err);
                });
            }
            else
            {
                callbackFailure(null);
            }
        });
    }
}
