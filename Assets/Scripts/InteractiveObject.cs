using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject uiPrefab;
    protected GameObject ui;
    protected GameObject canvas;
    bool isUIOpen = false;

    protected void Start()
    {
        canvas = GameObject.FindWithTag("Canvas");
    }

    public void TurnOnUI()
    {
        if (isUIOpen)
        {
            return;
        }
        Debug.Log("UI 호출");
        ui = Instantiate(uiPrefab);
        ui.transform.SetParent(canvas.transform, false);
        isUIOpen = true;
    }

    public void TurnOffUI()
    {
        if (!isUIOpen)
        {
            return;
        }
        Destroy(ui);
        isUIOpen = false;
    }

    abstract public void Interact(GameObject other);

}
