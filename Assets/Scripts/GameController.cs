using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviourPun
{
    public GameObject player;

    private void Start()
    {
        player = PhotonNetwork.Instantiate("Player", new Vector3(0f, 0.35f, -3f), Quaternion.identity);
    }

    private void Update()
    {
    }
}
