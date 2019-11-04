using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public GameObject lobbyCam;
    public Transform[] spawnPoint;
    public GameObject lobbyUI;
    public Text statusText;
    public const string version = "1.0";
    public const string roomName = "MultiPlayer";
    public string playerPrefabName = "Player";
    // Start is called before the first frame update

    private void Awake()
    {
        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if(props.ContainsKey("PlayerCount"))
        {
            int id = (int)props["PlayerCount"];
            PhotonNetwork.Instantiate(playerPrefabName, spawnPoint[id].position, spawnPoint[id].rotation, 0);
            id++;

            Hashtable prop = new Hashtable()
            {
                { "PlayerCount", id }
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(prop);
        }
    }

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
