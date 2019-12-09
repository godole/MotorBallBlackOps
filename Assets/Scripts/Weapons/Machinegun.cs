using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machinegun : Weapon
{
    public GameObject m_Bullet;

    public int m_MaxBulletCapacity;
    public int m_CurBulletCapacity;
    public float m_ShotDelay;
    
    float m_CurShotDelay = 0.0f;

    public override void OnStart()
    {
        if (Character.photonView.IsMine)
            UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].SetWeaponType(UIWeaponInfo.WEAPONTYPE_RANGE);
    }

    public override bool AttackCheck()
    {
        return true;
    }

    public override void AttackDown(Vector3 dir)
    {
        
    }

    public override void Attacking(Vector3 dir)
    {
        if (m_CurBulletCapacity < 1)
            return;
        
        m_CurShotDelay += Time.deltaTime;

        if(m_CurShotDelay > m_ShotDelay)
        {
            m_CurShotDelay = 0.0f;
            m_CurBulletCapacity--;

            CreateBullet(dir);

            Character.CurBatteryCapacity -= BatteryReduce;

            Character.PlayAnimation("Shooting", "Shooting");
        }
    }

    public override void Reload()
    {
        m_CurBulletCapacity = m_MaxBulletCapacity;
    }

    public override void SetWeaponUI()
    {
        UIUpdate();
    }

    IEnumerator ShotDelay()
    {
        yield return new WaitForSeconds(m_ShotDelay);
        IsAttackEnable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        IsAttackEnable = true;
        m_CurBulletCapacity = m_MaxBulletCapacity;
    }

    // Update is called once per frame
    void Update()
    {
    }

    void CreateBullet(Vector3 dir)
    {
        var b = Instantiate(m_Bullet, transform.position + dir * 2.0f, transform.rotation);
        b.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
        var bullet = b.GetComponent<Bullet>();
        bullet.MoveVector = dir;
        bullet.m_Team = Character.m_TeamNumber;
    }

    public void UIUpdate()
    {
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].CapacityText.text = m_CurBulletCapacity.ToString() + "/" + m_MaxBulletCapacity.ToString();
    }
}
