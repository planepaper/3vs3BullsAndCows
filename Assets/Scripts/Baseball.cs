using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Baseball : InteractiveObject
{
    bool stay=false;
    Collider player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stay)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                player.GetComponent<PlayerController>().ball++;
                TurnOffUI();
                Destroy(gameObject);
            }
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        TurnOnUI();
        stay = true;
        player = other;
    }

    private void OnTriggerStay(Collider other)
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        TurnOffUI();
        stay = false;
    }
}
