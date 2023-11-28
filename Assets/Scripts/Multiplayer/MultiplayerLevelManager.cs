using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using TMPro;

public class MultiplayerLevelManager : MonoBehaviourPunCallbacks
{
    //public int maxKills = 3; game finishes after the timer ends so nomaxkill needed
    public GameObject gameOverPopup;
    public Text winnerText;
    public MultiplayerTimer multiplayerTimer;
    public TextMeshProUGUI notificationText;

    void Start()
    {
        PhotonNetwork.Instantiate("Multiplayer Player", Vector3.zero, Quaternion.identity);
    }
    public void OnTimerEnd()
    {
        Photon.Realtime.Player winner = DetermineWinner();

        if (winner != null && winner.GetScore() > 0)
        {
            // at least one kill
            winnerText.text = winner.NickName + " wins!";
        }
        else
        {
            // If no kills
            winnerText.text = "Nobody won!";
        }

        gameOverPopup.SetActive(true);
    }

    [PunRPC]
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)//specifically who left
    {
        base.OnPlayerLeftRoom(otherPlayer);//check base
        ShowNotification($"{otherPlayer.NickName} has left the game");

    } 

    [PunRPC]
 
    private Photon.Realtime.Player DetermineWinner()
    {
        int highestScore = 0;
        Photon.Realtime.Player winner = null;
        foreach (var player in PhotonNetwork.PlayerList)//iterates through list to see who has highest score
        {
            if (player.GetScore() > highestScore)//checks if player score is higher than highestscore
            {
                highestScore = player.GetScore();//makes the player with the highest score the winner
                winner = player;
            }
        }
        return highestScore > 0 ? winner : null;//fancy IF ELSE//basically if highest score more than 0 determine winner, if not null. 
    }
    //A hashtable is a collection of key-value pairs that are organized based on the hash code of the key.
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {// Checks if the game has ended and calls OnTimerEnd if it has.

        if (propertiesThatChanged.ContainsKey("GameEnded") && (bool)propertiesThatChanged["GameEnded"])
        {
            OnTimerEnd();
        }
    }

    [PunRPC]
    public void EndGame()
    {
        OnTimerEnd();
    }

    [PunRPC]
    public void OnPlayAgainClicked()
    {
        // Call the RPC on all clients to reset
        photonView.RPC("ResetGame", RpcTarget.All);

    }
    [PunRPC]
    void ResetGame()
    {
        ResetTimer();
        ResetScores();

        // hide game over popup
        gameOverPopup.SetActive(false);
        SceneManager.LoadScene("GameScene_PlayerBattle");

    }
    void ResetTimer()
    {
        if (multiplayerTimer != null)
        {
            multiplayerTimer.StartTimer(); // Reset and start the timer
        }
    }
    void ResetScores()
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
        }
    }
    public void LeaveGame()
    {
        PhotonNetwork.LeaveRoom();
    }
    public override void OnLeftRoom()
    {
        PhotonNetwork.Disconnect();

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        string message = cause == DisconnectCause.DisconnectByClientLogic ?
                         "You have left the game" : "Connection lost";// does this even work
        ShowNotification(message);
        //Invoke("ClearNotification", 2.0f);
        SceneManager.LoadScene("MenuScene_Main"); // Redirect to main menu
        //should activate login panel??
        
    }
    private void ShowNotification(string message)
    {
        if (notificationText != null)
            notificationText.text = message;
        Invoke("ClearNotification", 2.0f);
       
    }
    private void ClearNotification()
    {
        if (notificationText != null)
            notificationText.text = ""; // Clears text
    }
}

