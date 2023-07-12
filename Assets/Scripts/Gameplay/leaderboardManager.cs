using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class leaderboardManager : MonoBehaviour
{

    public GameObject leaderboard;
    public List<TextMeshProUGUI> p1;
    public List<TextMeshProUGUI> f1;
    public List<TextMeshProUGUI> id;
    public GameObject imageBlocker;


    public void onBack()
    {
        leaderboard.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
    }


    private void OnEnable()
    {
        imageBlocker.SetActive(true);
        playfabManager.Instance.FetchLeaderboard(callbackSuccess, callbackFailure);
    }

    public void callbackSuccess(GetLeaderboardResult result)
    {
        imageBlocker.SetActive(false);
        for (int i = 0; i < result.Leaderboard.Count-1; i++)
        {
            var entry = result.Leaderboard[i];
            string playerName = entry.DisplayName;
            if(playerName == null)
            {
                playerName = "player";
            }
            int playerScore = entry.StatValue;

            p1[i].text = playerName;
            f1[i].text = playerScore + "";

            // Do something with the player name and score
            Debug.Log("Player: " + playerName + ", Score: " + playerScore);
        }
    }

    public void callbackFailure(PlayFabError error)
    {
        imageBlocker.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
