using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class UserInput : MonoBehaviour
{
    public CharacterBase m_Client;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Client == null)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            m_Client.LeftAttack();
        }

        if(Input.GetButtonDown("Fire2"))
        {
            m_Client.RightAttack();
        }

        if (Input.GetButtonDown("Fire3"))
        {
            m_Client.ThrowBall();
        }

        if(Input.GetKey(KeyCode.F))
        {
            m_Client.TakeOffBall();
        }

        if (Input.GetKey(KeyCode.F2))
            m_Client.CurrentHP = 0;
    }
}
