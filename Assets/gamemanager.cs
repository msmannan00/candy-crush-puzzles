using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using TMPro;

public class gamemanager : MonoBehaviour
{
    public Light2D light2D;
    public TextMeshProUGUI score;

    private List<string> currentLevelLightCombination;

    protected KeywordRecognizer recognizer;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    int current_score = 0;
    public float fadeDuration = 3f; // Duration of the color fade in seconds
    public List<string> colorNames;
    Color randomColor = Color.red;
    private string[] availableColorNames = { "red", "green", "blue", "white", "orange" };

    bool playerResponded = false;
    private int currentLevel = 1;
    int currentIndex = 0;


    private void Start()
    {
        GenerateLightCombination();
        DisplayLights();
        if (currentLevelLightCombination != null)
        {
            recognizer = new KeywordRecognizer(availableColorNames, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
            Debug.Log(recognizer.IsRunning);
        }

        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    private void DisplayLights()
    {
        StartCoroutine(DisplayLightSequence());
    }

    private IEnumerator DisplayLightSequence()
    {
        playerResponded = false;
        StartCoroutine(FadeLightColor(GetColorByName(currentLevelLightCombination[currentIndex]), fadeDuration));
        StartCoroutine(StartListening());
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator StartListening()
    {
        float responseTime = currentLevel * 50f + 2f;
        yield return new WaitForSeconds(responseTime);

        if (!playerResponded)
        {
            // Player didn't respond in time, handle accordingly (e.g., game over)
            Debug.Log("Game over");
        }
    }

    private void GenerateLightCombination()
    {
        currentLevelLightCombination = new List<string>();
        int lightCount = currentLevel * 2 + 1;

        for (int i = 0; i < lightCount; i++)
        {
            string lightColor = GetRandomColorName();
            currentLevelLightCombination.Add(lightColor);
        }
    }

    private IEnumerator FadeLightColor(Color targetColor, float duration)
    {
        Color initialColor = light2D.color;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            light2D.color = Color.Lerp(initialColor, targetColor, t);
            yield return null;
        }

        // Ensure the final color is set accurately
        light2D.color = targetColor;
    }

    private string GetRandomColorName()
    {
        int randomIndex = Random.Range(0, availableColorNames.Length);
        return availableColorNames[randomIndex];
    }

    private Color GetColorByName(string colorName)
    {
        switch (colorName)
        {
            case "red":
                return Color.red;
            case "white":
                return Color.white;
            case "green":
                return Color.green;
            case "blue":
                return Color.blue;
            case "orange":
                return new Color(1f, 0.64f, 0f);
            default:
                Debug.LogWarning("Unknown color name: " + colorName);
                return Color.white;
        }
    }

    private void Recognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        Debug.Log(args.text);
        string response = args.text;
        playerResponded = true;

        if (currentLevelLightCombination.Contains(args.text))
        {

            if (response == currentLevelLightCombination[currentIndex])
            {
                // Correct response
                currentIndex++;
                current_score++;
                if (currentIndex == currentLevelLightCombination.Count)
                {
                    Debug.Log("Level complete!");
                    currentLevel++;
                    current_score = 0;

                    GenerateLightCombination();
                    currentIndex = 0;
                }
                DisplayLights();
                playerResponded = false;
            }
            else
            {
                Debug.Log("Game over");
            }
        }


        score.text = "Level " + currentLevel + "\nScore: " + current_score;

    }
}
