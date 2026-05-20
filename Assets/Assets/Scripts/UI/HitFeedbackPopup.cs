using TMPro;
using UnityEngine;

public class HitFeedbackPopup : MonoBehaviour
{
    [SerializeField] private TextMeshPro textMesh;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private float floatSpeed = 0.8f;
    [SerializeField] private float startScale = 1.2f;
    [SerializeField] private float endScale = 0.4f;

    private Camera mainCamera;
    private float timer;
    private Color initialColor;

    public void Setup(string message, int points, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = message + "\n+" + points;
            textMesh.color = color;
            initialColor = color;
        }

        transform.localScale = Vector3.one * startScale;

        mainCamera = Camera.main;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0f, 180f, 0f);
        }

        float progress = timer / lifetime;

        float scale = Mathf.Lerp(startScale, endScale, progress);
        transform.localScale = Vector3.one * scale;

        if (textMesh != null)
        {
            Color fadedColor = initialColor;
            fadedColor.a = Mathf.Lerp(1f, 0f, progress);
            textMesh.color = fadedColor;
        }

        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}