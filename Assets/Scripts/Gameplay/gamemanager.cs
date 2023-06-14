using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using TMPro;
using System.Collections;
using UnityEngine.Windows.Speech;
using UnityEngine.SceneManagement;

public class gamemanager : MonoBehaviour
{
    public Light2D light2D;
    public TextMeshProUGUI score;
    public GameObject speak;
    public GameObject faillPopup;
    public Light2D[] statusLight;

    protected KeywordRecognizer recognizer;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    private bool sessionStarted = false;
    private List<string> sessionResult;
    private List<List<string>> currentLevelLightCombination;
    private int currentCombinationIndex;
    private List<string> currentCombination;
    private string[] availableColorNames = { "red", "green", "blue", "white", "orange" };

    private void Start()
    {
        recognizer = new KeywordRecognizer(availableColorNames, confidence);
        recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
        recognizer.Start();

        currentLevelLightCombination = new List<List<string>>();
        sessionResult = new List<string>();

        currentLevelLightCombination.AddRange(GeneratePermutations(new List<string> { "red", "green", "blue", "white" }, 3, 5));
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
                yield return new WaitForSeconds((1 / (currentCombinationIndex*2)));
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
                return Color.white;
        }
    }


    public void restartScene()
    {
        SceneManager.LoadScene("gameplay");
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (sessionStarted)
        {
            int currentIndex = sessionResult.Count;
            string response = args.text;
            sessionResult.Add(response);

            if(sessionResult[currentIndex] == currentLevelLightCombination[currentCombinationIndex - 1][currentIndex])
            {
                statusLight[sessionResult.Count-1].color = GetColorFromString(sessionResult[currentIndex]);
                if (sessionResult.Count == currentLevelLightCombination[currentCombinationIndex - 1].Count)
                {

                    for (int i = 0; i < sessionResult.Count; i++)
                    {
                        statusLight[i].color = Color.clear;
                    }

                    if (currentCombinationIndex >= 0)
                    {
                        currentCombinationIndex = 0;
                    }

                    StartCoroutine(SwitchColorsCoroutine());
                    speak.SetActive(false);
                    sessionStarted = false;
                    sessionResult.Clear();
                    score.text = "Level " + currentCombinationIndex;
                }
            }
            else
            {
                statusLight[sessionResult.Count - 1].color = GetColorFromString(sessionResult[currentIndex]);
                faillPopup.SetActive(true);
                speak.SetActive(false);
            }
            Debug.Log(args.text);
        }
    }
}
