using UnityEngine;
using UnityEngine.Windows.Speech;

public class SpeechRecognition : MonoBehaviour
{
    private DictationRecognizer recognizer;

    // Start is called before the first frame update
    void Start()
    {
        // Create a new DictationRecognizer
        recognizer = new DictationRecognizer();

        // Subscribe to the DictationRecognizer events
        recognizer.DictationResult += OnDictationResult;
        recognizer.DictationError += OnDictationError;

        // Start the speech recognition
        recognizer.Start();
    }

    // Event handler for receiving dictation results
    private void OnDictationResult(string text, ConfidenceLevel confidence)
    {
        // Process the recognized text
        Debug.Log("Recognized Text: " + text);

        // You can use the recognized text here or pass it to other methods for further processing
    }

    // Event handler for handling dictation errors
    private void OnDictationError(string error, int hresult)
    {
        // Handle dictation errors
        Debug.LogError("Dictation Error: " + error);
    }

    // Stop the speech recognition when the application quits
    private void OnApplicationQuit()
    {
        recognizer.Stop();
        recognizer.Dispose();
    }
}
