using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterBase : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject m_Bullet;

    Rigidbody m_RigidBody;

    GameSceneManager m_GameManager;

    public float m_AttackRange;
    public float m_AttackPower;
    public float m_TakeOffRange;

    bool m_IsFront = true;
    bool m_IsAccel = false;
    bool m_HasBall = false;
    bool m_IsHitBullet = false;
    [SerializeField]
    int m_PlayerID = -1;

    [SerializeField]
    float m_ThrowPower;

    public PhotonView m_PhotonView;

    public GameObject m_Cam;

    public GameObject m_CharacterMesh;

    public GameObject m_DestroyEff;

    public GameObject m_Ball;
    public bool HasBall { get => m_HasBall; set => m_HasBall = value; }
    public int PlayerID { get => m_PlayerID; set => m_PlayerID = value; }
    public bool IsHitBullet { get => m_IsHitBullet; set => m_IsHitBullet = value; }
    public int CurrentHP { get => m_CurrentHP; set => m_CurrentHP = value; }

    public int m_OldBallOwner;

    Vector3 networkPosition = Vector3.zero;
    Quaternion networkRotation = Quaternion.identity;

    public int m_MaxHP;
    int m_CurrentHP;

    public int m_TeamNumber = -1;

    [SerializeField]
    Animator m_Animator;

    public GameObject m_RedTeamSign;
    public GameObject m_BlueTeamSign;

    public int m_MaxBulletCapacity;
    public int m_CurBulletCapacity;
    public float m_ShotDelay;
    bool m_IsShotEnable = true;

    public int m_MeleeAttackDamage;
    public float m_MeleeAttackDelay;
    bool m_IsMeleeAttackEnable = true;

    // Start is called before the first frame update
    void OnEnable()
    {
        base.OnEnable();
        m_CurBulletCapacity = m_MaxBulletCapacity;
        m_RigidBody = GetComponent<Rigidbody>();
        m_PhotonView = GetComponent<PhotonView>();

        m_GameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();

        CurrentHP = m_MaxHP;

        if (m_PhotonView.IsMine)
        {
            m_PlayerID = m_GameManager.m_LocalID;
            m_TeamNumber = m_PlayerID % 2 == 1 ? GameSceneManager.RED_TEAM : GameSceneManager.BLUE_TEAM;
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
    }

    // Update is called once per frame
    void Update()
    {
        m_Ball.SetActive(m_HasBall);
        if (Input.GetKeyDown(KeyCode.LeftShift) && photonView.IsMine)
        {
            m_IsFront = !m_IsFront;
            m_CharacterMesh.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));
            m_Cam.GetComponent<CustomFreeLookCam>().IsFront = m_IsFront;
        }
        
        m_GameManager.m_HPUI[m_PlayerID - 1].value = CurrentHP / (float)m_MaxHP;

        if (m_TeamNumber == GameSceneManager.RED_TEAM)
            m_RedTeamSign.SetActive(true);
        else
            m_BlueTeamSign.SetActive(true);

        if(photonView.IsMine)
        {
            m_GameManager.m_MaxBulletCapacity.text = m_MaxBulletCapacity.ToString();
            m_GameManager.m_CurBulletCapacity.text = m_CurBulletCapacity.ToString();
        }

        if(m_CurrentHP <= 0)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        //Instantiate(m_DestroyEff, transform.position, Quaternion.identity);

        if (!photonView.IsMine)
            return;

        if (HasBall)
            PhotonNetwork.Instantiate("Ball", transform.position, new Quaternion(), 0);

        m_Cam.GetComponent<CustomFreeLookCam>().SetTarget(null);

        m_GameManager.RevivePlayer();
    }

    public void EnterPitstop(Vector3 endPoint, Quaternion rotation)
    {
        if (!photonView.IsMine)
            return;

        m_CurBulletCapacity = m_MaxBulletCapacity;
        m_CurrentHP = m_MaxHP;
        transform.position = endPoint;
        transform.rotation = rotation;
        gameObject.GetComponent<TestMovement>().ResetVelocity();
        m_RigidBody.velocity = Vector3.zero;
    }

    public virtual void LeftAttack()
    {
        if(m_IsShotEnable && m_CurBulletCapacity > 0)
        {
            m_CurBulletCapacity--;
            m_IsShotEnable = false;
            m_PhotonView.RPC("RealAttack", RpcTarget.AllViaServer, m_Cam.transform.forward);
            StartCoroutine(ShotDelay());
        }
    }

    IEnumerator ShotDelay()
    {
        yield return new WaitForSeconds(m_ShotDelay);
        m_IsShotEnable = true;
    }

    public virtual void RightAttack()
    {
        if(m_IsMeleeAttackEnable)
        {
            m_IsMeleeAttackEnable = false;
            m_PhotonView.RPC("_RightAttack", RpcTarget.AllViaServer);
            StartCoroutine(AttackDelay());
        }
    }

    IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(m_MeleeAttackDelay);
        m_IsMeleeAttackEnable = true;
    }

    public void HitBullet(int dmg)
    {
        m_PhotonView.RPC("_HitBullet", RpcTarget.AllViaServer, dmg);
    }

    [PunRPC]
    void _HitBullet(int dmg)
    {
        CurrentHP -= dmg;
        m_Animator.Play("Hit", m_Animator.GetLayerIndex("Hit"));
        StartCoroutine(DecreaseByBullet());
    }

    public virtual void ThrowBall()
    {
        m_PhotonView.RPC("_ThrowBall", RpcTarget.AllViaServer, m_Cam.transform.forward);
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

                Hashtable props = new Hashtable
                {
                    {GameSceneManager.BALL_OWNER_CHANGE, m_PlayerID}
                };
                Hashtable oldProps = new Hashtable
                {
                    {GameSceneManager.BALL_OWNER_CHANGE, PhotonNetwork.CurrentRoom.CustomProperties[GameSceneManager.BALL_OWNER_CHANGE] }
                };
                PhotonNetwork.CurrentRoom.SetCustomProperties(props, oldProps);
            }
        }
    }

    public void LossBall()
    {
        m_PhotonView.RPC("_LossBall", RpcTarget.AllViaServer);
    }

    [PunRPC]
    void _LossBall()
    {
        HasBall = false;
    }

    [PunRPC]
    void _RightAttack()
    {
        m_Animator.Play("Melee Attack", m_Animator.GetLayerIndex("Melee Attack"));

        var hit = Physics.OverlapSphere(transform.position, m_AttackRange, 1 << 11);

        foreach (var h in hit)
        {
            var character = h.GetComponent<CharacterBase>();
            if (character.m_TeamNumber != m_TeamNumber)
            {
                Vector3 enemyPos = h.gameObject.transform.position;
                Vector3 deltaPos = enemyPos - transform.position;
                deltaPos.Normalize();
                deltaPos *= m_AttackPower;
                character.Hit(deltaPos, m_MeleeAttackDamage);
            }
        }
    }

    void Hit(Vector3 force, int dmg)
    {
        m_PhotonView.RPC("_Hit", RpcTarget.AllViaServer, force, dmg);
    }

    [PunRPC]
    void _Hit(Vector3 force, int dmg)
    {
        m_Animator.Play("Hit", m_Animator.GetLayerIndex("Hit"));
        m_CurrentHP -= m_MeleeAttackDamage;
        if (m_PhotonView.IsMine)
            m_RigidBody.AddForce(force, ForceMode.Impulse);
    }

    [PunRPC]
    void RealAttack(Vector3 dir)
    {
        var b = Instantiate(m_Bullet, transform.position + dir * 2.0f, transform.rotation);
        var bullet = b.GetComponent<Bullet>();
        bullet.MoveVector = dir;
        bullet.m_Team = m_TeamNumber;
        m_Animator.Play("Shooting", m_Animator.GetLayerIndex("Shooting"));
    }

    [PunRPC]
    void _ThrowBall(Vector3 dir)
    {
        if (!HasBall)
            return;

        m_HasBall = false;

        if (!photonView.IsMine)
            return;

        Hashtable props = new Hashtable
        {
            {GameSceneManager.BALL_OWNER_CHANGE, -2}
        };
        Hashtable oldProps = new Hashtable
        {
            {GameSceneManager.BALL_OWNER_CHANGE, PhotonNetwork.CurrentRoom.CustomProperties[GameSceneManager.BALL_OWNER_CHANGE] }
        };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props, oldProps);

        var ball = PhotonNetwork.Instantiate("Ball", 
            transform.position + dir * 5.0f, new Quaternion());
        ball.GetComponent<MotorBall>().Shot(transform.position, dir, m_ThrowPower);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(m_PlayerID);
            stream.SendNext(CurrentHP);
            stream.SendNext(m_TeamNumber);
        }
        else
        {
            m_PlayerID = (int)stream.ReceiveNext();
            CurrentHP = (int)stream.ReceiveNext();
            m_TeamNumber = (int)stream.ReceiveNext();
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
