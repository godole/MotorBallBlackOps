using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public GameObject m_Bullet;
    
    public int m_MaxBulletCapacity;
    public int m_CurBulletCapacity;
    public float m_ShotDelay;

    public override void OnStart()
    {
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].SetWeaponType(UIWeaponInfo.WEAPONTYPE_RANGE);
    }

    public override bool AttackCheck()
    {
        return m_CurBulletCapacity > 0 && IsAttackEnable;
    }

    public override void AttackDown(Vector3 dir)
    {
        m_CurBulletCapacity--;
        Character.CurBatteryCapacity -= BatteryReduce;
        IsAttackEnable = false;

        CreateBullet(dir);

        Character.PlayAnimation("Shooting", "Shooting");
    }

    public override void StartDelay()
    {
        StartCoroutine(ShotDelay());
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
        var bullet = b.GetComponent<Bullet>();
        bullet.MoveVector = dir;
        bullet.m_Team = Character.m_TeamNumber;
    }

    public void UIUpdate()
    {
        UIController.getInstance.PlayPanel.WeaponInfo[SlotIndex].CapacityText.text = m_CurBulletCapacity.ToString() + "/" + m_MaxBulletCapacity.ToString();
    }
}
