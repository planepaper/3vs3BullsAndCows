using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private string gameVersion = "1";

    public Text connectText;

    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        connectText.text = "Connecting...";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connectText.text = "Connected";
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectText.text = "Fail to Connect. Try it again.";
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            connectText.text = "Connect to Room";
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        connectText.text = "Success to join in the room";
        PhotonNetwork.LoadLevel("NetworkDemoScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectText.text = "Fail to join in the room. Make new room.";
        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 2 });
    }
}
