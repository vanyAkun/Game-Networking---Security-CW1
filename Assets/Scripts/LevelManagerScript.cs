using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static Player;

public class LevelManagerScript : MonoBehaviour
{
    public int enemies = 5;
    public Text enemiesText;
    public Text timerText; // Text to display the timer
    public float timer = 2f; //  countdown
    public Text YouWinText;
    public Text YouLoseText;
    public Player player;
    public Text KillcountText;
    public Text deathText;

    public GameObject playAgainButton;
    public GameObject mainMenuButton;
    private void Awake()
    {
        Player.OnPlayerKilled += UpdateDeathCountDisplay;//Subscribes to static event from Player class. When a player is killed, it updates the death count display
        enemiesText.text = enemies.ToString();

        Enemy.OnEnemyKilled += OnEnemyKilled;
      
        UpdateTimerDisplay();
        if (timerText != null)
        {
            UpdateTimerDisplay();
        }
        playAgainButton.SetActive(false);

    }
    private void UpdateDeathCountDisplay()
    {
        if (player != null && deathText != null)
        {
            deathText.text = player.respawnCount.ToString();
        }
       
    }
    private void Update()
    {

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            UpdateTimerDisplay();
        }
        else if (!playAgainButton.activeSelf) 
        {
            CheckGameEndCondition();
        }
    }

    void OnEnemyKilled()
    {
        enemies--;
        enemiesText.text = enemies.ToString();

        if (enemies <= 0)
        {
            CheckGameEndCondition();
        }

    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MenuScene_Main"); 
        mainMenuButton.SetActive(false);
    }
    private void UpdateTimerDisplay()
    {
        if (timerText != null) 
        {
            timerText.text = Mathf.Ceil(timer).ToString();
        }
    }

    private void CheckGameEndCondition()
    {
        timer = 0; 
        UpdateTimerDisplay(); 

        int enemiesKilled = 5 - enemies; // Calculate enemies killed
        KillcountText.text = "Enemies Killed: " + enemiesKilled.ToString();
        KillcountText.gameObject.SetActive(true); 

        if (enemies > 0)
        {
            YouLoseText.gameObject.SetActive(true);
        }
        else
        {
            YouWinText.gameObject.SetActive(true);
        }

        playAgainButton.SetActive(true);
        mainMenuButton.SetActive(true);
    }
  
    public void PlayAgain()
    {
        playAgainButton.SetActive(false); 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
