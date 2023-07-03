using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using System;
using System.Collections.Generic;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

#if UNITY_IOS
using UnityEngine.iOS;
#else
#endif

public class playfabManager : GenericSingletonClass<playfabManager>
{

    public void OnServerInitialized()
    {
        PlayGamesClientConfiguration config = new PlayGamesClientConfiguration.Builder()
        .AddOauthScope("profile")
        .RequestServerAuthCode(false)
        .Build();

        PlayGamesPlatform.InitializeInstance(config);
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();
    }

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
        // Implement logout functionality if required
    }

    public void OnSignGmail(Action callbackSuccess, Action<PlayFabError> callbackFailure, Action<string, string> callbackSuccessPlayfab, Action<PlayFabError> callbackFailurePlayfab)
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
                    OnTryLogin("player@gmail.com", "killprg1", callbackSuccessPlayfab, callbackFailurePlayfab);
                    callbackSuccess();
                });
            }
            else
            {
                callbackFailure(null);
            }
        });
    }

    #if UNITY_IOS
            public void OnSignIOS(Action callbackSuccess, Action<PlayFabError> callbackFailure, Action<string, string> callbackSuccessPlayfab, Action<PlayFabError> callbackFailurePlayfab)
        {
            Device.RequestStoreReview();
            if (Device.systemVersion.StartsWith("10"))
            {
                NativeAPI.Authorize((success) =>
                {
                    if (success)
                    {
                        OnTryLogin("player@gmail.com", "killprg1", callbackSuccessPlayfab, callbackFailurePlayfab);
                        callbackSuccess();
                    }
                    else
                    {
                        callbackFailure(null);
                    }
                });
            }
            else
            {
                callbackFailure(null);
            }
        }

        public static class NativeAPI
        {
            public delegate void SignInCallback(bool success);

            public static void Authorize(SignInCallback callback)
            {
                // Native iOS code for sign-in authorization
                // Call the callback with appropriate success value
                bool success = true; // Replace with your sign-in logic
                callback?.Invoke(success);
            }
        }
    #else
    #endif

    public void OnTryRegisterNewAccount(string email, string password, string fname, string lname, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        if(fname == null || fname.Equals(""))
        {
            fname = "abc";
        }
        RegisterPlayFabUserRequest req = new RegisterPlayFabUserRequest
        {
            Username = fname,
            DisplayName = fname,
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

    public void InitiatePasswordRecovery(string email, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        RegisterPlayFabUserRequest req = new RegisterPlayFabUserRequest
        {
            Email = email, // or Username = emailOrUsername
            TitleId = "9AA0E"
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

    public void onSubmitScore(int score)
    {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
            {
                new StatisticUpdate
                {
                    StatisticName = "headlight-leaderboard", // Replace with your actual leaderboard name
                    Value = score
                }
            }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
        res =>
        {
            Debug.Log("success");
        },
        err =>
        {
            Debug.Log("fail");
        });

    }

    public void FetchLeaderboard(Action<GetLeaderboardResult> callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "headlight-leaderboard",
            StartPosition = 0,
            MaxResultsCount = 41 // Fetch 6 entries to include yourself
        };

        PlayFabClientAPI.GetLeaderboard(request,
        res =>
        {
            callbackSuccess(res);
        },
        err =>
        {
            callbackFailure(err);
        });

    }

    private void OnLeaderboardData(GetLeaderboardResult result)
    {
        // Process the leaderboard data and update your UI
        foreach (var entry in result.Leaderboard)
        {
            Debug.Log("Player: " + entry.PlayFabId + ", Score: " + entry.StatValue);
        }
    }

    private void OnError(PlayFabError error)
    {
        // Handle error response from PlayFab API
        Debug.LogError("PlayFab Error: " + error.GenerateErrorReport());
    }

    public void OnSignInFacebook(Action callbackInitialized, Action callbackSuccess, Action<PlayFabError> callbackFailure)
    {
        // Implement sign in with Facebook functionality if required
    }

    void Start()
    {
    }

    void Update()
    {

    }
}
