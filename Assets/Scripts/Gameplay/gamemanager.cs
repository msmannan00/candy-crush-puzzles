using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using System.Linq;

public class gamemanager : MonoBehaviour
{
    public Light2D light2D;
    public TextMeshProUGUI score;
    public TextMeshProUGUI remainingTime;
    public GameObject speak;
    public GameObject faillPopup;
    public Light2D[] statusLight;

    private DictationRecognizer recognizer;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    private Coroutine countDownCoroutine = null;
    private bool sessionStarted = false;
    private List<string> sessionResult;
    private List<List<string>> currentLevelLightCombination;
    private int currentCombinationIndex;
    private List<string> currentCombination;
    private string[] availableColorNames = { "red", "green", "blue", "white", "orange" };

    private void Start()
    {
        // Create an instance of the DictationRecognizer.
        recognizer = new DictationRecognizer();

        // Register the event handlers.
        recognizer.AutoSilenceTimeoutSeconds = 20f;
        recognizer.InitialSilenceTimeoutSeconds = 20f;
        recognizer.DictationHypothesis += OnDictationHypothesis;
        recognizer.DictationResult += OnDictationResult;
        recognizer.DictationComplete += OnDictationComplete;
        recognizer.DictationError += OnDictationError;
        // Start the dictation recognizer.
        recognizer.Start();

        currentLevelLightCombination = new List<List<string>>();
        sessionResult = new List<string>();

        currentLevelLightCombination.AddRange(GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 3, 33));
        currentLevelLightCombination.AddRange(GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 5, 33));
        currentLevelLightCombination.AddRange(GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 7, 33));

        currentCombinationIndex = 0;
        currentCombination = currentLevelLightCombination[currentCombinationIndex];

        // Start the coroutine to switch colors with a 1-second delay
        StartCoroutine(SwitchColorsCoroutine());
    }

    private IEnumerator SwitchColorsCoroutine()
    {
        float fadeDuration = 0.5f;

        for (int i = 0; i < currentCombination.Count; i++)
        {
            // Switch to the next color
            string colorName = currentCombination[i];
            Color targetColor = GetColorFromString(colorName);

            // Fade out the previous color
            yield return FadeColor(light2D.color, Color.clear, fadeDuration);

            // Fade in the new color
            yield return FadeColor(light2D.color, targetColor, fadeDuration);

            // Wait for a second before switching to the next color
            if (currentCombinationIndex == 0)
            {
                yield return new WaitForSeconds(1);
            }
            else
            {
                yield return new WaitForSeconds((1 / (currentCombinationIndex * 2)));
            }
        }

        // Move to the next combination
        currentCombinationIndex++;
        if (currentCombinationIndex >= currentLevelLightCombination.Count)
        {
            // If we reached the end, shuffle the combinations and start over
            ShuffleList(currentLevelLightCombination);
            currentCombinationIndex = 0;
        }

        currentCombination = currentLevelLightCombination[currentCombinationIndex];

        // Reset light color instantly
        yield return FadeColor(light2D.color, Color.clear, fadeDuration);
        yield return new WaitForSeconds(0.5f);
        speak.SetActive(true);
        sessionStarted = true;
        countDownCoroutine = StartCoroutine(Countdown());
    }


    private IEnumerator Countdown()
    {
        int countdownValue = ((currentCombinationIndex / 33) + 1) * 5;

        while (countdownValue > 0)
        {
            remainingTime.text = countdownValue + "sec";
            yield return new WaitForSeconds(1f);
            countdownValue--;
        }

        faillPopup.SetActive(true);
        speak.SetActive(false);
        sessionStarted = false;
        yield return new WaitForSeconds(2f);
        remainingTime.text = countdownValue + "sec";
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

    private List<List<string>> GeneratePermutations(List<string> colors, int length, int count)
    {
        List<List<string>> permutations = new List<List<string>>();

        // Generate all possible permutations
        List<List<string>> allPermutations = GetPermutations(colors, length);

        // Shuffle the list of permutations
        ShuffleList(allPermutations);

        // Take the first 'count' number of permutations
        for (int i = 0; i < count; i++)
        {
            permutations.Add(allPermutations[i]);
        }

        return permutations;
    }

    private List<List<string>> GetPermutations(List<string> colors, int length)
    {
        List<List<string>> permutations = new List<List<string>>();

        GeneratePermutationsHelper(colors, length, new List<string>(), permutations);

        return permutations;
    }

    private void GeneratePermutationsHelper(List<string> colors, int length, List<string> currentPermutation, List<List<string>> permutations)
    {
        if (currentPermutation.Count == length)
        {
            permutations.Add(new List<string>(currentPermutation));
            return;
        }

        for (int i = 0; i < colors.Count; i++)
        {
            currentPermutation.Add(colors[i]);
            GeneratePermutationsHelper(colors, length, currentPermutation, permutations);
            currentPermutation.RemoveAt(currentPermutation.Count - 1);
        }
    }

    private void ShuffleList<T>(List<T> list)
    {
        System.Random random = new System.Random();

        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = random.Next(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    private Color GetColorFromString(string colorName)
    {
        switch (colorName)
        {
            case "red":
                return Color.red;
            case "green":
                return Color.green;
            case "blue":
                return Color.blue;
            case "white":
                return Color.white;
            default:
                return Color.magenta;
        }
    }


    public void restartScene()
    {
        SceneManager.LoadScene("gameplay");
    }

    private void OnDictationHypothesis(string text)
    {
    }

    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log("Result: " + text);
        string[] words = Regex.Split(text.ToLower(), @"\W+");

        if (sessionStarted)
        {
            sessionResult = words.ToList();
            bool equal = sessionResult.SequenceEqual(currentLevelLightCombination[currentCombinationIndex - 1]);
            if (sessionResult.Count == currentLevelLightCombination[currentCombinationIndex - 1].Count && equal)
            {
                for (int i = 0; i < statusLight.Length; i++)
                {
                    statusLight[i].color = Color.clear;
                }

                if (currentCombinationIndex >= 99)
                {
                    currentCombinationIndex = 0;
                }

                StartCoroutine(SwitchColorsCoroutine());
                speak.SetActive(false);
                sessionStarted = false;
                sessionResult.Clear();
                score.text = "Level " + currentCombinationIndex;
                if (countDownCoroutine != null)
                {
                    StopCoroutine(countDownCoroutine);
                }
            }
            else
            {
                if (words.Length < 7)
                {
                    for (int i = 0; i < words.Length; i++)
                    {
                        statusLight[i].color = GetColorFromString(words[i]);
                    }
                }

                faillPopup.SetActive(true);
                speak.SetActive(false);
                if (countDownCoroutine != null)
                {
                    StopCoroutine(countDownCoroutine);
                }
            }
        }

    }

    private void OnDictationComplete(DictationCompletionCause cause)
    {
        Debug.Log("Dictation complete. Cause: " + cause.ToString());

        recognizer.Stop();
        recognizer.Start();
    }

    private void OnDictationError(string error, int hresult)
    {
        Debug.LogError("Dictation error: " + error + ", HRESULT: " + hresult);

        recognizer.Stop();
        recognizer.Start();
    }

    private void OnDestroy()
    {
        // Stop and dispose of the dictation recognizer when it's no longer needed.
        if (recognizer != null)
        {
            recognizer.Stop();
            recognizer.Dispose();
        }
    }
}
