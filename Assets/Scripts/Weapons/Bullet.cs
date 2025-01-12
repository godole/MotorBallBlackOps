﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float m_BulletSpeed;

    public float m_LifeTime;

    public int m_AttackDamage;

    public int m_Team;

    Vector3 m_MoveVector;

    public Vector3 MoveVector { get => m_MoveVector; set => m_MoveVector = value; }

    public GameObject m_ExplosionEff;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LifeTime());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + MoveVector * m_BulletSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var character = other.GetComponent<CharacterBase>();

            if (m_Team != character.m_TeamNumber)
            {
                Instantiate(m_ExplosionEff, transform.position, Quaternion.identity);
                character.RPC("Hit", Photon.Pun.RpcTarget.AllViaServer, Vector3.zero, m_AttackDamage);
                StopAllCoroutines();
                Destroy(gameObject);
            }
        }
        
        else if(other.CompareTag("Wall"))
        {
            StopAllCoroutines();
            Destroy(gameObject);
        }
    }

    IEnumerator LifeTime()
    {
        yield return new WaitForSeconds(m_LifeTime);
        Destroy(gameObject);
    }
}
