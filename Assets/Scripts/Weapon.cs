using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public char team;
    CapsuleCollider weaponCollider;
    AudioSource audioSource;

    void Start()
    {
        weaponCollider = GetComponent<CapsuleCollider>();
        audioSource = GetComponent<AudioSource>();

        weaponCollider.enabled = false;
    }

    public void Attack() {
        StopCoroutine("Swing");
        StartCoroutine("Swing");
    }

    // 0.15뒤 collider 활성화 0.30뒤 비활성화
    private IEnumerator Swing() {
        yield return new WaitForSeconds(0.10f);
        audioSource.Play();
        weaponCollider.enabled = true;
        yield return new WaitForSeconds(0.25f);
        weaponCollider.enabled = false;
    }
}
