using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField]
    PlayerController owner;
    CapsuleCollider weaponCollider;
    AudioSource audioSource;

    void Start()
    {
        weaponCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();
        weaponCollider.enabled = false;
    }

    public void Swing() {
        StopCoroutine(SwingCoroutine());
        StartCoroutine(SwingCoroutine());
    }

    // 0.15뒤 collider 활성화 0.30뒤 비활성화
    private IEnumerator SwingCoroutine() {
        yield return new WaitForSeconds(0.10f);
        audioSource.Play();
        weaponCollider.enabled = true;
        yield return new WaitForSeconds(0.25f);
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController otherPlayer = other.gameObject.GetComponent<PlayerController>();
        if (otherPlayer)
        {
            bool isMine = otherPlayer == owner;
            //bool isTeam = otherPlayer.team == owner.team;
            if (!isMine)
            {
                otherPlayer.OnHit(owner, other.transform.position);
            }
        }
    }
}
