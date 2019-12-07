using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterBase Character { get => m_Character; set => m_Character = value; }

    CharacterBase m_Character;
    
    public bool IsAttackEnable { get => m_IsAttackEnable; set => m_IsAttackEnable = value; }
    public int SlotIndex { get => m_SlotIndex; set => m_SlotIndex = value; }
    public float BatteryReduce { get => m_BatteryReduce; set => m_BatteryReduce = value; }

    bool m_IsAttackEnable = true;

    int m_SlotIndex;
    [SerializeField] float m_BatteryReduce;

    public virtual void OnStart()
    {

    }

    public virtual bool AttackCheck()
    {
        return IsAttackEnable;
    }
    public virtual void AttackUp(Vector3 dir)
    {

    }

    public virtual void AttackDown(Vector3 dir)
    {

    }

    public virtual void Attacking(Vector3 dir)
    {

    }

    public virtual void Reload()
    {

    }
    public virtual void StartDelay()
    {

    }
    public virtual void SetWeaponUI()
    {

    }
}