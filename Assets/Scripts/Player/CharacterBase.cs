using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterBase : MonoBehaviourPunCallbacks, IPunObservable
{
    Rigidbody m_RigidBody;

    GameSceneManager m_GameManager;

    public float m_TakeOffRange;

    bool m_IsFront = true;
    bool m_HasBall = false;
    bool m_IsHitBullet = false;
    [SerializeField]
    int m_PlayerID = -1;

    [SerializeField]
    float m_ThrowPower = 0;

    public GameObject m_Cam;

    public GameObject m_CharacterMesh;

    public GameObject m_DestroyEff;

    public GameObject m_Ball;
    public bool HasBall { get => m_HasBall; set => m_HasBall = value; }
    public int PlayerID { get => m_PlayerID; set => m_PlayerID = value; }
    public bool IsHitBullet { get => m_IsHitBullet; set => m_IsHitBullet = value; }
    public int CurrentHP { get => m_CurrentHP; set => m_CurrentHP = value; }
    public bool IsFront { get => m_IsFront; set => m_IsFront = value; }

    public int m_MaxHP;
    int m_CurrentHP;

    public int m_TeamNumber = -1;

    [SerializeField]
    Animator m_Animator;

    public GameObject m_RedTeamSign;
    public GameObject m_BlueTeamSign;

    public Weapon[] m_Weapons;

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        m_RigidBody = GetComponent<Rigidbody>();

        m_GameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();

        CurrentHP = m_MaxHP;

        if (photonView.IsMine)
        {
            m_PlayerID = m_GameManager.m_LocalID;

            if(m_PlayerID % 2 == 1)
            {
                m_TeamNumber = GameSceneManager.RED_TEAM;
                m_RedTeamSign.SetActive(true);
            }
            else
            {
                m_TeamNumber = GameSceneManager.BLUE_TEAM;
                m_BlueTeamSign.SetActive(true);
            }
        }
        else
        {
            MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();

            for (int i = 0; i < scripts.Length; i++)
            {
                if (scripts[i] is CharacterBase)
                    continue;
                else if (scripts[i] is PhotonView)
                    continue;
                else if (scripts[i] is PhotonTransformViewClassic)
                    continue;
                else if (scripts[i] is PhotonAnimatorView)
                    continue;

                scripts[i].enabled = false;
            }

            Destroy(m_RigidBody);
        }

        foreach (var weapon in m_Weapons)
        {
            weapon.Character = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_Ball.SetActive(m_HasBall);
        
        m_GameManager.m_HPUI[m_PlayerID - 1].value = CurrentHP / (float)m_MaxHP;

        

        if(m_CurrentHP <= 0)
        {
            Instantiate(m_DestroyEff, transform.position, Quaternion.identity);

            if (HasBall)
                PhotonNetwork.Instantiate("Ball", transform.position, new Quaternion(), 0);

            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine)
            return;

        m_Cam.GetComponent<CustomFreeLookCam>().SetTarget(null);

        m_GameManager.RevivePlayer();
    }

    public void RPC(string method, RpcTarget target, params object[] parameters)
    {
        photonView.RPC(method, target, parameters);
    }

    public void Reverse()
    {
        IsFront = !IsFront;
        m_CharacterMesh.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
        m_Cam.GetComponent<CustomFreeLookCam>().IsFront = IsFront;
    }

    public void EnterPitstop(Vector3 endPoint, Quaternion rotation)
    {
        if (!photonView.IsMine)
            return;

        m_CurrentHP = m_MaxHP;
        transform.position = endPoint;
        transform.rotation = rotation;
        gameObject.GetComponent<MachineBase>().ResetVelocity();
        m_RigidBody.velocity = Vector3.zero;
    }

    public void AttackCheck(int index)
    {
        if(m_Weapons[index].AttackCheck())
        {
            RPC("Attack", RpcTarget.AllViaServer, index);
            m_Weapons[index].StartDelay();
        }
    }

    [PunRPC]
    void Attack(int index)
    {
        m_Weapons[index].Attack();
    }

    public void TakeOffBall()
    {
        if (!m_HasBall)
        {
            var hit = Physics.OverlapSphere(transform.position, m_TakeOffRange, 1 << 11);

            foreach (var h in hit)
            {
                var character = h.gameObject.GetComponent<CharacterBase>();

                if (!character.m_HasBall)
                    continue;

                NetworkTool.SetCustomPropertiesSafe(GameSceneManager.BALL_OWNER_CHANGE, m_PlayerID);
            }
        }
    }

    [PunRPC]
    void Hit(Vector3 force, int dmg)
    {
        m_Animator.Play("Hit", m_Animator.GetLayerIndex("Hit"));
        m_CurrentHP -= dmg;
        if (photonView.IsMine)
            m_RigidBody.AddForce(force, ForceMode.Impulse);
    }

    [PunRPC]
    void ThrowBall(Vector3 dir)
    {
        if (!HasBall || !photonView.IsMine)
            return;

        m_HasBall = false;

        NetworkTool.SetCustomPropertiesSafe(GameSceneManager.BALL_OWNER_CHANGE, -2);

        var ball = PhotonNetwork.Instantiate("Ball", 
            transform.position + dir * 5.0f, new Quaternion());
        ball.GetComponent<MotorBall>().Shot(transform.position, dir, m_ThrowPower);
    }

    public void PlayAnimation(string name, string layerName)
    {
        m_Animator.Play(name, m_Animator.GetLayerIndex(layerName));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_PlayerID);
            stream.SendNext(CurrentHP);
        }
        else
        {
            m_PlayerID = (int)stream.ReceiveNext();
            CurrentHP = (int)stream.ReceiveNext();
        }
    }

    IEnumerator DecreaseByBullet()
    {
        if (IsHitBullet)
            yield return null;

        IsHitBullet = true;
        yield return new WaitForSeconds(1.0f);
        IsHitBullet = false;
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        var props = propertiesThatChanged;

        if (props.ContainsKey(GameSceneManager.BALL_OWNER_CHANGE))
        {
            int ballOwner = (int)props[GameSceneManager.BALL_OWNER_CHANGE];

            if (m_PlayerID == ballOwner)
            {
                HasBall = true;
                PhotonNetwork.Destroy(GameObject.FindWithTag("Ball"));
            }
            else
                HasBall = false;
        }
    }
}
