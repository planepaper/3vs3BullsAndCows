using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Baseball : InteractiveObject
{
    [PunRPC]
    private void Destroy() {
        TurnOffUI();
        Destroy(gameObject);
    }

    override public void Interact(GameObject other) {
        other.GetComponent<PlayerController>().ball++;
        photonView.RPC("Destroy", RpcTarget.All);
    }
}
