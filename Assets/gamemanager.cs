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

    public string[] keywords = new string[] { "red", "green", "blue", "orange", "white" };
    protected KeywordRecognizer recognizer;
    public ConfidenceLevel confidence = ConfidenceLevel.Medium;

    int current_score = 0;
    public float fadeDuration = 3f; // Duration of the color fade in seconds
    public List<string> colorNames;
    Color randomColor = Color.red;


    private void Start()
    {
        if (keywords != null)
        {
            recognizer = new KeywordRecognizer(keywords, confidence);
            recognizer.OnPhraseRecognized += Recognizer_OnPhraseRecognized;
            recognizer.Start();
            Debug.Log(recognizer.IsRunning);
        }

        foreach (var device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    public void ChangeLightColor()
    {
        // Get a random color name from the list
        string randomColorName = colorNames[Random.Range(0, colorNames.Count)];

        // Get the corresponding color based on the color name
        Color currentRandomColor = GetColorByName(randomColorName, randomColor);
        if (currentRandomColor == randomColor)
        {
            randomColor = Color.white;
        }
        else
        {
            randomColor = currentRandomColor;
        }

        // Assign the random color to the light object
        StartCoroutine(FadeLightColor(randomColor, fadeDuration));
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

    private Color GetColorByName(string colorName, Color randomColor)
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
        if (colorNames.Contains(args.text))
        {
            ChangeLightColor();
            current_score += 1;
            score.text = "Score: " + current_score;
        }
    }

    private void Update()
    {
    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= Recognizer_OnPhraseRecognized;
            recognizer.Stop();
        }
    }
}
