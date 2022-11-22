using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviourPun
{
    public static GameController Instance;
    public GameObject player;

    private void Start()
    {
        Instance = this;
        player = PhotonNetwork.Instantiate("Player", new Vector3(3.7f, 0f, 0f), Quaternion.identity);
    }

    private void Update()
    {
    }
}
