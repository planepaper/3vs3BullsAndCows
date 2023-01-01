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

    public void Attack() {
        StopCoroutine(Swing());
        StartCoroutine(Swing());
    }

    // 0.15뒤 collider 활성화 0.30뒤 비활성화
    private IEnumerator Swing() {
        yield return new WaitForSeconds(0.10f);
        audioSource.Play();
        weaponCollider.enabled = true;
        yield return new WaitForSeconds(0.25f);
        weaponCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        GameObject otherObject = other.gameObject;
        if (otherObject.tag == "Player")
        {
            PlayerController otherPlayer = otherObject.GetComponent<PlayerController>();
            bool isMine = otherObject == owner;
            bool isTeam = otherPlayer.team == owner.team;
            if (!isMine)
            {
                otherPlayer.OnHit(owner, other.transform.position);
            }
        }
    }
}
