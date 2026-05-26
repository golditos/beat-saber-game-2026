using UnityEngine;
using Ath.Beat.Audio;
using Ath.Beat.Gameplay;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    public BeatDetector beatDetector;
    public NoteSpawner noteSpawner;

    [Header("UI (opcional)")]
    public TMPro.TextMeshProUGUI scoreText;
    public TMPro.TextMeshProUGUI comboText;

    private int score = 0;
    private int combo = 0;

    private void OnEnable()
    {
        BlockScript.OnBlockHit  += HandleHit;
        BlockScript.OnBlockMiss += HandleMiss;
    }

    private void OnDisable()
    {
        BlockScript.OnBlockHit  -= HandleHit;
        BlockScript.OnBlockMiss -= HandleMiss;
    }

    private void Start()
    {
        beatDetector.StartSong(dspDelay: 0.5);
    }
    
    private void HandleHit(BlockScript bloc, HitRating rating)
    {
        int points = rating switch
        {
            HitRating.Perfect => 115,
            HitRating.Good => 80,
            HitRating.Ok => 40,
            _ => 0
        };

        combo++;
        score += points * Mathf.Max(1, combo / 8);

        UpdateUI();
        Debug.Log($"[Hit] {rating} | Score: {score} | Combo: {combo}");
    }

    private void HandleMiss(BlockScript bloc)
    {
        combo = 0;
        UpdateUI();
        Debug.Log("[Miss] Combo reiniciat");
    }

    private void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString("N0");
        if (comboText != null) comboText.text = combo > 1 ? $"x{combo}" : "";
    }
}