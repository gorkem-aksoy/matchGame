using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance; // Singleton pattern, baþka scriptlerden kolay eriþim için

    public int currentScore = 0; // Oyuncunun mevcut puaný
    public TextMeshProUGUI scoreText; // Puanýn görüntüleneceði UI Text

    public int targetScore = 1000; // Seviyeyi tamamlamak için gereken hedef puan
    public TextMeshProUGUI targetText; // Hedef puanýn görüntüleneceði UI Text

    public GameObject levelCompletePanel; // Seviye tamamlandýðýnda açýlacak panel

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
        UpdateScoreText(); // Baþlangýçta skoru güncelle
        UpdateTargetText(); // Hedef puaný güncelle
        levelCompletePanel.SetActive(false); // Seviye tamamlandý panelini kapalý baþlat
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
         
        UpdateScoreText(); // UI güncelle

        // Eðer hedef puana ulaþýldýysa
        if (currentScore >= targetScore)
        {
            LevelComplete();
        }
    }

    // Skor UI güncelleme fonksiyonu
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {currentScore}";
        }
    }

    // Hedef UI güncelleme fonksiyonu
    private void UpdateTargetText()
    {
        if (targetText != null)
        {
            targetText.text = $"Target: {targetScore}";
        }
    }

    // Seviye tamamlandýðýnda yapýlacak iþlemler
    private void LevelComplete()
    {
        Debug.Log("Level Complete!");
        levelCompletePanel.SetActive(true); // Seviye tamamlandý panelini göster
        Time.timeScale = 0; // Oyunu durdur
    }
}
