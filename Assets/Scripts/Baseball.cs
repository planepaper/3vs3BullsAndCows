using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Baseball : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject collisionUIPrefab;
    private static GameObject collisionUI;
    private static GameObject canvas;

    // 상태변수
    private bool isInteract;

    void Awake()
    {
        if (!collisionUI)
        {
            canvas = GameObject.FindWithTag("Canvas");
            collisionUI = Instantiate(collisionUIPrefab);
            collisionUI.transform.SetParent(canvas.transform, false);
            collisionUI.SetActive(false);
        }
    }

    private void Update()
    {
        isInteract = Input.GetKeyDown(KeyCode.E);
    }
    private void TurnOnCollisionUI()
    {
        collisionUI.SetActive(true);
    }
    private void TurnOffCollisionUI()
    {
        collisionUI.SetActive(false);
    }

    [PunRPC]
    private void RemoveBall()
    {
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController _player = other.gameObject.GetComponent<PlayerController>();
        if(!_player || !_player.photonView.IsMine) { return; }
        TurnOnCollisionUI();
    }

    private void OnTriggerStay(Collider other)
    {
        PlayerController _player = other.gameObject.GetComponent<PlayerController>();
        if (!_player) return;

        if (isInteract)
        {
            _player.AddBall();
            TurnOffCollisionUI();
            photonView.RPC(nameof(RemoveBall), RpcTarget.All);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerController _player = other.gameObject.GetComponent<PlayerController>();
        if (!_player || !_player.photonView.IsMine) { return; }
        TurnOffCollisionUI();
    }
}
