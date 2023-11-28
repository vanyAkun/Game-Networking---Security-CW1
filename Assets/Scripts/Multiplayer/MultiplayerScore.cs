using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;
using UnityEngine.UI;


public class MultiplayerScore : MonoBehaviourPunCallbacks
{
    public GameObject playerScorePrefab;
    public Transform panel;

    Dictionary<int, GameObject> playerScore = new Dictionary<int, GameObject>();//dic to store scores.
    void Start()//loops through each player in the list,sets score and  matches the player id with the score.
    {
        foreach (var player in PhotonNetwork.PlayerList)
        {
            player.SetScore(0);
            var playerScoreObject = Instantiate(playerScorePrefab, panel);
            var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
            playerScoreObjectText.text = string.Format("{0} Kills: {1}", player.NickName, player.GetScore());

            playerScore[player.ActorNumber] = playerScoreObject;
        }
    }
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (playerScore.ContainsKey(otherPlayer.ActorNumber))
        {
            // If the player who left is in the dictionary, remove their score display
            Destroy(playerScore[otherPlayer.ActorNumber]);
            playerScore.Remove(otherPlayer.ActorNumber);
        }
    }
    //access the playerscore dictionary to find the gameobject to reflect the latest scrore of the player and updates.
    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
    {
        var playerScoreObject = playerScore[targetPlayer.ActorNumber];
        var playerScoreObjectText = playerScoreObject.GetComponent<Text>();
        playerScoreObjectText.text = string.Format ("{0} Kills: {1}", targetPlayer.NickName, targetPlayer.GetScore());
    }
}
