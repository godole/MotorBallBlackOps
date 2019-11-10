using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotorBall : MonoBehaviourPun, IPunObservable
{
    public float m_Speed;

    bool m_IsCatched = false;

    bool m_IsCatchEnable = true;

    Rigidbody m_RigidBody;

    public bool IsCatchEnable { get => m_IsCatchEnable; set => m_IsCatchEnable = value; }

    // Start is called before the first frame update
    void Awake()
    {
        m_RigidBody = GetComponent<Rigidbody>();

        if (!photonView.IsMine)
            Destroy(m_RigidBody);
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_IsCatched && photonView.IsMine)
            m_RigidBody.AddForce(new Vector3(0.0f, 0.0f, m_Speed));
    }

    public void Shot(Vector3 pos, Vector3 dir, float power)
    {
        if (!photonView.IsMine)
            return;

        dir *= power;
        if (m_RigidBody != null)
            m_RigidBody.AddForce(dir.x, dir.y, dir.z, ForceMode.Impulse);
    }

    public void Catched()
    {
        photonView.RPC("_Catched", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void _Catched()
    {
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
}
