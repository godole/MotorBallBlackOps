using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;
using Hashtable = ExitGames.Client.Photon.Hashtable;

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

        if (m_Client.IsInPitstop)
            return;

        if (Input.GetButtonDown("Fire1"))
        {
            m_Client.AttackCheck(0);
        }

        if (Input.GetButtonUp("Fire1"))
        {
            m_Client.RPC("AttackUp", Photon.Pun.RpcTarget.AllBufferedViaServer, 0, m_Client.m_Cam.ShotDirection);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            m_Client.AttackCheck(1);
        }

        if (Input.GetButtonUp("Fire2"))
        {
            m_Client.RPC("AttackUp", Photon.Pun.RpcTarget.AllBufferedViaServer, 1, m_Client.m_Cam.ShotDirection);
        }

        if (Input.GetButton("Fire1"))
        {
            m_Client.RPC("Attacking", Photon.Pun.RpcTarget.AllBufferedViaServer, 0, m_Client.m_Cam.ShotDirection);
        }

        if (Input.GetButton("Fire2"))
        {
            m_Client.RPC("Attacking", Photon.Pun.RpcTarget.AllBufferedViaServer, 1, m_Client.m_Cam.ShotDirection);
        }

        if (Input.GetButtonDown("Fire3"))
        {
            m_Client.ThrowStart();

        }

        if (Input.GetButtonUp("Fire3"))
        {
            m_Client.RPC("ThrowBall", Photon.Pun.RpcTarget.AllViaServer,
                m_Client.m_Cam.transform.forward, m_Client.ThrowChargningPower);
        }

        if(Input.GetButtonDown("Jump"))
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");

            m_Client.Boost(new Vector2(h, v));
        }


        if (Input.GetKeyDown(KeyCode.F))
        {
            m_Client.TakeOffBall();
        }

        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_Client.Reverse();
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_Client.Reverse();
        }

        if (Input.GetKey(KeyCode.F2))
            m_Client.CurrentHP = 0;

        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_Client.LockOn();
        }
    }
}
