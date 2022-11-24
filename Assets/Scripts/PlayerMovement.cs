using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    public static int MaxHealth = 10;
    public int health = MaxHealth;

    private Rigidbody rigid;
    private Animator animator;
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private GameObject playerUIprefab;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float lookSensitivity;
    private bool isAttacking;



    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();

        CameraMovement camera = GetComponent<CameraMovement>();
        if (photonView.IsMine) {
            camera.OnStartFollowing();
        }
        if (playerUIprefab)
        {
            GameObject _ui = Instantiate(playerUIprefab);
            _ui.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUIPrefab reference on player Prefab", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected) {
            return;
        }
        processInputs();

        if (isAttacking)
        {
            photonView.RPC("Attack", RpcTarget.All);
        }
        Move();
        Rotation();

        if (health <= 0f) {
            //gameover;
            Debug.Log($"{photonView.Owner.NickName} is retired");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }
        else
        {
            health = (int)stream.ReceiveNext();
        }
    }

    private void Move() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveHorizontal = transform.right * h;
        Vector3 moveVertical = transform.forward * v;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;
        rigid.MovePosition(transform.position + velocity * Time.deltaTime);
        animator.SetFloat("Horizontal", h, h * 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", v, v * 0.1f, Time.deltaTime);
    }

    private void Rotation() {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 charactorRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        transform.rotation = transform.rotation * Quaternion.Euler(charactorRotationY);

    }

    private void processInputs() {
        isAttacking = Input.GetButtonDown("Fire1");
    }

    [PunRPC]
    private void Attack() {
        animator.SetTrigger("Attack");
        weapon.Attack();
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
