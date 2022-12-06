using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    
    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Slider PlayerHealthSlider;
    [SerializeField]
    private GameObject fillArea;
    [SerializeField]
    private Vector3 screenOffset;
    private float characterControllerHeight = 1.4f;

    [SerializeField]
    private Text ballAmountText;
    private PlayerController target;

    private void Awake()
    {
        //this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        //fillArea = transform.Find("Fill Area").gameObject;
        PlayerHealthSlider.interactable = false;
        PlayerHealthSlider.maxValue = PlayerController.MaxHealth;
        ballAmountText.text = PlayerController.InitialBall.ToString();
    }

    void Update()
    {
        if (PlayerHealthSlider) {
            PlayerHealthSlider.value = target.health;
        }
        if (PlayerHealthSlider.value <= 0)
        {
            fillArea.SetActive(false);
        }
        else {
            fillArea.SetActive(true);
        }
        if (!target) {
            Destroy(this.gameObject);
            return;
        }
        ballAmountText.text = target.ball.ToString();
    }
    private void LateUpdate()
    {
        if (target) {
            Vector3 targetPosition;
            targetPosition = target.transform.position;
            targetPosition.y += characterControllerHeight;
            this.transform.position = targetPosition;
            this.transform.LookAt(Camera.main.transform);

            //this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
        }
    }

    public void SetTarget(PlayerController _target) {
        if (!_target) {
            return;
        }
        target = _target;
        playerNameText.text = target.photonView.Owner.NickName;

        CharacterController _charactorController = _target.GetComponent<CharacterController>();
        if (_charactorController) {
            characterControllerHeight = _charactorController.height;
        }
    }
}
