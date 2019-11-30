﻿using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CharacterBase : MonoBehaviourPunCallbacks, IPunObservable
{
    Rigidbody m_RigidBody;

    [SerializeField]
    float m_TakeOffWidth;
    [SerializeField]
    float m_TakeOffHeight;

    bool m_IsFront = true;
    bool m_HasBall = false;
    bool m_IsHitBullet = false;
    [SerializeField]
    int m_PlayerID = -1;

    [SerializeField]
    float m_ThrowPower = 0;

    public CustomFreeLookCam m_Cam;

    public GameObject m_CharacterMesh;

    public GameObject m_DestroyEff;

    public GameObject m_HitEff;

    public GameObject m_Ball;
    public bool HasBall { get => m_HasBall; set => m_HasBall = value; }
    public int PlayerID { get => m_PlayerID; set => m_PlayerID = value; }
    public bool IsHitBullet { get => m_IsHitBullet; set => m_IsHitBullet = value; }
    public int CurrentHP { get => m_CurrentHP; set => m_CurrentHP = value; }
    public bool IsFront { get => m_IsFront; set => m_IsFront = value; }
    public float TakeOffWidth { get => m_TakeOffWidth; set => m_TakeOffWidth = value; }
    public float TakeOffHeight { get => m_TakeOffHeight; set => m_TakeOffHeight = value; }

    public int m_MaxHP;
    int m_CurrentHP;

    public int m_TeamNumber = -1;

    [SerializeField]
    Animator m_Animator;

    [SerializeField]
    Material m_CharacterRedMaterial;

    [SerializeField]
    Material m_CharacterBlueMaterial;

    public Weapon[] m_Weapons;

    // Start is called before the first frame update
    public override void OnEnable()
    {
        base.OnEnable();
        m_RigidBody = GetComponent<Rigidbody>();

        CurrentHP = m_MaxHP;

        if (photonView.IsMine)
        {
            m_PlayerID = GameSceneManager.getInstance.m_LocalID;
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

        //GameSceneManager.getInstance.m_HPUI[m_PlayerID - 1].value = CurrentHP / (float)m_MaxHP;

        if (transform.position.y < -50.0f)
            m_CurrentHP = 0;

        if (m_CurrentHP <= 0)
        {
            if (HasBall && photonView.IsMine)
                PhotonNetwork.Instantiate("Ball", transform.position, new Quaternion(), 0);

            Instantiate(m_DestroyEff, transform.position, Quaternion.identity);

            PhotonNetwork.Destroy(gameObject);
        }

        if (m_PlayerID % 2 == 1)
        {
            m_TeamNumber = GameSceneManager.RED_TEAM;
            m_CharacterMesh.GetComponentInChildren<SkinnedMeshRenderer>().material = m_CharacterRedMaterial;
        }
        else
        {
            m_TeamNumber = GameSceneManager.BLUE_TEAM;
            m_CharacterMesh.GetComponentInChildren<SkinnedMeshRenderer>().material = m_CharacterBlueMaterial;
        }

        if(photonView.IsMine)
        {
            foreach (var weapon in m_Weapons)
            {
                weapon.SetWeaponUI();
            }
        }
    }

    private void OnDestroy()
    {
        if (!photonView.IsMine)
            return;

        m_Cam.SetTarget(null);

        GameSceneManager.getInstance.RevivePlayer();
    }
    
    public void RPC(string method, RpcTarget target, params object[] parameters)
    {
        photonView.RPC(method, target, parameters);
    }

    public void Reverse()
    {
        m_Cam.UnLock();
        IsFront = !IsFront;
        m_CharacterMesh.transform.Rotate(new Vector3(0.0f, 180.0f, 0.0f));

        m_Cam.gameObject.transform.Rotate(0.0f, 180.0f, 0.0f);
        m_Cam.IsFront = IsFront;
    }

    public void EnterPitstop(Vector3 endPoint, Quaternion rotation)
    {
        if (!photonView.IsMine)
            return;

        m_CurrentHP = m_MaxHP;
        transform.position = endPoint;
        transform.rotation = rotation;

        foreach (var weapon in m_Weapons)
            weapon.Reload();

        gameObject.GetComponent<MachineBase>().ResetVelocity();
    }
    
    public void AttackCheck(int index)
    {
        if(m_Weapons[index].AttackCheck())
        {
            RPC("Attack", RpcTarget.AllViaServer, index, m_Cam.ShotDirection);
            m_Weapons[index].StartDelay();    
        }
    }

    [PunRPC]
    void Attack(int index, Vector3 dir)
    {
        m_Weapons[index].Attack(dir);
    }

    public void TakeOffBall()
    {
        if (!m_HasBall)
        {
            Vector3 dir = m_Cam.ShotDirection;
            Vector3 center = transform.position + dir * m_TakeOffHeight / 2;
            Vector3 size = new Vector3(m_TakeOffWidth, 1.0f, m_TakeOffHeight);
            Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
            var hit = Physics.OverlapBox(
                center,
                size / 2,
                rot, 1 << 11);

            GameObject.Find("DebugDraw").GetComponent<DebugDraw>().DrawBox(center, size, rot);

            foreach (var h in hit)
            {
                var character = h.gameObject.GetComponent<CharacterBase>();

                if (!character.m_HasBall)
                    continue;

                NetworkTool.SetCustomPropertiesSafe(GameSceneManager.BALL_OWNER_CHANGE, m_PlayerID);
            }
        }
    }

    public void LockOn()
    {
        if (!m_Cam.IsLockOn)
        {
            var targets = GameObject.FindGameObjectsWithTag("Player");
            float minDist = float.MaxValue;
            GameObject t = null;

            foreach (var target in targets)
            {
                if (target.GetComponent<CharacterBase>().m_TeamNumber == m_TeamNumber)
                    continue;
                
                Vector3 scrPos = Camera.main.WorldToScreenPoint(target.transform.position);

                if (!Camera.main.pixelRect.Contains(scrPos) || scrPos.z < 0.0f)
                    continue;

                Vector2 dist = (Vector2)scrPos - Camera.main.pixelRect.center;
                float distf = dist.magnitude;

                if (distf < minDist)
                {
                    minDist = distf;
                    t = target;
                }
            }

            if(t != null)
                m_Cam.LockOn(t.transform);
        }
        else
            m_Cam.UnLock();
    }

    [PunRPC]
    void Hit(Vector3 force, int dmg)
    {
        m_Animator.Play("Hit", m_Animator.GetLayerIndex("Hit"));
        Instantiate(m_HitEff, transform.position, Quaternion.identity);
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
            transform.position, new Quaternion());
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
