using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RoomEntry : MonoBehaviour
{
    public Text roomText; //these two vars are where room net data is going to be stored.
    public string roomName;
    
    public void JoinRoom() //When the player will press the Join button that will be displayed next to eachindividual room,
                           //it is this method that will be called. When it is, it first ensures thatwe are leaving the lobby
    {
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom(roomName);
    }
}
