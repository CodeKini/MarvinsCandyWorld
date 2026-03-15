// ============================================================
// GameManager.cs
// Place this on a GameObject called "GameManager" in NormalWorld.
// This object uses DontDestroyOnLoad so it persists into CandyWorld,
// keeping the score and UI alive across the scene change.
//
// The GameManager also owns the HUD Canvas as a child object,
// so the UI travels with it between scenes.
//
// Required child hierarchy:
//   GameManager (this script)
//   └── HUD Canvas (Canvas, CanvasScaler, GraphicRaycaster)
//       ├── ScoreText       (UI > Legacy > Text)
//       ├── WinText         (UI > Legacy > Text)
//       └── FadePanel       (UI > Image, black, stretch to fill)
// ============================================================

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // ---- Singleton ----
    public static GameManager Instance { get; private set; }

    [Header("UI References  (assign in Inspector)")]
    public Text  scoreText;   // shows "Candies: X / 10"
    public Text  winText;     // hidden until player wins
    public Image fadePanel;   // full-screen black image for fade

    [Header("Game Settings")]
    public int   candiesNeeded = 10;
    public float fadeDuration  = 1f;  // seconds for each fade

    // ---- Private state ----
    private int  candiesCollected = 0;
    private bool gameWon          = false;

    // ================================================================
    //  Unity lifecycle
    // ================================================================

    void Awake()
    {
        // Classic singleton pattern that survives scene loads
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);   // a duplicate was loaded – destroy it
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);  // keep alive across scenes
    }

    void Start()
    {
        RefreshScoreUI();

        if (winText  != null) winText.gameObject.SetActive(false);
        if (fadePanel != null) SetFadeAlpha(0f);
    }

    // ================================================================
    //  Public API called by other scripts
    // ================================================================

    /// <summary>Called by CandyPickup when the player touches a candy.</summary>
    public void CollectCandy()
    {
        if (gameWon) return;

        candiesCollected++;
        RefreshScoreUI();

        if (candiesCollected >= candiesNeeded)
            HandleWin();
    }

    /// <summary>Called by PoolPortal to start the scene transition.</summary>
    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoadScene(sceneName));
    }

    // ================================================================
    //  Private helpers
    // ================================================================

    void RefreshScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Candies: {candiesCollected} / {candiesNeeded}";
    }

    void HandleWin()
    {
        gameWon = true;

        if (winText != null)
        {
            winText.gameObject.SetActive(true);
            winText.text = "You collected all the candy!\nYou WIN! ";
        }
    }

    // Fades screen to black, loads the scene, then fades back in
    IEnumerator FadeAndLoadScene(string sceneName)
    {
        // --- Fade OUT (transparent → black) ---
        yield return StartCoroutine(Fade(0f, 1f));

        SceneManager.LoadScene(sceneName);

        // Wait one frame so the new scene finishes initialising
        yield return null;

        // --- Fade IN (black → transparent) ---
        yield return StartCoroutine(Fade(1f, 0f));
    }

    IEnumerator Fade(float fromAlpha, float toAlpha)
    {
        if (fadePanel == null) yield break;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetFadeAlpha(Mathf.Lerp(fromAlpha, toAlpha, t));
            yield return null;
        }

        SetFadeAlpha(toAlpha);
    }

    void SetFadeAlpha(float alpha)
    {
        if (fadePanel == null) return;
        Color c = fadePanel.color;
        c.a = alpha;
        fadePanel.color = c;
    }
}
