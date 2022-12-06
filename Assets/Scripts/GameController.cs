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
        GameObject[] ball1 = GameObject.FindGameObjectsWithTag("ball1");
        GameObject[] ball2 = GameObject.FindGameObjectsWithTag("ball2");
        GameObject[] ball3 = GameObject.FindGameObjectsWithTag("ball3");
        GameObject[] ball4 = GameObject.FindGameObjectsWithTag("ball4");
        Instance = this;
        player = PhotonNetwork.Instantiate("Player", new Vector3(-172, 5f, 225), Quaternion.identity);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        for(int i = 0; i < 10; i++)
        {
            ball1[i].SetActive(false);
        }
        for (int i = 0; i < 10; i++)
        {
            ball2[i].SetActive(false);
        }
        for (int i = 0; i < 10; i++)
        {
            ball3[i].SetActive(false);
        }
        for (int i = 0; i < 10; i++)
        {
            ball4[i].SetActive(false);
        }
        int num = Random.Range(0, 4);
        Debug.Log(num);
        switch (num)
        {
            case 0:
                for (int i = 0; i < 10; i++)
                {
                    ball1[i].SetActive(true);
                }
                break;
            case 1:
                for (int i = 0; i < 10; i++)
                {
                    ball2[i].SetActive(true);
                }
                break;
            case 2:
                for (int i = 0; i < 10; i++)
                {
                    ball3[i].SetActive(true);
                }
                break;
            case 3:
                for (int i = 0; i < 10; i++)
                {
                    ball4[i].SetActive(true);
                }
                break;
        }
    }
}
