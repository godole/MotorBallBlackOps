using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Gate : MonoBehaviour
{
    public Transform m_RevivePosition;
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
        if(other.gameObject.tag == "Player")
        {
            var character = other.gameObject.GetComponent<CharacterBase>();

            if (!character.m_PhotonView.IsMine)
                return;

            if (character.HasBall)
            {
                object score;
                if (!PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(character.m_TeamNumber.ToString(), out score))
                {
                    Debug.Log("Error Get Score");
                }

                Hashtable props = new Hashtable
                {
                    {character.m_TeamNumber.ToString(), (int)score + 1}
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props);
            }
        }
        GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>().SetCreatePosition(m_RevivePosition);
    }
}
