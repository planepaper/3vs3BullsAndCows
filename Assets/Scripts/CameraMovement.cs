using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    private float distance = 2f;
    [SerializeField]
    private float height = 1f;
    [SerializeField]
    private Vector3 centerOffset = Vector3.zero;
    [SerializeField]
    private bool followOnStart = false;


    private Transform cameraTransform;
    private Vector3 cameraOffset = Vector3.zero;
    private bool isFollowing;

    // Start is called before the first frame update
    void Start()
    {
        if (followOnStart)
        {
            OnStartFollowing();
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraTransform == null & isFollowing)
        {
            OnStartFollowing();
        }

        if(isFollowing)
        {
            Follow();
        }

    }
    public void OnStartFollowing() {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
        Cut();
    }

    private void Follow() {
        Vector2 direction = new Vector2(this.transform.forward.x, this.transform.forward.z) * distance;
        cameraOffset.x = -direction.x;
        cameraOffset.y = height;
        cameraOffset.z = -direction.y;


        cameraTransform.position = this.transform.position + cameraOffset;
        cameraTransform.LookAt(this.transform.position + centerOffset);
    }
    private void Cut() {
        Vector2 direction = new Vector2(this.transform.forward.x, this.transform.forward.z) * distance;
        cameraOffset.x = -direction.x;
        cameraOffset.y = height;
        cameraOffset.z = -direction.y;

        cameraTransform.position = this.transform.position + cameraOffset;
        cameraTransform.LookAt(this.transform.position + centerOffset);
    }
}
