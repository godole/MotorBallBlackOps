using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Gate : MonoBehaviourPunCallbacks
{
    const string ACTIVE_GATE = "active_gate";
    bool m_IsActive = false;
    [SerializeField]
    int m_ID;
    public Transform m_RevivePosition;

    public bool IsActive { get => m_IsActive; set => m_IsActive = value; }
    public int ID { get => m_ID; set => m_ID = value; }

    // Start is called before the first frame update
    void Start()
    {
        if (m_ID == 1)
            IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsActive)
            return;

        if (other.gameObject.tag == "Player")
        {
            var character = other.gameObject.GetComponent<CharacterBase>();
            
            if (!character.photonView.IsMine)
                return;

            if (character.HasBall)
            {
                ActiveNextGate();

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
        
        else if(other.gameObject.tag == "Ball")
        {
            if (!other.GetComponent<PhotonView>().IsMine)
                return;

            ActiveNextGate();
        }
    }

    private void ActiveNextGate()
    {
        NetworkTool.SetCustomPropertiesSafe(ACTIVE_GATE, (int)ID);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        var props = propertiesThatChanged;

        if(props.ContainsKey(ACTIVE_GATE))
        {
            if ((int)props[ACTIVE_GATE] + 1 == ID || ((int)props[ACTIVE_GATE] == 2 && ID == 1))
            {
                IsActive = true;
            }
            else
                IsActive = false;
        }
    }
}
