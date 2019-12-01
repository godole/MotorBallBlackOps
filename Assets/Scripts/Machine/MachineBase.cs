using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineBase : MonoBehaviour
{
    Rigidbody m_Rigidbody;
    CharacterBase m_CharacterBase;

    public Animator m_Animator;

    public float m_MovementPower;   //가속도
    public float m_BoostMaxSpeed;   //부스트 최대 속도
    public float m_NormalMaxSpeed;  //부스트X 최대 속도
    public float m_RotationSpeed;   //회전 속도
    public float m_RotationPower;   //회전 가속도
    public float m_BallDecreaseRatio;//공 먹었을때 감소 비율
    public float m_HitDecreaseSpeed; //총 맞았을때 감소 속도
    public float m_ReverseSpeed;    //후진 속도
    public float m_DashPower;
    public float m_DashTime;

    //배터리
    float m_Battery;
    float m_BatteryCapacity;
    float m_BatteryConsume;
    float m_BatteryUseCunsume;

    //엔진 출력
    float m_OverdriveLimit;
    float m_MaximumSpeed;
    float m_AcceleratingForce;

    //기동성
    float m_TotalWeight;
    float m_Handling;
    float m_BrakingForce;

    //장갑
    float m_Endurance;
    float m_DefensePower;

    //화력
    float m_Damage;
    float m_SprayPenetration;
    float m_AttackSpeed;
    float m_ReloadSpeed;
    float m_ChargeTime;
    float m_LockOnTime;

    bool m_IsFront = true;
    bool m_IsBoostEnable = true;

    float m_CurMaxSpeed;

    Vector3 m_tempVelocity;

    // Start is called before the first frame update
    void OnEnable()
    {
        m_CharacterBase = GetComponent<CharacterBase>();
        m_Rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_IsFront = !m_IsFront;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
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
        
        transform.Rotate(new Vector3(0.0f, h * m_RotationSpeed * Time.fixedDeltaTime, 0.0f));

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

    public void Boost(Vector3 dir)
    {
        if (!m_IsBoostEnable)
            return;

        //m_tempVelocity = m_Rigidbody.velocity;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.AddForce(dir * m_DashPower, ForceMode.Impulse);
        StartCoroutine(StopBoost());
    }

    IEnumerator StopBoost()
    {
        m_IsBoostEnable = false;
        yield return new WaitForSeconds(m_DashTime);
        m_IsBoostEnable = true;
        //m_Rigidbody.velocity = m_tempVelocity;
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