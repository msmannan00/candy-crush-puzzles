#if UNITY_EDITOR1
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.Windows.Speech;
using UnityEngine.UI;

public class gamePlayManager : MonoBehaviour
{
    public Light2D light2D;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI stageText;
    public GameObject leaderboard;
    public GameObject failPopupObject;
    public GameObject successPopupObject;
    public GameObject completePopupObject;
    public GameObject sidemenu;
    public GameObject pausemenu;
    public GameObject pausebutton;
    public GameObject menuicon;
    public AudioSource theme;
    public AudioSource button;
    public List<GameObject> correctOption;


    private DictationRecognizer recognizer;
    private Coroutine countDownCoroutine = null;
    private bool sessionStarted = false;
    private List<string> sessionResult = new List<string>();
    private List<List<string>> currentLevelLightCombinations = new List<List<string>>();
    private int currentCombinationIndex = 0;
    private int currentCombinationIndexLevel = 1;
    private List<string> currentCombination;
    public GameObject backgroundShadow;
    public GameObject mike;
    public GameObject defaultDot;
    public GameObject playButton;
    bool mGamePaused = false;

    private void Start()
    {
        initializeRecognizer();
        theme.Play();
    }

    public void onPauseGame()
    {
        backgroundShadow.SetActive(false);
        pausemenu.SetActive(true);
        mGamePaused = true;
    }

    public void onResumeGame()
    {
        pausemenu.SetActive(false);
        mGamePaused = false;
        if (!sessionStarted)
        {
            backgroundShadow.SetActive(true);
            light2D.color = nextColor;
        }
    }

    public void showLeaderBoard()
    {
        backgroundShadow.SetActive(false);
        sidemenu.SetActive(false);
        leaderboard.SetActive(true);
    }

    public void hideLeaderBoard()
    {
        leaderboard.SetActive(false);
        backgroundShadow.SetActive(true);
    }

