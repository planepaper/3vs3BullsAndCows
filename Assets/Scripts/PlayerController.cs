using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    
    public const int MAX_HEALTH = 10;
    public int health = MAX_HEALTH;
    public int ball = 0;
    ///
    public InteractiveObject interactObj;
    public Vector3 spawnPoint;
    public char team;
    ///

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
    }

    // Update is called once per frame
    void Update()
    {
        if ((!photonView.IsMine && PhotonNetwork.IsConnected) || !isAlive) return;
        

        isAttacking = Input.GetKeyDown(KeyCode.Mouse0);
        if (isAttacking)
        {
            photonView.RPC(nameof(Attack), RpcTarget.All);
        }

        isInteract = Input.GetKeyDown(KeyCode.E);
        if (isInteract && interactObj != null)
        {
            interactObj.Interact(this.gameObject);
        }

        if (health <= 0f)
        {
            photonView.RPC(nameof(Dead), RpcTarget.All);
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
        weapon.Swing();
    }

    [PunRPC]
    private void Dead()
    {
        isAlive = false;
        if(killedBy != null)
            killedBy.ball += ball;
        ball = 0;
        movementController.SetActive(false);
        animator.SetTrigger("Death");
        StartCoroutine(EnableRespownUI());
    }

    private IEnumerator EnableRespownUI()
    {
        if (photonView.IsMine)
        {
            respawnUI.SetActive(true);
            Text UItext = respawnUI.GetComponent<Text>();
            for (int i = respawnTime; i > 0; i--)
            {
                UItext.text = $"Respwn... {i}";
                yield return new WaitForSeconds(1.0f);
            }
            respawnUI.SetActive(false);
        }
        else
        {
            yield return new WaitForSeconds(respawnTime);
        }
        Respown();
    }

    private void Respown()
    {
        isAlive = true;
        health = MAX_HEALTH;
        killedBy = null;
        movementController.SetActive(true);
        animator.SetTrigger("Respawn");
        transform.position = spawnPoint;
    }

    //야구배트 피격 관련 함수들
    public void OnHit(PlayerController hitBy, Vector3 hitPoint) {
        photonView.RPC(nameof(OnDamaged), RpcTarget.All, collid.ClosestPoint(hitPoint));
        killedBy = hitBy;
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
        InteractiveObject otherObject = other.gameObject.GetComponent<InteractiveObject>();
        if (otherObject)
        {
            interactObj = otherObject;
            interactObj.TurnOnUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractiveObject otherObject = other.gameObject.GetComponent<InteractiveObject>();
        if (otherObject)
        {
            interactObj.TurnOffUI();
            interactObj = null;
        }
    }

}
