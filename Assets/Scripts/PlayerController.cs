using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public static int MaxHealth = 10;
    public int health = MaxHealth;
    public InteractiveObject interactObj;

    private Animator animator;
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private GameObject playerUIprefab;

    private bool isAlive = true;
    private bool isAttacking;
    private bool isInteract;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();
        if (playerUIprefab)
        {
            GameObject _ui = Instantiate(playerUIprefab);
            _ui.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUIPrefab reference on player Prefab", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((!photonView.IsMine && PhotonNetwork.IsConnected) || !isAlive)
        {
            return;
        }
        processInputs();

        if (isAttacking)
        {
            photonView.RPC("Attack", RpcTarget.All);
        }
        if (isInteract && interactObj != null) {
            interactObj.TurnOnUI();
        }
        if (health <= 0f)
        {
            photonView.RPC("Dead", RpcTarget.All);
            return;
        }
    }

    void LateUpdate()
    {
        animator.ResetTrigger("Attack");
    }

    private void processInputs()
    {
        isAttacking = Input.GetButtonDown("Fire1");
        isInteract = Input.GetButtonDown("Interact");
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }

    [PunRPC]
    private void Attack()
    {
        animator.SetTrigger("Attack");
        weapon.Attack();
    }

    [PunRPC]
    private void Dead()
    {
        //gameover;
        isAlive = false;
        Debug.Log($"{photonView.Owner.NickName} is retired");
        animator.SetTrigger("Death");

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.gameObject.tag == "Weapon" && other.gameObject != weapon)
        {
            health -= 1;
            Debug.Log($"{photonView.Owner.NickName}가 맞았습니다.\n 현재체력 : {health}");
        }
    }
}
