using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton pattern, ba�ka scriptlerden kolay eri�im i�in

    public int currentScore = 0; // Oyuncunun mevcut puan�
    public TextMeshProUGUI scoreText; // Puan�n g�r�nt�lenece�i UI Text

    public int targetScore = 1000; // Seviyeyi tamamlamak i�in gereken hedef puan
    public TextMeshProUGUI targetText; // Hedef puan�n g�r�nt�lenece�i UI Text

    public GameObject levelCompletePanel; // Seviye tamamland���nda a��lacak panel

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreText(); // Ba�lang��ta skoru g�ncelle
        UpdateTargetText(); // Hedef puan� g�ncelle
        levelCompletePanel.SetActive(false); // Seviye tamamland� panelini kapal� ba�lat
    }

    // Puan ekleme fonksiyonu
    public void AddScore(int points)
    {
        switch (points)
        {
            case 2:
                // Puan ekle
                currentScore += 10;
                break;
            case 3:
                // Puan ekle
                currentScore += 15;
                break;
            case 4:
                // Puan ekle
                currentScore += 20;
                break;
            default:
                if(points > 4)
                {
                    currentScore += 30;
                }
                break;
        }
         
        UpdateScoreText(); // UI g�ncelle

        // E�er hedef puana ula��ld�ysa
        if (currentScore >= targetScore)
        {
            LevelComplete();
        }
    }

    // Skor UI g�ncelleme fonksiyonu
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    // Hedef UI g�ncelleme fonksiyonu
    private void UpdateTargetText()
    {
        if (targetText != null)
        {
            targetText.text = $"Target: {targetScore}";
        }
    }

    // Seviye tamamland���nda yap�lacak i�lemler
    private void LevelComplete()
    {
        Debug.Log("Level Complete!");
        levelCompletePanel.SetActive(true); // Seviye tamamland� panelini g�ster
        Time.timeScale = 0; // Oyunu durdur
    }
}
