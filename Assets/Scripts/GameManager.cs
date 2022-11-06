using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    
    void Awake()
    {
        Screen.SetResolution(1080, 1920, false);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster() {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
