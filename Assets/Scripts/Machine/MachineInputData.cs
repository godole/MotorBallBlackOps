using System.Collections;
using System.Collections.Generic;

public class MachineInputData
{
    public enum EMachineExtendStat
    {
        BatteryCapacity,
        BatteryConsume,
        BatteryUseConsume,
        OverdriveLimit,
        MaximumSpeed,
        AcceleratingForce,
        Overdrive,
        Speed,
        Acceleration,
        Weight,
        Handling,
        BrakingForce,
        Endurance,
        DefensePower,
        Damage,
        SprayPenetration,
        AttackSpeed,
        ReloadSpeed,
        ChargeTime,
        LockOnTime
    }

    public enum EMachineBaseStat
    {
        Type,
        Grade,
        BatteryEfficient,
        EnginePower,
        Mobility,
        Armor,
        FirePower
    }

    Dictionary<EMachineExtendStat, int> m_ExtendData;
    Dictionary<EMachineBaseStat, int> m_BaseStats;

    public void SetCoefficientData(EMachineExtendStat type, int data)
    {
        m_ExtendData[type] = data;
    }

    public void SetMachineBaseStat(EMachineBaseStat type, int data)
    {
        m_BaseStats[type] = data;
    }

    public void CalculateMachineStat(MachineBase machine)
    {

    }
}
