using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    private Transform transform;
    private Animator animator;
    [SerializeField]
    private float walkSpeed = 6.0f;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        transform = GetComponent<Transform>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        bool attack = Input.GetButtonDown("Jump");
        transform.Translate(new Vector3(h * walkSpeed * Time.deltaTime, 0, v * walkSpeed * Time.deltaTime));
        animator.SetFloat("Horizontal", h, h * 0.1f, Time.deltaTime);
        animator.SetFloat("Vertical", v, v * 0.1f, Time.deltaTime);
        if (attack)
        {
            animator.SetTrigger("Attack");
        }
    }
}
