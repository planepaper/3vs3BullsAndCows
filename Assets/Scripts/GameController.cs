using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class GameController : MonoBehaviourPunCallbacks
{
    public static GameController Instance;
    public GameObject player;
    public List<GameObject> ballPositions;
    public List<GameObject> activeBalls;
    public List<GameObject> respownPoints;

    public SafeBox safeBoxA;
    public SafeBox safeBoxB;

    public Text[] teamAText;
    public Text[] teamBText;

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

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("StartGame", RpcTarget.All);
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

    private void Update()
    {

    }

    public void UpdateGuessBoard()
    {
        for (int i = 0; i < safeBoxA.textIndex; i++)
        {
            teamAText[i].text = MakeGuessResultString(safeBoxA, i);
        }
        for (int i = 0; i < safeBoxB.textIndex; i++)
        {
            teamBText[i].text = MakeGuessResultString(safeBoxB, i);
        }
    }

    [PunRPC]
    public void StartGame()
    {
        // Bulls And Cows Number Setting
        BullsAndCows bullsAndCows = new BullsAndCows();

        safeBoxA.SetFourNumbers(bullsAndCows.GetRandomNumber());
        safeBoxB.SetFourNumbers(bullsAndCows.GetRandomNumber());



        // Respawn Everybody to specific position

        // 
    }
    private string MakeGuessResultString(SafeBox safebox, int i)
    {
        string guessResult = safebox.guessNumbers[i * 4 + 0].ToString()
    + safebox.guessNumbers[i * 4 + 1].ToString()
    + safebox.guessNumbers[i * 4 + 2].ToString()
    + safebox.guessNumbers[i * 4 + 3].ToString() +
    " strike : " + safebox.guessResults[i * 2 + 0] +
    " ball : " + safebox.guessResults[i * 2 + 1];

        return guessResult;
    }
}
