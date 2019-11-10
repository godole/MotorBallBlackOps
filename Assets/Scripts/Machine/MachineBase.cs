using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineBase : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    CharacterBase m_CharacterBase;
    GameSceneManager m_GameManager;

    public Animator m_Animator;

    public float m_MovementPower;   //가속도
    public float m_BoostMaxSpeed;   //부스트 최대 속도
    public float m_NormalMaxSpeed;  //부스트X 최대 속도
    public float m_RotationSpeed;   //회전 속도
    public float m_RotationPower;   //회전 가속도
    public float m_BallDecreaseRatio;//공 먹었을때 감소 비율
    public float m_HitDecreaseSpeed; //총 맞았을때 감소 속도
    public float m_ReverseSpeed;    //후진 속도

    bool m_IsFront = true;

    float m_CurMaxSpeed;

    // Start is called before the first frame update
    void OnEnable()
    {
        m_CharacterBase = GetComponent<CharacterBase>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
        m_GameManager = GameObject.Find("GameSceneManager").GetComponent<GameSceneManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_IsFront = !m_IsFront;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (m_Rigidbody == null)
            return;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (!m_IsFront)
        {
            h *= -1.0f;
        }

        float _h = (v < -0.1f ? -h : h);

        transform.Rotate(new Vector3(0.0f, _h * m_RotationSpeed * Time.fixedDeltaTime, 0.0f));

        Vector3 tempv = m_Rigidbody.velocity;
        tempv.y = 0.0f;
        float curSpeed = tempv.magnitude;

        m_Animator.SetFloat("Accel", curSpeed);

        if (v > 0.1f)
        {
            SetFinalPower(m_BoostMaxSpeed, v);
            m_CurMaxSpeed = curSpeed;
        }
        else if (v < -0.1f)
        {
            SetFinalPower(m_ReverseSpeed, -1.0f);
            m_CurMaxSpeed = curSpeed;
        }
        else
        {
            if (curSpeed <= m_NormalMaxSpeed)
                SetFinalPower(m_CurMaxSpeed, 1.0f);
            else
                SetFinalPower(m_BoostMaxSpeed, 0.0f);
        }

        SetFinalPower(m_BoostMaxSpeed, v);
        m_Rigidbody.AddForce(transform.right.normalized * m_RotationPower * _h);

        if (m_CharacterBase.HasBall)
            finalSpeed = finalSpeed * m_BallDecreaseRatio;

        if (m_CharacterBase.IsHitBullet)
            finalSpeed = finalSpeed - m_HitDecreaseSpeed;

        UniformVelocity(finalSpeed, finalAccel);
    }

    public void ResetVelocity()
    {
        UniformVelocity(0.0f, 0.0f);
        SetFinalPower(0.0f, 0.0f);
        m_CurMaxSpeed = 0.0f;
    }

    void UniformVelocity(float targetVel, float accel)
    {
        if (m_Rigidbody.velocity.magnitude < targetVel)
            m_Rigidbody.AddForce(transform.forward.normalized * m_MovementPower * accel);
    }

    float finalSpeed;
    float finalAccel;

    void SetFinalPower(float speed, float accel)
    {
        finalSpeed = speed;
        finalAccel = accel;
    }
}