    private void animatePlay()
    {
        // Define the wobble parameters
        float wobbleDuration = 0.5f;
        float wobbleAngle = 10f;
        int wobbleVibrato = 10;

        // Define the scale parameters
        float scaleDuration = 0.5f;
        Vector3 scaleInScale = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 scaleOutScale = new Vector3(1f, 1f, 1f);

        // Apply the wobble and scale animations
        LeanTween.rotateZ(playButton, wobbleAngle, wobbleDuration)
            .setLoopPingPong(wobbleVibrato)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.scale(playButton, scaleInScale, scaleDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(playButton, scaleOutScale, scaleDuration)
                            .setEase(LeanTweenType.easeInOutSine)
                            .setOnComplete(animatePlay);
                    });
            });
    }

    private void initializeRecognizer()
    {
        recognizer = new DictationRecognizer();
        recognizer.AutoSilenceTimeoutSeconds = 20f;
        recognizer.InitialSilenceTimeoutSeconds = 20f;
        recognizer.DictationResult += OnVoiceCapture;
        recognizer.AutoSilenceTimeoutSeconds = 20f;
        recognizer.InitialSilenceTimeoutSeconds = 20f;
        recognizer.DictationComplete += OnDictationComplete;
        recognizer.DictationError += OnDictationError;
        recognizer.Start();
    }

    public void onPlay()
    {
        button.Play();
        backgroundShadow.SetActive(true);
        initializeLevel();
        playButton.SetActive(false);
        pausebutton.SetActive(true);
        theme.Stop();
        defaultDot.SetActive(false);
    }

    private void initializeLevel()
    {
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 3, 33));
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 5, 33));
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 7, 33));
        currentCombination = currentLevelLightCombinations[currentCombinationIndexLevel - 1];
        StartCoroutine(SwitchColorsCoroutine());
    }

    Color nextColor = Color.black;
    private IEnumerator SwitchColorsCoroutine()
    {
        menuicon.SetActive(false);
        mike.SetActive(false);

        float fadeDuration = 0.2f;
        float blackDuration = 1f;
        currentCombinationIndex = 0;

        // Loop through each color in the combination
        for (int i = 0; i < currentCombination.Count; i++)
        {
            mike.SetActive(true);
            backgroundShadow.SetActive(true);
            // Switch to the next color
            nextColor = GetNextColor();
            yield return FadeColor(light2D.color, nextColor, fadeDuration);
            yield return new WaitForSeconds(blackDuration);
            if (i != currentCombination.Count - 1)
            {
                yield return FadeColor(light2D.color, GetDarkerTone(nextColor), 0.2f);
            }
            if (mGamePaused)
            {
                while (mGamePaused)
                {
                    backgroundShadow.SetActive(false);
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                yield return new WaitForSeconds(blackDuration);
                if (i != currentCombination.Count - 1)
                {
                    yield return FadeColor(light2D.color, GetDarkerTone(nextColor), 0.2f);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        backgroundShadow.SetActive(false);
        yield return FadeColor(light2D.color, Color.black, 0.2f);
        sessionStarted = true;
        countDownCoroutine = StartCoroutine(Countdown());
        mike.SetActive(true);
    }


    public Color GetDarkerTone(Color color)
    {
        if (color == Color.red)
        {
            // Return a darker shade of red
            return new Color(0.5f, 0f, 0f);
        }
        else if (color == Color.green)
        {
            // Return a darker shade of green
            return new Color(0f, 0.5f, 0f);
        }
        else if (color == Color.blue)
        {
            // Return a darker shade of blue
            return new Color(0f, 0f, 0.5f);
        }
        else if (color == Color.white)
        {
            // Return a darker shade of white
            return new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            // If the color is not supported, return the original color
            return color;
        }
    }

    private Color GetNextColor()
    {
        // Get the next color in the combination
        Color targetColor = helperMethods.GetInstance().GetColorFromString(currentCombination[currentCombinationIndex]);

        // Increment the index for the next iteration
        currentCombinationIndex++;


        return targetColor;
    }

    private IEnumerator FadeColor(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            light2D.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    private IEnumerator Countdown()
    {
        int countdownValue = ((currentCombinationIndexLevel / 33) + 1) * 5;

        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1f);
            if (!mGamePaused)
            {
                countdownValue--;
            }
        }

        yield return new WaitForSeconds(1f);
        if (sessionStarted)
        {
            onGameEnd();
        }
    }

    private void onGameEnd()
    {
        int level = ((currentCombinationIndexLevel / 33) + 1);
        int start = 0;
        int end = 0;
        if (level == 1)
        {
            start = 2;
            end = 4;
        }
        else if (level == 2)
        {
            start = 1;
            end = 5;
        }
        else
        {
            start = 0;
            end = 6;
        }

        int index = 0;
        for (int e = start; e <= end; e++)
        {
            Color newColor = helperMethods.GetInstance().GetColorFromString(currentCombination[index]);
            correctOption[e].GetComponent<Image>().color = newColor;
            correctOption[e].SetActive(true);
            index++;
        }

        failPopupObject.SetActive(true);
        backgroundShadow.SetActive(false);
        sessionStarted = false;
        menuicon.SetActive(true);
        pausebutton.SetActive(false);
    }

    private void OnVoiceCapture(string text, ConfidenceLevel confidence)
    {
        string[] words = Regex.Split(text.ToLower(), @"\W+");

        if (sessionStarted)
        {
            string[] allowedWords = { "red", "green", "blue", "white" };
            words = words.Where(w => allowedWords.Contains(w)).ToArray();

            sessionResult = words.ToList();
            bool equal = sessionResult.SequenceEqual(currentLevelLightCombinations[currentCombinationIndexLevel - 1]);
            if (sessionResult.Count == currentLevelLightCombinations[currentCombinationIndexLevel - 1].Count && equal)
            {
                Debug.Log("Result: " + currentCombinationIndex);
                if (currentCombinationIndexLevel >= 99)
                {
                    currentCombinationIndex = 0;
                    currentCombinationIndexLevel = 0;
                    completePopupObject.SetActive(true);
                }
                if (countDownCoroutine != null)
                {
                    StopCoroutine(countDownCoroutine);
                    successPopupObject.SetActive(true);
                }
                playfabManager.Instance.onSubmitScore(currentCombinationIndexLevel + 1);
                menuicon.SetActive(true);
                pausebutton.SetActive(false);
            }
            else
            {
                onGameEnd();
            }
        }
    }

    public void nextLevel()
    {
        button.Play();
        failPopupObject.SetActive(false);
        completePopupObject.SetActive(false);
        successPopupObject.SetActive(false);
        backgroundShadow.SetActive(true);
        sessionStarted = false;
        sessionResult.Clear();
        levelText.text = "Level " + ((currentCombinationIndexLevel / 33) + 1) + ":";
        stageText.text = "Stage " + ((currentCombinationIndexLevel % 33) + 1);
        currentCombinationIndexLevel += 1;
        currentCombination = currentLevelLightCombinations[currentCombinationIndexLevel - 1];
        StartCoroutine(SwitchColorsCoroutine());
        StopCoroutine(countDownCoroutine);
    }

    public void openSideMenu()
    {
        backgroundShadow.SetActive(false);
        sidemenu.SetActive(true);
    }

    public void closeSideMenu()
    {
        sidemenu.SetActive(false);
    }

    /*Helper methods*/

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        recognizer.Stop();
        recognizer.Start();
    }

    private void OnDictationError(string error, int hresult)
    {
        recognizer.Stop();
        recognizer.Start();
    }

    public void RestartScene()
    {
        button.Play();
        helperMethods.GetInstance().RestartScene("gameplay");
    }

    public void logout()
    {
        helperMethods.GetInstance().RestartScene("login");
    }

    public void RestartAndPlay()
    {
        button.Play();
        for (int e = 0; e < 7; e++)
        {
            correctOption[e].SetActive(false);
        }

        failPopupObject.SetActive(false);
        completePopupObject.SetActive(false);
        successPopupObject.SetActive(false);
        sessionStarted = false;
        StopCoroutine(countDownCoroutine);
        onPlay();
    }

    private void OnDestroy()
    {
        if (recognizer != null && recognizer.Status == SpeechSystemStatus.Running)
        {
            recognizer.Stop();
            recognizer.Dispose();
        }
    }
}
#else
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.UI;
using TextSpeech;
using UnityEngine;
using UnityEngine.UI;
using TextSpeech;
using UnityEngine.Android;

public class gamePlayManager : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D light2D;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI stageText;
    public GameObject leaderboard;
    public GameObject failPopupObject;
    public GameObject successPopupObject;
    public GameObject completePopupObject;
    public GameObject sidemenu;
    public GameObject pausemenu;
    public GameObject pausebutton;
    public AudioSource theme;
    public AudioSource button;
    public List<GameObject> correctOption;

    private AndroidJavaObject speechRecognizer;
    private Coroutine countDownCoroutine = null;
    private bool sessionStarted = false;
    private List<string> sessionResult = new List<string>();
    private List<List<string>> currentLevelLightCombinations = new List<List<string>>();
    private int currentCombinationIndex = 0;
    private int currentCombinationIndexLevel = 1;
    private List<string> currentCombination;
    public GameObject backgroundShadow;
    public GameObject mike;
    public GameObject defaultDot;
    public GameObject playButton;
    private AndroidJavaObject speechRecognitionManager;
    public GameObject menuicon;
    bool mGamePaused = false;

    private void Start()
    {
        Setting("en-US");
        SpeechToText.Instance.onResultCallback = OnVoiceCapture;
        SpeechToText.Instance.isShowPopupAndroid = true;
        Permission.RequestUserPermission(Permission.Microphone);
        theme.Play();
    }

    public void onPauseGame()
    {
        backgroundShadow.SetActive(false);
        pausemenu.SetActive(true);
        mGamePaused = true;
    }

    public void onResumeGame()
    {
        pausemenu.SetActive(false);
        mGamePaused = false;
        if (!sessionStarted)
        {
            backgroundShadow.SetActive(true);
            light2D.color = nextColor;
        }
    }

    public void showLeaderBoard()
    {
        sidemenu.SetActive(false);
        leaderboard.SetActive(true);
    }

    public void hideLeaderBoard()
    {
        leaderboard.SetActive(false);
    }

    public void Setting(string code)
    {
        SpeechToText.Instance.Setting(code);
    }

    private void animatePlay()
    {
        // Define the wobble parameters
        float wobbleDuration = 0.5f;
        float wobbleAngle = 10f;
        int wobbleVibrato = 10;

        // Define the scale parameters
        float scaleDuration = 0.5f;
        Vector3 scaleInScale = new Vector3(1.2f, 1.2f, 1.2f);
        Vector3 scaleOutScale = new Vector3(1f, 1f, 1f);

        // Apply the wobble and scale animations
        LeanTween.rotateZ(playButton, wobbleAngle, wobbleDuration)
            .setLoopPingPong(wobbleVibrato)
            .setEase(LeanTweenType.easeInOutSine)
            .setOnComplete(() =>
            {
                LeanTween.scale(playButton, scaleInScale, scaleDuration)
                    .setEase(LeanTweenType.easeInOutSine)
                    .setOnComplete(() =>
                    {
                        LeanTween.scale(playButton, scaleOutScale, scaleDuration)
                            .setEase(LeanTweenType.easeInOutSine)
                            .setOnComplete(animatePlay);
                    });
            });
    }

    private void InitializeSpeechRecognizer()
    {
        SpeechToText.Instance.isShowPopupAndroid = true;
        Permission.RequestUserPermission(Permission.Microphone);
    }

    public void onPlay()
    {
        button.Play();
        backgroundShadow.SetActive(true);
        initializeLevel();
        playButton.SetActive(false);
        pausebutton.SetActive(true);
        theme.Stop();
        defaultDot.SetActive(false);
    }

    private void initializeLevel()
    {
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 3, 33));
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 5, 33));
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 7, 33));
        currentCombination = currentLevelLightCombinations[currentCombinationIndexLevel - 1];
        StartCoroutine(SwitchColorsCoroutine());
    }

    Color nextColor = Color.black;
    private IEnumerator SwitchColorsCoroutine()
    {
        menuicon.SetActive(false);
        mike.SetActive(false);

        float fadeDuration = 0.2f;
        float blackDuration = 1f;
        currentCombinationIndex = 0;

        // Loop through each color in the combination
        for (int i = 0; i < currentCombination.Count; i++)
        {
            mike.SetActive(false);
            backgroundShadow.SetActive(true);
            // Switch to the next color
            nextColor = GetNextColor();
            yield return FadeColor(light2D.color, nextColor, fadeDuration);
            yield return new WaitForSeconds(blackDuration);
            if (i != currentCombination.Count - 1)
            {
                yield return FadeColor(light2D.color, GetDarkerTone(nextColor), 0.2f);
            }
            if (mGamePaused)
            {
                while (mGamePaused)
                {
                    backgroundShadow.SetActive(false);
                    yield return new WaitForSeconds(0.5f);
                    continue;
                }
                yield return new WaitForSeconds(blackDuration);
                if (i != currentCombination.Count - 1)
                {
                    yield return FadeColor(light2D.color, GetDarkerTone(nextColor), 0.2f);
                }
            }
        }

        yield return new WaitForSeconds(0.5f);
        backgroundShadow.SetActive(false);
        yield return FadeColor(light2D.color, Color.black, 0.2f);
        sessionStarted = true;
        countDownCoroutine = StartCoroutine(Countdown());
        mike.SetActive(false);
    }

    public Color GetDarkerTone(Color color)
    {
        if (color == Color.red)
        {
            // Return a darker shade of red
            return new Color(0.5f, 0f, 0f);
        }
        else if (color == Color.green)
        {
            // Return a darker shade of green
            return new Color(0f, 0.5f, 0f);
        }
        else if (color == Color.blue)
        {
            // Return a darker shade of blue
            return new Color(0f, 0f, 0.5f);
        }
        else if (color == Color.white)
        {
            // Return a darker shade of white
            return new Color(0.5f, 0.5f, 0.5f);
        }
        else
        {
            // If the color is not supported, return the original color
            return color;
        }
    }

    private Color GetNextColor()
    {
        // Get the next color in the combination
        Color targetColor = helperMethods.GetInstance().GetColorFromString(currentCombination[currentCombinationIndex]);

        // Increment the index for the next iteration
        currentCombinationIndex++;


        return targetColor;
    }

    private IEnumerator FadeColor(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            light2D.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
    }

    void OnResultSpeech(string _data)
    {
        stageText.text = _data;
        Debug.Log("Dasdsasda");
    }

    public void StopRecording()
    {
        SpeechToText.Instance.StopRecording();
    }

    private System.Collections.IEnumerator RecordingTimer(float duration)
    {
        Debug.Log("xxxx111");
        yield return new WaitForSeconds(duration);
        Debug.Log("xxxx222");
        StopRecording();
    }

    private Coroutine recordingCoroutine;
    private IEnumerator Countdown()
    {
        SpeechToText.Instance.StartRecording("speak colors");
        yield return new WaitForSeconds(0.5f);
        if(sessionStarted){
            onGameEnd();
        }
    }

    private void onGameEnd()
    {
        SpeechToText.Instance.StopRecording();
        int level = ((currentCombinationIndexLevel / 33) + 1);
        int start = 0;
        int end = 0;
        if (level == 1)
        {
            start = 2;
            end = 4;
        }
        else if (level == 2)
        {
            start = 1;
            end = 5;
        }
        else
        {
            start = 0;
            end = 6;
        }

        int index = 0;
        for (int e = start; e <= end; e++)
        {
            Color newColor = helperMethods.GetInstance().GetColorFromString(currentCombination[index]);
            correctOption[e].GetComponent<Image>().color = newColor;
            correctOption[e].SetActive(true);
            index++;
        }

        pausebutton.SetActive(false);
        menuicon.SetActive(true);
        failPopupObject.SetActive(true);
        backgroundShadow.SetActive(false);
        pausebutton.SetActive(false);
        sessionStarted = false;
    }

    void OnVoiceCapture(string text)
    {

        string[] words = Regex.Split(text.ToLower(), @"\W+");

        if (sessionStarted)
        {
            string[] allowedWords = { "red", "green", "blue", "white" };
            words = words.Where(w => allowedWords.Contains(w)).ToArray();

            sessionResult = words.ToList();
            bool equal = sessionResult.SequenceEqual(currentLevelLightCombinations[currentCombinationIndexLevel - 1]);
            if (sessionResult.Count == currentLevelLightCombinations[currentCombinationIndexLevel - 1].Count && equal)
            {
                Debug.Log("Result: " + currentCombinationIndex);
                if (currentCombinationIndexLevel >= 99)
                {
                    currentCombinationIndex = 0;
                    currentCombinationIndexLevel = 0;
                    completePopupObject.SetActive(true);
                }
                if (countDownCoroutine != null)
                {
                    StopCoroutine(countDownCoroutine);
                    successPopupObject.SetActive(true);
                }
                playfabManager.Instance.onSubmitScore(currentCombinationIndexLevel + 1);
                menuicon.SetActive(true);
                pausebutton.SetActive(false);
            }
            else
            {
                onGameEnd();
            }
        }
        pausebutton.SetActive(false);
    }

    public void nextLevel()
    {
        button.Play();
        failPopupObject.SetActive(false);
        completePopupObject.SetActive(false);
        successPopupObject.SetActive(false);
        backgroundShadow.SetActive(true);
        sessionStarted = false;
        sessionResult.Clear();
        levelText.text = "Level " + ((currentCombinationIndexLevel / 33) + 1) + ":";
        stageText.text = "Stage " + ((currentCombinationIndexLevel % 33) + 1);
        currentCombinationIndexLevel += 1;
        currentCombination = currentLevelLightCombinations[currentCombinationIndexLevel - 1];
        StartCoroutine(SwitchColorsCoroutine());
        StopCoroutine(countDownCoroutine);
    }

    public void openSideMenu()
    {
        if(!sessionStarted){
            sidemenu.SetActive(true);
        }
    }

    public void closeSideMenu()
    {
        sidemenu.SetActive(false);
    }

    /*Helper methods*/

    public void RestartScene()
    {
        button.Play();
        helperMethods.GetInstance().RestartScene("gameplay");
    }

    public void logout()
    {
        helperMethods.GetInstance().RestartScene("login");
    }

    public void RestartAndPlay()
    {
        button.Play();
        for (int e = 0; e < 7; e++)
        {
            correctOption[e].SetActive(false);
        }

        failPopupObject.SetActive(false);
        completePopupObject.SetActive(false);
        successPopupObject.SetActive(false);
        sessionStarted = false;
        StopCoroutine(countDownCoroutine);
        onPlay();
    }

    private void OnDestroy()
    {
        SpeechToText.Instance.StopRecording();
    }
}
#endif
