using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using TMPro;

public class GameplayManager : MonoBehaviour
{
    public UnityEngine.Rendering.Universal.Light2D light2D;
    public TextMeshProUGUI score;
    public float fadeDuration = 2f; // Duration of the color fade in seconds

    // List of known named colors
    public List<string> colorNames;

    private string lastSpokenWord = string.Empty; // Last spoken word captured from the microphone
    private bool spokenWordMatchesColor = false; // Flag to track if spoken word matches target color

    // Start is called before the first frame update
    void Start()
    {
        // Populate the colorNames list with a variety of color names
        colorNames = new List<string>()
        {
            "red",
            "green",
            "blue",
            "yellow",
            "cyan",
            "magenta",
            "purple",
            "orange",
            "pink",
            "teal",
            "lime",
            "olive",
            "navy",
            "maroon",
            "silver",
            "aqua",
            "fuchsia",
            "white"
        };

        StartCoroutine(ChangeColorRoutine());
    }

    // Coroutine to change the color gradually after a delay
    IEnumerator ChangeColorRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Wait for 5 seconds before changing color

            Color startColor = light2D.color;
            Color targetColor = GetRandomNamedColor();
            string targetColorName = GetColorName(targetColor);

            float elapsedTime = 0f;
            spokenWordMatchesColor = false;

            if (colorNames.Contains(lastSpokenWord) && lastSpokenWord.Equals(targetColorName))
            {
                // Spoken word matches target color, set the boolean flag
                spokenWordMatchesColor = true;
            }
            else
            {
                // Spoken word doesn't match target color, wait for the full duration
                yield return new WaitForSeconds(fadeDuration);
            }

            while (elapsedTime < fadeDuration && !spokenWordMatchesColor)
            {
                // Calculate the current color based on the elapsed time and fade duration
                float t = elapsedTime / fadeDuration;
                Color currentColor = Color.Lerp(startColor, targetColor, t);

                // Set the color of the light
                light2D.color = currentColor;

                elapsedTime += Time.deltaTime;

                // Check for microphone input
                float[] samples = new float[128];
                int position = Microphone.GetPosition(null) - 128;
                if (position >= 0)
                {
                    AudioListener.GetSpectrumData(samples, 0, FFTWindow.Blackman);

                    // Adjust the threshold value to detect speech more accurately
                    float threshold = 0.1f;

                    if (Mathf.Max(samples) > threshold)
                    {
                        // Speech is detected, process the captured audio
                        string spokenWord = ProcessAudio(samples);
                        Debug.Log("Spoken word: " + spokenWord);

                        // Check if the spoken word matches a color name
                        if (colorNames.Contains(spokenWord))
                        {
                            Debug.Log("Recognized color: " + spokenWord);
                            lastSpokenWord = spokenWord; // Store the last spoken word
                        }
                    }
                }

                yield return null;
            }

            // Ensure the final color is set correctly
            light2D.color = targetColor;
        }
    }

    // Generates a random named color
    Color GetRandomNamedColor()
    {
        // Get a random color name from the list
        string randomColorName = colorNames[Random.Range(0, colorNames.Count)];

        // Convert the color name to a Color object
        Color namedColor;
        ColorUtility.TryParseHtmlString(randomColorName, out namedColor);

        return namedColor;
    }

    // Get the name of a color from the colorNames list
    string GetColorName(Color color)
    {
        foreach (string colorName in colorNames)
        {
            Color namedColor;
            ColorUtility.TryParseHtmlString(colorName, out namedColor);

            if (color.Equals(namedColor))
            {
                return colorName;
            }
        }

        return string.Empty;
    }

    // Process the captured audio and extract spoken word
    string ProcessAudio(float[] samples)
    {
        // TODO: Implement your speech recognition logic here
        // You can use a speech recognition library or service to process the audio and extract the spoken word
        // Update the implementation based on the library or service you choose

        // For demonstration purposes, this code assumes the spoken word is "red" if the audio is above the threshold
        float threshold = 0.1f;
        if (samples[0] > threshold)
        {
            return "red";
        }

        return string.Empty;
    }

    // Update is called once per frame
    void Update()
    {

    }}
