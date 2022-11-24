using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerMovement : MonoBehaviourPunCallbacks
{
    private Rigidbody rigid;
    private Animator animator;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float lookSensitivity;
    [SerializeField]
    private Weapon weapon;

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>();
        gameObject.GetComponentInChildren<Camera>().tag = "MainCamera";
        /*
        if (photonView.IsMine) {
            gameObject.GetComponentInChildren<Camera>().tag = "MainCamera";
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (!photonView.IsMine) {
            return;
        }
        */
        Attack();
        
    }
    void FixedUpdate()
    {
        /*
        if (!photonView.IsMine)
        {
            return;
        }
        */
        Move();
        Rotation();
    }

    void Move() {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 moveHorizontal = transform.right * h;
        Vector3 moveVertical = transform.forward * v;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * walkSpeed;
        rigid.MovePosition(transform.position + velocity * Time.deltaTime);
        animator.SetFloat("Horizontal", h, h * 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", v, v * 0.1f, Time.deltaTime);
    }
    void Rotation() {
        float yRotation = Input.GetAxisRaw("Mouse X");
        Vector3 charactorRotationY = new Vector3(0f, yRotation, 0f) * lookSensitivity;
        rigid.MoveRotation(rigid.rotation * Quaternion.Euler(charactorRotationY));
    }
    void Attack() {
        bool attack = Input.GetButtonDown("Fire1");

        if (attack)
        {
            animator.SetTrigger("Attack");
            weapon.Attack();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Weapon" && other.gameObject != weapon)
        {
            Debug.Log($"{gameObject.name}가 맞았습니다.");
        }
    }
}
