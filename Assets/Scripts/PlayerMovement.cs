using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;

public class PlayerMovement : MonoBehaviourPunCallbacks
{

    private Rigidbody rigid;
    private Animator animator;
    [SerializeField]
    private Weapon weapon;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float jumpPower;

    [SerializeField]
    private float lookSensitivity;
    private bool isJumping;



    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();
        //character = GetComponent<CharacterController>();
        CameraMovement camera = GetComponent<CameraMovement>();
        if (photonView.IsMine || !PhotonNetwork.IsConnected) {
            camera.OnStartFollowing();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if ((!photonView.IsMine && PhotonNetwork.IsConnected)) {
            return;
        }
        processInputs();
        Move();
        Rotation();
    }

    private void processInputs()
    {
        isJumping = Input.GetButtonDown("Jump");
    }



    private void Move() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveHorizontal = transform.right * h;
        Vector3 moveVertical = transform.forward * v;


        Vector3 velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;
        rigid.MovePosition(rigid.position + velocity * Time.deltaTime);
        animator.SetFloat("Horizontal", h, h * 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", v, v * 0.1f, Time.deltaTime);
        animator.SetBool("Move", h != 0 || v != 0);
        if (isJumping)
        {
            animator.SetTrigger("Jump");
            rigid.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void Rotation() {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 charactorRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        transform.rotation = transform.rotation * Quaternion.Euler(charactorRotationY);
    }
}
