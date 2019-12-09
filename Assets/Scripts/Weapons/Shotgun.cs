using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : Weapon
{
    public int m_MaxBulletCapacity;
    public int m_CurBulletCapacity;
    public float m_Range;
    public float m_ShotDelay;
    public float m_ShotAngle;
    public int m_Damage;

    private void Start()
    {
        IsAttackEnable = true;
    }

    public override void OnStart()
    {
        if (Character.photonView.IsMine)
            UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].SetWeaponType(UIWeaponInfo.WEAPONTYPE_RANGE);
    }

    public override bool AttackCheck()
    {
        return m_CurBulletCapacity > 0 && IsAttackEnable;
    }

    public override void AttackDown(Vector3 dir)
    {
        if (!Character.photonView.IsMine)
            return;

        m_CurBulletCapacity--;
        Character.CurBatteryCapacity -= BatteryReduce;
        IsAttackEnable = false;

        GameObject.Find("DebugDraw").GetComponent<DebugDraw>().DrawBox(
            Character.transform.position + dir * m_Range / 2
            , new Vector3(3.0f, 2.0f, m_Range), 
            Quaternion.LookRotation(dir, Vector3.up));

        var cols = Physics.OverlapSphere(Character.transform.position, m_Range, 1 << 11);

        foreach (var col in cols)
        {
            CharacterBase character = col.gameObject.GetComponent<CharacterBase>();
            Vector3 deltaPos = col.transform.position - transform.position;
            float deltaangle = Quaternion.FromToRotation(dir, deltaPos).eulerAngles.y;

            if (deltaangle > 180.0f)
                deltaangle = -(360.0f - deltaangle);

            if (deltaangle > -m_ShotAngle && deltaangle < m_ShotAngle)
            {
                if (character.m_TeamNumber != Character.m_TeamNumber)
                    character.RPC("Hit", Photon.Pun.RpcTarget.AllBufferedViaServer, Vector3.zero, m_Damage);
            }
        }

        Character.PlayAnimation("Shooting", "Shooting");
    }

    public override void StartDelay()
    {
        StartCoroutine(ShotDelay());
    }

    IEnumerator ShotDelay()
    {
        yield return new WaitForSeconds(m_ShotDelay);
        IsAttackEnable = true;
    }

    public override void SetWeaponUI()
    {
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].CapacityText.text = m_CurBulletCapacity.ToString() + "/" + m_MaxBulletCapacity.ToString();
    }
}
