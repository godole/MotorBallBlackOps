using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class Gate : MonoBehaviour
{
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
            ActiveNextGate();
        }

        GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>().SetCreatePosition(m_RevivePosition);
    }

    private void ActiveNextGate()
    {
        var gates = GameObject.FindGameObjectsWithTag("Gate");

        foreach (var item in gates)
        {
            Gate g = item.GetComponent<Gate>();
            if (ID + 1 == g.ID || (ID == 7 && g.ID == 1))
            {
                IsActive = false;
                g.IsActive = true;
                break;
            }
        }
    }
}
