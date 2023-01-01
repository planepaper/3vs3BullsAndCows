using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class PlayerUI : MonoBehaviour
{
    
    [SerializeField]
    private Text playerNameText;
    [SerializeField]
    private Slider PlayerHealthSlider;
    [SerializeField]
    private GameObject fillArea;
    [SerializeField]
    private Text ballAmountText;
    private PlayerController target;

    private void Awake()
    {
        PlayerHealthSlider.interactable = false;
        PlayerHealthSlider.maxValue = PlayerController.MaxHealth;
        ballAmountText.text = PlayerController.InitialBall.ToString();
    }

    void Update()
    {
        if (!target)
        {
            return;
        }
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

    public void SetTarget(PlayerController _target) {
        if (!_target) {
            return;
        }
        target = _target;
        playerNameText.text = target.photonView.Owner.NickName;
    }
}
