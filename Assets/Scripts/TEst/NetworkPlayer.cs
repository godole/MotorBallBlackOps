using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayer : MonoBehaviourPun
{
    public GameObject localCam;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            localCam.SetActive(false);

            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

            for(int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] is NetworkPlayer)
                    continue;
                else if (scripts[i] is PhotonView)
                    continue;
                else if (scripts[i] is PhotonTransformViewClassic)
                    continue;

                scripts[i].enabled = false;
            }

            var rigidbody = GetComponent<Rigidbody>();
            Destroy(rigidbody);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
