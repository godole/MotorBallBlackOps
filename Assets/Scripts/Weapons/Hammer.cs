using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer : Weapon
{
    public int m_AttackMaxDamage;
    public float m_AttackDelay;
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

    public override void AttackDown(Vector3 dir)
    {
        if (!Character.photonView.IsMine)
            return;

        UIController.getInstance.PlayPanel.ThrowGageSlider.gameObject.SetActive(true);
    }

    public override void AttackUp(Vector3 dir)
    {
        if (!IsAttackEnable)
            return;

        Character.PlayAnimation("Melee Attack", "Melee Attack");

        if (!Character.photonView.IsMine)
            return;

        IsAttackEnable = false;

        UIController.getInstance.PlayPanel.ThrowGageSlider.gameObject.SetActive(false);

        var rb = Character.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(dir * m_AttackMaxPower, ForceMode.Impulse);

        Vector3 center = transform.position + dir * m_AttackHeight / 2;
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

        Character.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);

        m_AttackDamage = 0.0f;
        m_AttackPower = 0.0f;

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

        UIController.getInstance.PlayPanel.ThrowGageSlider.value = m_AttackDamage / m_AttackMaxDamage;
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(m_AttackDelay);
        IsAttackEnable = true;
    }
}
