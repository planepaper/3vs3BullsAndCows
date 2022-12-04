using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public static int MaxHealth = 10;
    public static int InitialBall = 0;
    public int health = MaxHealth;
    public InteractiveObject interactObj;
    public int ball = InitialBall;

    private Animator animator;
    private Collider collider;
    private Rigidbody rigid;
    private AudioSource audioSource;
    private Weapon weapon;
    [SerializeField]
    private GameObject playerUIprefab;

    [SerializeField]
    private float knockbackPower;
    private bool isAlive = true;
    private bool isAttacking;
    private bool isInteract;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collider = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>();
        audioSource = GetComponent<AudioSource>();
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

    private void OnDamaged(Vector3 hitPoint) {
        Vector3 direction = new Vector3(transform.position.x - hitPoint.x, 0, transform.position.z - hitPoint.z).normalized;
        Debug.Log(direction);
        rigid.AddForce(knockbackPower * direction, ForceMode.Impulse);
        health -= 1;
        audioSource.Play();
        Debug.Log($"{photonView.Owner.NickName}가 맞았습니다.\n 현재체력 : {health}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.gameObject.tag == "Weapon" && other.gameObject != weapon)
        {
            OnDamaged(collider.ClosestPoint(other.transform.position));
        }
    }
}
