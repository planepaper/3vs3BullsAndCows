using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigid;
    private Animator animator;
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float lookSensitivity;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Attack();
    }
    void FixedUpdate()
    {
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
        }
    }
}
