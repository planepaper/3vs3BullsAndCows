using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance;
    public GameObject player;
    public List<GameObject> ballPositions;
    public List<GameObject> activeBalls;
    


    private void Start()
    {
        Instance = this;
        player = PhotonNetwork.Instantiate("Player", new Vector3(-172, 5f, 225), Quaternion.identity);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InstantiateBalls();
    }

    private void InstantiateBalls() {
        if (!PhotonNetwork.IsMasterClient) {
            return;
        }
        int num = Random.Range(0, 4);
        Debug.Log(num);
        Transform[] posArr = ballPositions[num].GetComponentsInChildren<Transform>();
        foreach (Transform pos in posArr)
        {
            if (pos.position == Vector3.zero)
            {
                continue;
            }
            activeBalls.Add(PhotonNetwork.Instantiate("Baseball", pos.position, Quaternion.identity));
        }
    }
}
