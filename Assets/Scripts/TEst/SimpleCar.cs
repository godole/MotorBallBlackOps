using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Cameras;

public class SimpleCar : MonoBehaviour
{
    public WheelCollider frontLeftCol, frontRightCol;
    public WheelCollider backLeftCol, backRightCol;

    public Transform frontLeft, frontRight;
    public Transform backLeft, backRight;

    public CustomFreeLookCam m_Cam;
    public GameObject m_CharacterMesh;

    public Animator m_Animator;

    public float _steerAngle = 25.0f;
    public float _motorForce = 1500.0f;

    public float m_BoostMaxSpeed;
    public float m_NormalMaxSpeed;
    float m_CurSpeed = 0.0f;
    float m_CurMaxSpeed = 0.0f;

    public float steerAngle;

    bool m_IsFront = true;

    float h, v;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<CharacterBase>().m_Cam = m_Cam.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (!m_IsFront)
        {
            v *= -1.0f;
            h *= -1.0f;
        }
        
        m_Animator.SetFloat("Accel", v);

        Drive();
        SteerCar();

        UpdateWheelPos(frontLeftCol, frontLeft);
        UpdateWheelPos(frontRightCol, frontRight);
        UpdateWheelPos(backLeftCol, backLeft);
        UpdateWheelPos(backRightCol, backRight);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_IsFront = !m_IsFront;
            m_CharacterMesh.transform.rotation = Quaternion.Euler(0.0f, m_IsFront ? 0.0f : 180.0f, 0.0f);
            m_Cam.IsFront = m_IsFront;
        }
    }

    void Drive()
    {
        m_CurSpeed = 2.0f * 3.14f * backRightCol.radius * backRightCol.rpm * 60.0f / 1000.0f;
        m_CurSpeed = Mathf.Round(m_CurSpeed);

        if(v > 0.1f)
        {
            UniformVelocity(m_BoostMaxSpeed, v);
            m_CurMaxSpeed = m_CurSpeed;
        }
        else if(v < -0.1f)
        {
            UniformVelocity(m_BoostMaxSpeed, -1.0f);
            m_CurMaxSpeed = m_CurSpeed;
        }
        else
        {
            if (m_CurSpeed <= m_NormalMaxSpeed)
                UniformVelocity(m_CurMaxSpeed, 1.0f);
            else
                UniformVelocity(m_BoostMaxSpeed, 0.0f);
        }
    }

    void SteerCar()
    {
        steerAngle = _steerAngle * h;
        frontLeftCol.steerAngle = steerAngle;
        frontRightCol.steerAngle = steerAngle;
    }

    void UpdateWheelPos(WheelCollider col, Transform trans)
    {
        Vector3 pos = trans.position;
        Quaternion rot = trans.rotation;

        col.GetWorldPose(out pos, out rot);

        trans.position = pos;
        trans.rotation = rot;
    }

    void UniformVelocity(float targetVelocity, float accel)
    {
        if (m_CurSpeed < targetVelocity)
        {
            backLeftCol.motorTorque = accel * _motorForce;
            backRightCol.motorTorque = accel * _motorForce;
        }

        else
        {
            backLeftCol.motorTorque = 0.0f;
            backRightCol.motorTorque = 0.0f;
        }
    }
}
