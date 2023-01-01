using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    public static int MaxHealth = 10;
    public static int InitialBall = 0;
    public int health = MaxHealth;
    public int ball = InitialBall;
    public InteractiveObject interactObj;
    public Vector3 spawnPoint;
    public char team;

    //컴포넌트
    private Animator animator;
    private Collider collid;
    private Rigidbody rigid;
    private AudioSource audioSource;
    private PlayerMovement movementController;
    private Weapon weapon;
    private PlayerController killedBy;

    // UI관련 컴포넌트
    private GameObject respawnUI;
    private PlayerUI playerUI;

    // 프리팹
    [SerializeField]
    private GameObject RespawnUIprefab;

    [SerializeField]
    private float knockbackPower;
    [SerializeField]
    private int respawnTime;

    // 상태변수
    private bool isAlive = true;
    private bool isAttacking;
    private bool isInteract;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        collid = GetComponent<CapsuleCollider>();
        rigid = GetComponent<Rigidbody>();
        weapon = GetComponentInChildren<Weapon>();
        audioSource = GetComponent<AudioSource>();
        movementController = GetComponent<PlayerMovement>();
        playerUI = FindObjectOfType<PlayerUI>();
        playerUI.SetTarget(this);
        respawnUI = Instantiate(RespawnUIprefab);
        respawnUI.transform.SetParent(GameObject.Find("Canvas").transform, false);
        respawnUI.SetActive(false);
        weapon.team = team;
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
        if (isInteract && interactObj != null)
        {
            interactObj.Interact(this.gameObject);
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

    public bool UseBall() {
        if (ball > 0)
        {
            ball--;
            return true;
        }
        else {
            return false;
        }
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
            stream.SendNext(ball);
            stream.SendNext(spawnPoint);

        }
        else
        {
            health = (int)stream.ReceiveNext();
            ball = (int)stream.ReceiveNext();
            spawnPoint = (Vector3)stream.ReceiveNext();
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
        isAlive = false;
        Debug.Log(killedBy.photonView.Owner.NickName);
        killedBy.ball += ball;
        ball = 0;
        movementController.SetActive(false);
        animator.SetTrigger("Death");
        StartCoroutine("WaitRespown");
    }

    private IEnumerator WaitRespown()
    {
        Text UItext = null;
        respawnUI.SetActive(true);
        UItext = respawnUI.GetComponent<Text>();
        for (int i = respawnTime; i > 0; i--)
        {
            UItext.text = $"Respwn... {i}";
            yield return new WaitForSeconds(1.0f);
        }
        respawnUI.SetActive(false);
        Respown();
    }

    private void Respown()
    {
        isAlive = true;
        health = MaxHealth;
        killedBy = null;
        movementController.SetActive(true);
        animator.SetTrigger("Respawn");
        if (photonView.IsMine)
        {
            transform.position = spawnPoint;
        }

    }

    [PunRPC]
    private void OnDamaged(Vector3 hitPoint)
    {
        Vector3 direction = new Vector3(transform.position.x - hitPoint.x, 0, transform.position.z - hitPoint.z).normalized;
        rigid.AddForce(knockbackPower * direction, ForceMode.Impulse);
        health -= 1;
        audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.tag == "Weapon")
        {
            bool isMine = otherObject == weapon.gameObject;
            bool isTeam = otherObject.GetComponent<Weapon>().team == team;
            if (!isMine)
            {
                photonView.RPC("OnDamaged", RpcTarget.All, collid.ClosestPoint(other.transform.position));
                killedBy = other.gameObject.GetComponentInParent<PlayerController>();
            }
        }
        if (otherObject.tag == "Interactive")
        {
            interactObj = other.gameObject.GetComponent<InteractiveObject>();
            interactObj.TurnOnUI();
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Interactive")
        {
            interactObj.TurnOffUI();
            interactObj = null;
        }
    }

}
