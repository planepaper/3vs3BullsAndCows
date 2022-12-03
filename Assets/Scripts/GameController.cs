using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance;
    public GameObject player;

    private void Start()
    {
        Instance = this;
        player = PhotonNetwork.Instantiate("Player", new Vector3(-172, 5f, 225), Quaternion.identity);
    }
}
