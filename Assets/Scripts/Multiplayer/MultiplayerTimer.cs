using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using TMPro;

public class MultiplayerTimer : MonoBehaviourPunCallbacks
{
    public TextMeshProUGUI timerText;
    private float remainingTime;
    public bool timerIsRunning = false;
    private const float TimerDuration = 10f;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)//if the client is the master client, starts timer
        {
            StartTimer();
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient && timerIsRunning)//if client is master client and timer is running, decrease timer.
        {
            remainingTime -= Time.deltaTime;
            UpdateRoomTimerProperty(remainingTime);

            if (remainingTime <= 0)//if timer is less than zero,it stops it and call on timerfinished
            {
                timerIsRunning = false;
                OnTimerFinished(); 
            }
        }
    }

    public void StartTimer()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            remainingTime = TimerDuration;
            timerIsRunning = true;
            UpdateRoomTimerProperty(remainingTime);
            UpdateTimerDisplay(); 
        }
    }
    void OnTimerFinished()//Creates a Hashtable containing a key-value"GameEnded" set to true and updates the rooms custom properties with this Hashtable.
    {
        var props = new ExitGames.Client.Photon.Hashtable
    {
        { "GameEnded", true }
    };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }



    //checks for changes in the "Timer" property and updates remainingTime and the timer display for all clients.
    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("Timer"))
        {
            remainingTime = (float)propertiesThatChanged["Timer"];
            UpdateTimerDisplay();

            if (remainingTime <= 0)
            {
                timerIsRunning = false;
                
            }
        }
    }
    void UpdateTimerDisplay()
    {
        timerText.text = FormatTime(remainingTime);
    }
    string FormatTime(float time)
    {
        return string.Format("{0:00}", (int)time);
    }
    void UpdateRoomTimerProperty(float time)// Updates the roomcustom properties with the time. This is how the timers state is shared across.
    {
        var props = new ExitGames.Client.Photon.Hashtable
        {
            { "Timer", time }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
}