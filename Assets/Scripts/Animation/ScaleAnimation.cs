using UnityEngine;

public class ScaleAnimation : MonoBehaviour
{
    public Vector3 scaleInTarget = Vector3.one * 0.8f;
    public float scaleInDuration = 0.5f;
    public Vector3 scaleOutTarget = Vector3.one;
    public float scaleOutDuration = 0.5f;

    private Coroutine animationCoroutine;
    public bool repeat = true;

    private void Start()
    {
        // Start the animation coroutine
        animationCoroutine = StartCoroutine(AnimationCoroutine());
    }

    private System.Collections.IEnumerator AnimationCoroutine()
    {
        // Scale in
        LeanTween.scale(gameObject, scaleInTarget, scaleInDuration);

        yield return new WaitForSeconds(scaleInDuration);

        // Scale out
        LeanTween.scale(gameObject, scaleOutTarget, scaleOutDuration);

        yield return new WaitForSeconds(scaleOutDuration);

        // Repeat the animation
        if (repeat)
        {
            Start();
        }
    }

    private void OnEnable()
    {
        // Start the animation coroutine when the object is enabled
        animationCoroutine = StartCoroutine(AnimationCoroutine());
    }

    private void OnDestroy()
    {
        // Stop the animation coroutine when the object is destroyed
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }
    }
}
