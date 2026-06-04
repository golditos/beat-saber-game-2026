using System.Collections;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    public static ScoreUI Instance { get; private set; }

    [Header("HUD Texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI comboText;
    [SerializeField] private TextMeshProUGUI lastHitText;
    [SerializeField] private TextMeshProUGUI pointsText;

    [Header("Floating Hit Feedback")]
    [SerializeField] private HitFeedbackPopup popupPrefab;
    [SerializeField] private Transform popupParent;

    [Header("HUD Animation")]
    [SerializeField] private float feedbackVisibleTime = 0.45f;
    [SerializeField] private float feedbackScaleAmount = 1.25f;

    private Coroutine feedbackCoroutine;
    private Vector3 lastHitOriginalScale = Vector3.one;
    private Vector3 pointsOriginalScale = Vector3.one;
    
    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    private void Awake()
    {
        Instance = this;

        if (lastHitText != null)
        {
            lastHitOriginalScale = lastHitText.transform.localScale;
        }
        else
        {
            Debug.LogError("ScoreUI: LastHitText NO está asignado.");
        }

        if (pointsText != null)
        {
            pointsOriginalScale = pointsText.transform.localScale;
        }
        else
        {
            Debug.LogError("ScoreUI: PointsText NO está asignado.");
        }

        if (scoreText == null)
        {
            Debug.LogError("ScoreUI: ScoreText NO está asignado.");
        }

        if (comboText == null)
        {
            Debug.LogError("ScoreUI: ComboText NO está asignado.");
        }
    }

    private void Start()
    {
        UpdateScore(0);
        UpdateCombo(0);

        if (lastHitText != null)
        {
            lastHitText.text = "READY";
        }

        if (pointsText != null)
        {
            pointsText.text = "";
        }
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE " + score;
        }
    }

    public void UpdateCombo(int combo)
    {
        if (comboText != null)
        {
            comboText.text = "COMBO x" + combo;
        }
    }

    public void ShowHitFeedback(string message, int points, Vector3 worldPosition)
    {
        Debug.Log("ScoreUI recibe feedback: " + message + " +" + points);

        Color feedbackColor = GetColorByMessage(message);

        ShowHUDFeedback(message, points, feedbackColor);
        ShowFloatingPopup(message, points, worldPosition, feedbackColor);
    }

    private void ShowHUDFeedback(string message, int points, Color color)
    {
        if (lastHitText != null)
        {
            lastHitText.text = message;
            lastHitText.color = color;
        }
        else
        {
            Debug.LogError("No se puede mostrar el tipo de golpe porque LastHitText está vacío.");
        }

        if (pointsText != null)
        {
            pointsText.text = "+" + points;
            pointsText.color = color;
        }
        else
        {
            Debug.LogError("No se pueden mostrar los puntos porque PointsText está vacío.");
        }

        if (feedbackCoroutine != null)
        {
            StopCoroutine(feedbackCoroutine);
        }

        feedbackCoroutine = StartCoroutine(AnimateHUDFeedback());
    }

    private void ShowFloatingPopup(string message, int points, Vector3 worldPosition, Color color)
    {
        if (popupPrefab == null) return;

        HitFeedbackPopup popup = Instantiate(
            popupPrefab,
            worldPosition,
            Quaternion.identity,
            popupParent
        );

        popup.Setup(message, points, color);
    }

    private IEnumerator AnimateHUDFeedback()
    {
        float timer = 0f;

        if (lastHitText != null)
        {
            lastHitText.transform.localScale = lastHitOriginalScale * feedbackScaleAmount;
        }

        if (pointsText != null)
        {
            pointsText.transform.localScale = pointsOriginalScale * feedbackScaleAmount;
        }

        while (timer < feedbackVisibleTime)
        {
            timer += Time.deltaTime;

            float progress = timer / feedbackVisibleTime;
            float scale = Mathf.Lerp(feedbackScaleAmount, 1f, progress);

            if (lastHitText != null)
            {
                lastHitText.transform.localScale = lastHitOriginalScale * scale;
            }

            if (pointsText != null)
            {
                pointsText.transform.localScale = pointsOriginalScale * scale;
            }

            yield return null;
        }

        if (lastHitText != null)
        {
            lastHitText.transform.localScale = lastHitOriginalScale;
        }

        if (pointsText != null)
        {
            pointsText.transform.localScale = pointsOriginalScale;
        }
    }

    private Color GetColorByMessage(string message)
    {
        switch (message)
        {
            case "HEADSHOT":
                return Color.red;

            case "PERFECT":
                return Color.yellow;

            case "GOOD":
                return Color.green;

            case "HIT":
                return Color.white;

            case "LEG HIT":
                return Color.gray;

            default:
                return Color.white;
        }
    }
}