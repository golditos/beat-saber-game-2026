using UnityEngine;
using Ath.Beat.Gameplay;

public class SpiderHitReceiver : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private ScoreUI scoreUI;

    [Header("Particles")]
    [SerializeField] private ParticleSystem defaultHitParticles;
    [SerializeField] private float particleDestroyDelay = 2f;

    [Header("Score values")]
    [SerializeField] private int hitPoints = 50;
    [SerializeField] private int goodPoints = 100;
    [SerializeField] private int perfectPoints = 150;
    [SerializeField] private int headshotPoints = 300;
    [SerializeField] private int legPoints = 25;

    [Header("Anti spam")]
    [SerializeField] private float hitCooldown = 0.1f;

    private float lastHitTime;
    private bool wasHit;

    private BlockScript blockScript;
    private NoteSpawner noteSpawner;

    private static int currentScore;
    private static int currentCombo;

    private void Awake()
    {
        blockScript = GetComponent<BlockScript>();

        if (blockScript == null)
        {
            blockScript = GetComponentInParent<BlockScript>();
        }

        noteSpawner = FindFirstObjectByType<NoteSpawner>();

        if (blockScript == null)
        {
            Debug.LogWarning("SpiderHitReceiver no ha encontrado BlockScript en " + gameObject.name);
        }

        if (noteSpawner == null)
        {
            Debug.LogWarning("SpiderHitReceiver no ha encontrado NoteSpawner en la escena.");
        }
    }

    private void OnEnable()
    {
        wasHit = false;
        lastHitTime = 0f;
    }

    public void ReceiveHit(HitType hitType, Vector3 hitPoint, ParticleSystem particlesPrefab)
    {
        if (wasHit) return;
        if (Time.time - lastHitTime < hitCooldown) return;

        wasHit = true;
        lastHitTime = Time.time;

        int points = GetPoints(hitType);
        string feedbackText = GetFeedbackText(hitType);

        currentScore += points;
        currentCombo++;

        Debug.Log($"Hit correcto: {feedbackText} | +{points} | Score total: {currentScore}");

        ScoreUI activeScoreUI = GetActiveScoreUI();

        if (activeScoreUI != null)
        {
            Debug.Log("Enviando feedback a ScoreUI ACTIVO: " + feedbackText + " +" + points);

            activeScoreUI.UpdateScore(currentScore);
            activeScoreUI.UpdateCombo(currentCombo);
            activeScoreUI.ShowHitFeedback(feedbackText, points, hitPoint);
        }
        else
        {
            Debug.LogError("SpiderHitReceiver no encuentra ningún ScoreUI activo en la escena.");
        }

        SpawnHitParticles(hitType, hitPoint, particlesPrefab);

        ReturnSpiderToPool();
    }

    private ScoreUI GetActiveScoreUI()
    {
        if (scoreUI != null && scoreUI.gameObject.activeInHierarchy)
        {
            return scoreUI;
        }

        if (ScoreUI.Instance != null && ScoreUI.Instance.gameObject.activeInHierarchy)
        {
            scoreUI = ScoreUI.Instance;
            return scoreUI;
        }

        ScoreUI[] allScoreUIs = FindObjectsOfType<ScoreUI>();

        foreach (ScoreUI ui in allScoreUIs)
        {
            if (ui != null && ui.gameObject.activeInHierarchy)
            {
                scoreUI = ui;
                return scoreUI;
            }
        }

        return null;
    }

    private void ReturnSpiderToPool()
    {
        if (blockScript == null)
        {
            blockScript = GetComponent<BlockScript>();

            if (blockScript == null)
            {
                blockScript = GetComponentInParent<BlockScript>();
            }
        }

        if (noteSpawner == null)
        {
            noteSpawner = FindFirstObjectByType<NoteSpawner>();
        }

        if (noteSpawner != null && blockScript != null)
        {
            noteSpawner.ReturnToPool(blockScript);
        }
        else
        {
            Debug.LogWarning("No se pudo devolver la araña a la pool. Se desactiva directamente: " + gameObject.name);
            gameObject.SetActive(false);
        }
    }

    private void SpawnHitParticles(HitType hitType, Vector3 hitPoint, ParticleSystem particlesPrefab)
    {
        ParticleSystem prefabToUse = particlesPrefab != null ? particlesPrefab : defaultHitParticles;

        if (prefabToUse == null) return;

        ParticleSystem particles = Instantiate(prefabToUse, hitPoint, Quaternion.identity);

        int particleAmount = GetParticleAmount(hitType);
        float particleSize = GetParticleSize(hitType);
        float particleSpeed = GetParticleSpeed(hitType);

        var emission = particles.emission;
        emission.SetBursts(new ParticleSystem.Burst[]
        {
            new ParticleSystem.Burst(0f, particleAmount)
        });

        var main = particles.main;
        main.startSizeMultiplier = particleSize;
        main.startSpeedMultiplier = particleSpeed;

        particles.Play();

        Destroy(particles.gameObject, particleDestroyDelay);
    }

    private int GetParticleAmount(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Leg:
                return 8;

            case HitType.Hit:
                return 14;

            case HitType.Good:
                return 24;

            case HitType.Perfect:
                return 36;

            case HitType.Headshot:
                return 55;

            default:
                return 10;
        }
    }

    private float GetParticleSize(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Leg:
                return 0.04f;

            case HitType.Hit:
                return 0.06f;

            case HitType.Good:
                return 0.08f;

            case HitType.Perfect:
                return 0.1f;

            case HitType.Headshot:
                return 0.13f;

            default:
                return 0.06f;
        }
    }

    private float GetParticleSpeed(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Leg:
                return 1.2f;

            case HitType.Hit:
                return 1.6f;

            case HitType.Good:
                return 2f;

            case HitType.Perfect:
                return 2.4f;

            case HitType.Headshot:
                return 3f;

            default:
                return 1.5f;
        }
    }

    private int GetPoints(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Hit:
                return hitPoints;

            case HitType.Good:
                return goodPoints;

            case HitType.Perfect:
                return perfectPoints;

            case HitType.Headshot:
                return headshotPoints;

            case HitType.Leg:
                return legPoints;

            default:
                return 0;
        }
    }

    private string GetFeedbackText(HitType hitType)
    {
        switch (hitType)
        {
            case HitType.Hit:
                return "HIT";

            case HitType.Good:
                return "GOOD";

            case HitType.Perfect:
                return "PERFECT";

            case HitType.Headshot:
                return "HEADSHOT";

            case HitType.Leg:
                return "LEG HIT";

            default:
                return "";
        }
    }

    public static void ResetGlobalScore()
    {
        currentScore = 0;
        currentCombo = 0;
    }
}