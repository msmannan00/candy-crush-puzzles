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
    public GameObject failPopupObject;
    public GameObject successPopupObject;
    public GameObject completePopupObject;
    public AudioSource theme;
    public AudioSource button;
    public List<GameObject> correctOption;


    private DictationRecognizer recognizer;
    private Coroutine countDownCoroutine = null;
    private bool sessionStarted = false;
    private List<string> sessionResult = new List<string>();
    private List<List<string>> currentLevelLightCombinations = new List<List<string>>();
    private int currentCombinationIndex = 0;
    private int currentCombinationIndexLevel = 88;
    private List<string> currentCombination;
    public GameObject backgroundShadow;
    public GameObject mike;
    public GameObject defaultDot;
    public GameObject playButton;

    private void Start()
    {
        initializeRecognizer();
        theme.Play();
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
        theme.Stop();
        mike.SetActive(true);
        defaultDot.SetActive(false);
    }

    private void initializeLevel()
    {
        LeanTween.alpha(mike, 0f, 0f);
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 3, 33));
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 5, 33));
        currentLevelLightCombinations.AddRange(helperMethods.GetInstance().GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 7, 33));
        currentCombination = currentLevelLightCombinations[currentCombinationIndexLevel - 1];
        StartCoroutine(SwitchColorsCoroutine());
    }

    private IEnumerator SwitchColorsCoroutine()
    {
        float fadeDuration = 0.2f;
        float blackDuration = 1f;

        // Loop through each color in the combination
        for (int i = 0; i < currentCombination.Count; i++)
        {
            // Switch to the next color
            yield return FadeColor(light2D.color, GetNextColor(), fadeDuration);
            yield return new WaitForSeconds(blackDuration);
            if (i != currentCombination.Count - 1)
            {
                yield return FadeColor(light2D.color, Color.black, 0.2f);
            }
        }

        yield return new WaitForSeconds(0.5f);
        backgroundShadow.SetActive(false);
        yield return FadeColor(light2D.color, Color.black, 0.2f);
        sessionStarted = true;
        countDownCoroutine = StartCoroutine(Countdown());
    }

    private Color GetNextColor()
    {
        // Get the next color in the combination
        Color targetColor = helperMethods.GetInstance().GetColorFromString(currentCombination[currentCombinationIndex]);

        // Increment the index for the next iteration
        currentCombinationIndex++;

        // Check if the index exceeds the combination count
        if (currentCombinationIndex >= currentCombination.Count)
        {
            // Reset the index and shuffle the combination
            currentCombinationIndex = 0;
        }

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
        int countdownValue = ((currentCombinationIndexLevel / 33) + 1) * 2;

        while (countdownValue > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownValue--;
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
            }
            else
            {
                onGameEnd();
            }
        }
    }

    public void nextLevel()
    {
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
        helperMethods.GetInstance().RestartScene("gameplay");
    }

    public void RestartAndPlay()
    {
        for (int e = 0; e < 7; e++)
        {
            correctOption[e].SetActive(false);
        }

        currentCombinationIndex = 0;
        currentCombinationIndexLevel = 1;
        failPopupObject.SetActive(false);
        completePopupObject.SetActive(false);
        successPopupObject.SetActive(false);
        sessionStarted = false;
        levelText.text = "Level 1:";
        stageText.text = "Stage 1";
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
