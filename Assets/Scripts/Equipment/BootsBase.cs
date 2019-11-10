using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootsBase : EquipItem
{
    float m_MaximumSpeed;
    float m_AcceleratingForce;

    public float MaximumSpeed { get => m_MaximumSpeed; set => m_MaximumSpeed = value; }
    public float AcceleratingForce { get => m_AcceleratingForce; set => m_AcceleratingForce = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
