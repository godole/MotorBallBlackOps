using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public CharacterBase Character { get => m_Character; set => m_Character = value; }

    CharacterBase m_Character;
    
    public bool IsAttackEnable { get => m_IsAttackEnable; set => m_IsAttackEnable = value; }

    bool m_IsAttackEnable = true;

    public virtual bool AttackCheck()
    {
        return IsAttackEnable;
    }
    public virtual void Attack()
    {

    }
    public virtual void StartDelay()
    {

    }
}