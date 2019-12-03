using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitstop : MonoBehaviour
{
    CharacterBase m_Character = null;
    private void Start()
    {
    }

    private void Update()
    {
        if (m_Character == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            Vector3 exitPos = Vector3.zero;
            Quaternion exitRot = Quaternion.identity;

            if (props.ContainsKey(GameSceneManager.CREATE_POSITION))
            {
                exitPos = (Vector3)props[GameSceneManager.CREATE_POSITION];
            }

            if (props.ContainsKey(GameSceneManager.CREATE_ROTATION))
            {
                exitRot = (Quaternion)props[GameSceneManager.CREATE_ROTATION];
            }

            m_Character.RPC("ChangeWeapon", RpcTarget.AllBufferedViaServer, "Shotgun", 0);
            m_Character.ExitPitstop(exitPos, exitRot);

            m_Character = null;
        }
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<CharacterBase>();

        if (character == null)
            return;

        if (character.HasBall)
            return;

        if (!character.photonView.IsMine)
            return;

        m_Character = character;
        character.EnterPitstop();
    }
}
