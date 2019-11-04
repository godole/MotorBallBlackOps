using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour, IPunObservable
{
    Rigidbody m_RigidBody;
    PhotonView m_PhotonView;

    Vector3 networkPosition = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    Quaternion networkRotation = Quaternion.identity;

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_RigidBody.position);
            stream.SendNext(m_RigidBody.rotation);
            stream.SendNext(m_RigidBody.velocity);
        }
        else
        {
            networkPosition = (Vector3)stream.ReceiveNext();
            networkRotation = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_RigidBody = GetComponent<Rigidbody>();
        m_PhotonView = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_PhotonView.IsMine)
        {
            m_RigidBody.velocity = velocity;
            m_RigidBody.position = Vector3.Lerp(m_RigidBody.position, networkPosition, Time.fixedDeltaTime * 7.0f);
            m_RigidBody.rotation = Quaternion.Lerp(m_RigidBody.rotation, networkRotation, Time.fixedDeltaTime * 7.0f);
        }
    }
}
