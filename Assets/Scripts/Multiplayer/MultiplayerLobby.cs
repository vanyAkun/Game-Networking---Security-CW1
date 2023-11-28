using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using NUnit.Framework;



public class MultiplayerLobby : MonoBehaviourPunCallbacks 
{
    public Transform LoginPanel;
    public Transform SelectionPanel;
    public Transform CreateRoomPanel;
    public Transform InsideRoomPanel;
    public Transform ListRoomsPanel;
    public Transform insideRoomPlayerList;
    public Transform listRoomPanel; 
    public Transform listRoomPanelContent;//reference to the placewhere we will be placing and removing on-screen entries for available rooms.
 


    public InputField roomNameInput;
    public InputField playerNameInput;

    public GameObject textPrefab;
    public GameObject roomEntryPrefab;
    public GameObject startGameButton;
   
    string playerName;

    Dictionary<string, RoomInfo> cachedRoomList;

  private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 1000000)); 

        cachedRoomList = new Dictionary<string, RoomInfo>();//dictionary to store room info
        PhotonNetwork.AutomaticallySyncScene = true;
    }

  public void LoginButtonClicked()
    {
        PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;
        PhotonNetwork.ConnectUsingSettings();
    }
  public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }
    public void StartGameClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen=false;
        PhotonNetwork.CurrentRoom.IsVisible=false;//makes current room private
        PhotonNetwork.LoadLevel("GameScene_PlayerBattle");
    }
  public void ListRoomsClicked() //This is where we will start to use the Lobby in the Photon network.
    {
        PhotonNetwork.JoinLobby();
    }
  public void ActivatePanel(string PanelName)
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);

        if (PanelName == LoginPanel.gameObject.name)
            LoginPanel.gameObject.SetActive(true);
        else if (PanelName == SelectionPanel.gameObject.name)
            SelectionPanel.gameObject.SetActive(true);
        else if (PanelName == CreateRoomPanel.gameObject.name)
            CreateRoomPanel.gameObject.SetActive(true);
        else if (PanelName == InsideRoomPanel.gameObject.name)
            InsideRoomPanel.gameObject.SetActive(true);
        else if (PanelName == ListRoomsPanel.gameObject.name)
            ListRoomsPanel.gameObject.SetActive(true);
    }
  public void CreateARoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;

        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions );
    }
  public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
  
  public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        ActivatePanel("ListRooms");
    }
  public override void OnRoomListUpdate(List<RoomInfo>roomList)//basically updates when theres changes in the list of the rooms
    {
        Debug.Log("Room Update: " + roomList.Count); //printing a message to the console of the number of roomsavailable.
                                                     //We will only be doing this the moment we join the lobby/open the roomlist since the method is a callback method
        DestroyChildren(listRoomPanelContent);

        UpdateCachedRoomList(roomList);

        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent); //start of our room list generation code.line ofcode that is creating the individual room entries  for the list but these entries won'thave any details in them, nor will their Join button actually work
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();       //or each room entry on the list, we are getting access directly to it's RoomEntryscript. We are storing this access under a new variable newRoomEntryScript

            newRoomEntryScript.roomName = room.Key;                               // We are then taking the rooms network name, and applying it to the variables insideits RoomEntry script.
            newRoomEntryScript.roomText.text = string.Format("[{0} - ({1}/{2})]", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers); //We are then also taking this name, the current number of players,
                                                                                                                                 //and the numberof max players, for each individual room, and applying this to the UI element
                                                                                                                                 //thatdisplays the rooms details.We are repeating this entire process for each individual room,
                                                                                                                                 //so that even if thereare multiple entries on the room list, they are have their own details alone,
                                                                                                                                 //andthey are correct

        }

    }
  public override void OnLeftRoom()
    {
        Debug.Log("Room left");
        ActivatePanel("CreateRoom");

        DestroyChildren(insideRoomPlayerList);
    }
  public override void OnDisconnected(DisconnectCause cause)
    {
        ActivatePanel("Login");
    }
  public override void OnConnectedToMaster()
    {
        Debug.Log("Master Server Connected!!");
        ActivatePanel("Selection");
    }
  public override void OnCreatedRoom()
    {
        Debug.Log("Room has been created");
    }
  public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to create room");
    }
  public override void OnJoinedRoom()
    {
        Debug.Log("Room has been joined");
        ActivatePanel("InsideRoom");
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);//if master client button visible.

        foreach (var player in PhotonNetwork.PlayerList) //updates player list
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName;
        }
    }
  public void DestroyChildren(Transform parent) // this method to pass the transform component of any gameobjec and it will destroy every child object contained with in, so with this all text entries will be deleted(player name,room,etc)
    {
        foreach (Transform child in parent)//find the Transform component in child to parent variables.ist of text prefabs that will contain the player names.
                                           //Each of these prefabs will automatically have a Transform componen //which willbe the child Transforms that are found.
                                           //If there are 10 child Transforms, then our foreach loop will repeat 10 times, one foreach individual found Transform
        {
            Destroy(child.gameObject); //We will set up code so that we pass our player list into this method, which willthen check this list for any child Transforms and clear them
        }
    }
  public void LeaveLobbyClicked() //The method will be invoked by the Leave Lobby button
    {
        PhotonNetwork.LeaveLobby();
    }
  public override void OnLeftLobby() //being a callback, will then be automatically called, taking our player back to themultiplayer menu where they can choose what to do
    {
        Debug.Log("Left Lobby");
        DestroyChildren(listRoomPanelContent);
        DestroyChildren(insideRoomPlayerList);
        cachedRoomList.Clear(); 
        ActivatePanel("Selection");
    }
  public void UpdateCachedRoomList(List<RoomInfo> roomList)//iterates through room list
    {
        foreach (var room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)//if room nor open,available/removed form list it gets removed from the cacheroomlist
                cachedRoomList.Remove(room.Name);
            else
                cachedRoomList[room.Name] = room;  //room added or updated in the cachedroomlist with room name as the key. 
        }
    }
  public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)//updates ui with players in room. adds new entry if new player joins.
    {
        Debug.Log("A Player joined the Room");
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;
        playerListEntry.name = newPlayer.NickName;

    }
  public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    { 
        foreach (Transform child in insideRoomPlayerList)//iterates through list
        {
            if (child.name == otherPlayer.NickName)//checks if nickna,e matches player who left
            {
                Destroy(child.gameObject);//boom destroys player name from list
                break;
            }
        }
    }
  
    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom(); 
    }
  public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join room" + message);
    }
  public override void OnJoinRandomFailed (short returnCode, string message)
    {
        Debug.Log("Failed to join random room" + message);
    }
    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
}
