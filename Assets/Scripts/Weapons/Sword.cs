﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public int m_AttackDamage;
    public float m_AttackDelay;
    public float m_AttackRange;
    public float m_AttackPower;

    public override void Attack()
    {
        if (IsAttackEnable)
        {
            Character.PlayAnimation("Melee Attack", "Melee Attack");

            IsAttackEnable = false;

            var hit = Physics.OverlapSphere(transform.position, m_AttackRange, 1 << 11);

            foreach (var h in hit)
            {
                var character = h.GetComponent<CharacterBase>();
                if (character.m_TeamNumber != Character.m_TeamNumber)
                {
                    Vector3 enemyPos = h.gameObject.transform.position;
                    Vector3 deltaPos = enemyPos - transform.position;
                    deltaPos.Normalize();
                    deltaPos *= m_AttackPower;
                    character.RPC("Hit", RpcTarget.AllBufferedViaServer, deltaPos, m_AttackDamage);
                }
            }
        }
    }

    public override void StartDelay()
    {
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(m_AttackDelay);
        IsAttackEnable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}