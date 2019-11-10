using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class NetworkTool : MonoBehaviour
{
    // Start is called before the first frame update
    public static void SetCustomPropertiesSafe(string key, object value)
    {
        Hashtable props = new Hashtable
                {
                    {key, value}
                };
        Hashtable oldProps = new Hashtable
                {
                    {key, PhotonNetwork.CurrentRoom.CustomProperties[key] }
                };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props, oldProps);
    }
}
