using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    [SerializeField]
    GameObject uiPrefab;
    GameObject ui;
    PlayerController player;
    bool isUIOpen= false;

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
        if (other.CompareTag("Player") && player == null)
        {

            player = other.gameObject.GetComponent<PlayerController>();
            other.gameObject.GetComponent<PlayerController>().interactObj = this;
            TurnOnUI();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("나감");
        if (other.gameObject == player.gameObject)
        {
            player = null;
            TurnOffUI();
        }
    }

    public void TurnOnUI() {
        if (isUIOpen) {
            return;
        }
        ui = Instantiate(uiPrefab);
        ui.transform.SetParent(GameObject.Find("Canvas").transform, false);
        isUIOpen = true;
    }

    public void TurnOffUI()
    {
        Debug.Log("지워짐");
        Destroy(ui);
        isUIOpen = false;
    }

}
