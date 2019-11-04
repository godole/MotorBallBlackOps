using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine;
using Photon.Pun;

public class CatchBall : MonoBehaviour
{
    public CharacterBase m_Character;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var ball = other.gameObject.GetComponent<MotorBall>();
        if (!ball.IsCatchEnable)
            return;

        Hashtable props = new Hashtable
        {
            {GameSceneManager.BALL_OWNER_CHANGE, m_Character.PlayerID}
        };
        Hashtable oldProps = new Hashtable
        {
            {GameSceneManager.BALL_OWNER_CHANGE, PhotonNetwork.CurrentRoom.CustomProperties[GameSceneManager.BALL_OWNER_CHANGE] }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props, oldProps);
    }
}
