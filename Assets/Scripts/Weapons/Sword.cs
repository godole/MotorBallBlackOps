﻿using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : Weapon
{
    public int m_AttackDamage;
    public float m_AttackDelay;
    public float m_AttackingDelay;
    public float m_AttackWidth;
    public float m_AttackHeight;
    public float m_AttackPower;

    public override void OnStart()
    {
        if (Character.photonView.IsMine)
            UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].SetWeaponType(UIWeaponInfo.WEAPONTYPE_MELEE);
    }

    public override void AttackDown(Vector3 dir)
    {
        Character.PlayAnimation("Melee Attack", "Melee Attack");

        IsAttackEnable = false;

        if (!Character.photonView.IsMine)
            return;

        StartCoroutine(AttackingDelay(dir));
    }

    IEnumerator AttackingDelay(Vector3 dir)
    {
        yield return new WaitForSeconds(m_AttackingDelay);
        Character.CurBatteryCapacity -= BatteryReduce;

        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].MeleeNotReady();

        //var hit = Physics.OverlapSphere(transform.position, m_AttackRange, 1 << 11);
        Vector3 center = Character.transform.position + dir * m_AttackHeight / 2;
        Vector3 size = new Vector3(m_AttackWidth, 1.0f, m_AttackHeight);
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        var hit = Physics.OverlapBox(
            center,
            size / 2,
            rot, 1 << 11);

        GameObject.Find("DebugDraw").GetComponent<DebugDraw>().DrawBox(center, size, rot);

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

    public override void StartDelay()
    {
        StartCoroutine(AttackDelay());
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(m_AttackDelay);
        IsAttackEnable = true;
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].MeleeReady();
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
