using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    float m_BatteryCapacity;
    float m_BatteryConsume;
    float m_OverdriveLimit;
    float m_Weight;
    float m_Handling;
    float m_BrakingForce;
    float m_Endurance;
    float m_DefensePower;

    PartsBase[] m_Parts;

    public float BatteryCapacity    { get => m_BatteryCapacity; set => m_BatteryCapacity = value; }
    public float BatteryConsume     { get => m_BatteryConsume; set => m_BatteryConsume = value; }
    public float OverdriveLimit     { get => m_OverdriveLimit; set => m_OverdriveLimit = value; }
    public float Weight             { get => m_Weight; set => m_Weight = value; }
    public float Handling           { get => m_Handling; set => m_Handling = value; }
    public float BrakingForce       { get => m_BrakingForce; set => m_BrakingForce = value; }
    public float Endurance          { get => m_Endurance; set => m_Endurance = value; }
    public float DefensePower       { get => m_DefensePower; set => m_DefensePower = value; }

    public PartsBase GetPartsByIndex(int index)
    {
        return m_Parts[index];
    }
}
