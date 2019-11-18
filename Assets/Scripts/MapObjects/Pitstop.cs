using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitstop : MonoBehaviour
{
    public Transform m_EndPoint;
    GameSceneManager m_GameManager;

    private void Start()
    {
        m_GameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<CharacterBase>();
        if(character != null)
        {
            var props = PhotonNetwork.CurrentRoom.CustomProperties;
            Vector3 exitPos = Vector3.zero;
            Quaternion exitRot = Quaternion.identity;

            if(props.ContainsKey(GameSceneManager.CREATE_POSITION))
            {
                exitPos = (Vector3)props[GameSceneManager.CREATE_POSITION];
            }

            if (props.ContainsKey(GameSceneManager.CREATE_ROTATION))
            {
                exitRot = (Quaternion)props[GameSceneManager.CREATE_ROTATION];
            }
            character.EnterPitstop(exitPos, exitRot);
        }
    }
}
