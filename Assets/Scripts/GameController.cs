using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance;
    public GameObject player;
    public char team;
    public List<GameObject> ballPositions;
    public List<GameObject> activeBalls;
    public List<GameObject> respownPoints;

    public SafeBox safeBoxA;
    public SafeBox safeBoxB;


    private void Start()
    {
        Instance = this;
        PlayerController playerController;

        if (PhotonNetwork.CurrentRoom.PlayerCount % 2 == 1)
        {
            player = PhotonNetwork.Instantiate("PlayerA", new Vector3(-172, 5f, 225), Quaternion.identity);
            playerController = player.gameObject.GetComponent<PlayerController>();
            playerController.spawnPoint = respownPoints[0].transform.position;
            playerController.team = 'A';
        }
        else
        {
            player = PhotonNetwork.Instantiate("PlayerB", new Vector3(-172, 5f, 225), Quaternion.identity);
            playerController = player.gameObject.GetComponent<PlayerController>();
            playerController.spawnPoint = respownPoints[1].transform.position;
            playerController.team = 'B';
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        InstantiateBalls();
    }

    private void InstantiateBalls()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
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

    private void StartGame()
    {
        // Bulls And Cows Number Setting
        BullsAndCows bullsAndCows = new BullsAndCows();

        safeBoxA.SetFourNumbers(bullsAndCows.GetRandomNumber());
        safeBoxB.SetFourNumbers(bullsAndCows.GetRandomNumber());



        // Respawn Everybody to specific position

        // 
    }
}
