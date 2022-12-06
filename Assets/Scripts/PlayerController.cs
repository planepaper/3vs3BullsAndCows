using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    public static int MaxHealth = 10;
    public static int InitialBall = 0;
    public int health = MaxHealth;
    public int ball = InitialBall;
    public InteractiveObject interactObj;
    public GameObject spawnPoint;
    public string id;

    private Animator animator;
    private Collider collid;
    private Rigidbody rigid;
    private AudioSource audioSource;
    private PlayerMovement movementController;
    private Weapon weapon;
    private PlayerController killedBy;
    [SerializeField]
    private GameObject playerUIprefab;

    [SerializeField]
    private GameObject RespawnUIprefab;
    [SerializeField]
    private float knockbackPower;
    [SerializeField]
    private int respawnTime;
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
        id = PhotonNetwork.LocalPlayer.UserId;

        spawnPoint = GameObject.Find("SpawnPoint/Point1");
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
        if (interactObj != null)
        {
            interactObj.TurnOnUI();
        }
        if (isInteract && interactObj != null) {
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
        }
        else
        {
            health = (int)stream.ReceiveNext();
            ball = (int)stream.ReceiveNext();
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
        StartCoroutine("Respown");
    }

    private IEnumerator Respown() {
        GameObject respawnUIObject = null;
        Text respawnUI = null;
        if (photonView.IsMine)
        {
            respawnUIObject = Instantiate(RespawnUIprefab);
            respawnUIObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
            respawnUI = respawnUIObject.GetComponent<Text>();
        }
        for (int i = respawnTime; i > 0; i--) {
            if (respawnUI) {
                //Debug.Log($"Respwn... {i}");
                respawnUI.text = $"Respwn... {i}";
            }
            yield return new WaitForSeconds(1.0f);
        }
        isAlive = true;
        health = MaxHealth;
        killedBy = null;
        movementController.SetActive(true);
        transform.position = spawnPoint.transform.position;
        animator.SetTrigger("Respawn");
        if (photonView.IsMine)
        {
            Destroy(respawnUIObject);
        }
    }

    [PunRPC]
    private void OnDamaged(Vector3 hitPoint) {
        Vector3 direction = new Vector3(transform.position.x - hitPoint.x, 0, transform.position.z - hitPoint.z).normalized;
        rigid.AddForce(knockbackPower * direction, ForceMode.Impulse);
        health -= 1;
        audioSource.Play();
        //Debug.Log($"{photonView.Owner.NickName}가 맞았습니다.\n 현재체력 : {health}");
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Weapon" && other.gameObject != weapon)
        {
            photonView.RPC("OnDamaged", RpcTarget.All, collid.ClosestPoint(other.transform.position));
            killedBy = other.gameObject.GetComponentInParent<PlayerController>();
        }
        if(other.gameObject.tag == "Interactive")
        {
            interactObj = other.gameObject.GetComponent<InteractiveObject>();
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Interactive") {
            interactObj.TurnOffUI();
            interactObj = null;
        }
    }
}
