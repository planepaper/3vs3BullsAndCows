using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NetworkController : MonoBehaviourPunCallbacks
{
    private string gameVersion = "2";

    [SerializeField]
    private Text connectText;
    [SerializeField]
    private Button connectButton;
    [SerializeField]
    InputField playerNameInputField;



    private void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;
        connectText.text = "Connecting...";
        connectButton.enabled = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connectText.text = "Connected";
        connectButton.enabled = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        connectText.text = "Fail to Connect. Try it again.";
        connectButton.enabled = false;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            connectText.text = "Connect to Room";
            connectButton.enabled = false;
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnJoinedRoom()
    {
        connectText.text = "Success to join in the room";
        PhotonNetwork.NickName = playerNameInputField.text;
        PhotonNetwork.LoadLevel("SampleScene");
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        connectText.text = "Fail to join in the room. Make new room.";
        Debug.Log($"{returnCode} : {message}");
        connectButton.enabled = false;
        PhotonNetwork.CreateRoom("test", new RoomOptions { MaxPlayers = 2 });
    }
}
