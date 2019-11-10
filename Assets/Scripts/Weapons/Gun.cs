﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    public GameObject m_Bullet;
    
    public int m_MaxBulletCapacity;
    public int m_CurBulletCapacity;
    public float m_ShotDelay;

    GameSceneManager m_GameManager;

    public override void Attack()
    {
        if (IsAttackEnable && m_CurBulletCapacity > 0)
        {
            m_CurBulletCapacity--;
            IsAttackEnable = false;

            CreateBullet(Character.m_Cam.transform.forward);
            
            Character.PlayAnimation("Shooting", "Shooting");
        }
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

    // Start is called before the first frame update
    void Start()
    {
        m_CurBulletCapacity = m_MaxBulletCapacity;
        m_GameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
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
        m_GameManager.m_MaxBulletCapacity.text = m_MaxBulletCapacity.ToString();
        m_GameManager.m_CurBulletCapacity.text = m_CurBulletCapacity.ToString();
    }
}