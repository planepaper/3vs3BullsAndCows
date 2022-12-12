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
    public Vector3 spawnPoint;
    public char team;

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
        weapon.team = team;
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
        GameObject respawnUIObject = null;
        Text respawnUI = null;
        if (photonView.IsMine)
        {
            respawnUIObject = Instantiate(RespawnUIprefab);
            respawnUIObject.transform.SetParent(GameObject.Find("Canvas").transform, false);
            respawnUI = respawnUIObject.GetComponent<Text>();
        }
        for (int i = respawnTime; i > 0; i--)
        {
            if (respawnUI)
            {
                //Debug.Log($"Respwn... {i}");
                respawnUI.text = $"Respwn... {i}";
            }
            yield return new WaitForSeconds(1.0f);
        }
        Respown();
        if (photonView.IsMine)
        {
            Destroy(respawnUIObject);
        }
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
