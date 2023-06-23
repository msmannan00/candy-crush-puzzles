using UnityEngine;

public class RotateAnimation : MonoBehaviour
{
    public RectTransform imageTransform;
    public float rotationSpeed = 0f; // Halved rotation speed

    private void Start()
    {
        Rotate();
    }

    private void Rotate()
    {
        float rotationAngle = 360f;
        float rotationTime = rotationAngle / rotationSpeed;

        LeanTween.rotateAroundLocal(imageTransform.gameObject, Vector3.forward, rotationAngle, rotationTime*3)
            .setOnComplete(Rotate);
    }
}
