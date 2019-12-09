using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon
{
    public int m_AttackMaxDamage;
    public float m_AttackDelay;
    public float m_AttackingDelay;
    public float m_AttackWidth;
    public float m_AttackHeight;
    public float m_AttackMaxPower;

    float m_AttackDamage;
    float m_AttackPower;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnStart()
    {
        if (Character.photonView.IsMine)
            UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].SetWeaponType(UIWeaponInfo.WEAPONTYPE_MELEE);
    }

    public override void AttackDown(Vector3 dir)
    {
        if (!Character.photonView.IsMine)
            return;

        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].ChargeSlider.gameObject.SetActive(true);
    }

    public override void AttackUp(Vector3 dir)
    {
        if (!IsAttackEnable)
            return;

        Character.PlayAnimation("Melee Attack", "Melee Attack");

        Character.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        var rb = Character.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * m_AttackMaxPower, ForceMode.Impulse);

        StartCoroutine(AttackingDelay(dir));
        StartCoroutine(AttackDelay());
    }

    public override void Attacking(Vector3 dir)
    {
        if (!Character.photonView.IsMine)
            return;

        m_AttackDamage += m_AttackMaxDamage * Time.deltaTime;
        if (m_AttackDamage > m_AttackMaxDamage)
            m_AttackDamage = m_AttackMaxDamage;

        m_AttackPower += m_AttackMaxPower * Time.deltaTime;
        if (m_AttackPower > m_AttackMaxPower)
            m_AttackPower = m_AttackMaxPower;

        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].ChargeSlider.value = m_AttackDamage / m_AttackMaxDamage;
    }

    IEnumerator AttackingDelay(Vector3 dir)
    {
        yield return new WaitForSeconds(m_AttackingDelay);
        if (!Character.photonView.IsMine)
            yield return null;

        Character.CurBatteryCapacity -= BatteryReduce;

        IsAttackEnable = false;

        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].MeleeNotReady();
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].ChargeSlider.gameObject.SetActive(false);

        Vector3 center = Character.transform.position + dir * m_AttackHeight / 4;
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

        m_AttackDamage = 0.0f;
        m_AttackPower = 0.0f;

        
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(m_AttackDelay);
        IsAttackEnable = true;
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].MeleeReady();
    }
}
