using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

public class Baseball : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("들어옴");
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("stay");
    }
}
