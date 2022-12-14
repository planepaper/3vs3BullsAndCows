using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviourPunCallbacks
{
    // 서버
    public bool teamAFinishGame = false;
    public bool teamBFinishGame = false;

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
            player = PhotonNetwork.Instantiate("PlayerB", new Vector3(-140, 5f, 298), Quaternion.identity);
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
        if (teamAFinishGame)
        {
            Debug.Log("teama");
            // PhotonNetwork.LoadLevel("TeamAWinScene");
            SceneManager.LoadScene("TeamAWinScene");
        }
        if (teamBFinishGame)
        {
            Debug.Log("teamb");
            // PhotonNetwork.LoadLevel("TeamBWinScene");
            SceneManager.LoadScene("TeamBWinScene");
        }
    }

    [PunRPC]
    public void UpdateGuessBoard()
    {
        photonView.RPC("UpdateGuessBoardImpl", RpcTarget.All);
    }

    [PunRPC]
    public void UpdateGuessBoardImpl()
    {
        for (int i = 0; i < safeBoxA.textIndex; i++)
        {
            teamBText[i].text = MakeGuessResultString(safeBoxA, i);
            if (safeBoxA.guessNumbers[i * 4 + 0] == safeBoxA.fourNumbers[0] &&
            safeBoxA.guessNumbers[i * 4 + 1] == safeBoxA.fourNumbers[1] &&
            safeBoxA.guessNumbers[i * 4 + 2] == safeBoxA.fourNumbers[2] &&
            safeBoxA.guessNumbers[i * 4 + 3] == safeBoxA.fourNumbers[3])
            {
                Debug.Log("haha");
                teamBFinishGame = true;
            }

        }
        for (int i = 0; i < safeBoxB.textIndex; i++)
        {
            teamAText[i].text = MakeGuessResultString(safeBoxB, i);

            if (safeBoxB.guessNumbers[i * 4 + 0] == safeBoxB.fourNumbers[0] &&
            safeBoxB.guessNumbers[i * 4 + 1] == safeBoxB.fourNumbers[1] &&
            safeBoxB.guessNumbers[i * 4 + 2] == safeBoxB.fourNumbers[2] &&
            safeBoxB.guessNumbers[i * 4 + 3] == safeBoxB.fourNumbers[3])
            {

                teamAFinishGame = true;
            }
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(teamAFinishGame);
            stream.SendNext(teamBFinishGame);
        }
        else
        {
            teamAFinishGame = (bool)stream.ReceiveNext();
            teamBFinishGame = (bool)stream.ReceiveNext();
        }
    }
}
