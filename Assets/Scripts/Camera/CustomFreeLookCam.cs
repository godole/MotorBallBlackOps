using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CustomFreeLookCam : UnityStandardAssets.Cameras.PivotBasedCameraRig
{
    // This script is designed to be placed on the root object of a camera rig,
    // comprising 3 gameobjects, each parented to the next:

    // 	Camera Rig
    // 		Pivot
    // 			Camera

    [SerializeField] private float m_MoveSpeed = 1f;                      // How fast the rig will move to keep up with the target's position.
    [Range(0f, 10f)] [SerializeField] private float m_TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.
    [SerializeField] private float m_TurnSmoothing = 0.0f;                // How much smoothing to apply to the turn input, to reduce mouse-turn jerkiness
    [SerializeField] private float m_TiltMax = 75f;                       // The maximum value of the x axis rotation of the pivot.
    [SerializeField] private float m_TiltMin = 45f;                       // The minimum value of the x axis rotation of the pivot.
    [SerializeField] private bool m_LockCursor = false;                   // Whether the cursor should be hidden and locked.
    [SerializeField] private bool m_VerticalAutoReturn = false;           // set wether or not the vertical axis should auto return
    [SerializeField] private float m_TiltMaxY = 75f;                       // The maximum value of the x axis rotation of the pivot.
    [SerializeField] private float m_TiltMinY = 45f;
    [SerializeField] private GameObject m_AimSprite;

    private float m_LookAngle;                    // The rig's y axis rotation.
    private float m_TiltAngle;                    // The pivot's x axis rotation.
    private float m_TiltAngleY;                    // The pivot's x axis rotation.
    private const float k_LookDistance = 100;    // How far in front of the pivot the character's look target is.
    private Vector3 m_PivotEulers;
    private Quaternion m_PivotTargetRot;
    private Quaternion m_TransformTargetRot;
    private bool m_IsFront = true;
    Transform m_LockOnTarget = null;

    public bool IsFront { get => m_IsFront; set => m_IsFront = value; }
    public Vector3 ShotDirection
    {
        get
        {
            Vector3 dir = Vector3.zero;

            if (m_LockOnTarget == null)
                dir = transform.forward;
            else
            {
                dir = m_LockOnTarget.position - transform.position;
                dir.Normalize();
            }

            return dir;
        }
    }
    public bool IsLockOn
    {
        get
        {
            return m_LockOnTarget != null;
        }
    }

    public Transform LockOnTarget { get => m_LockOnTarget; set => m_LockOnTarget = value; }

    public void LockOn(Transform target)
    {
        m_LockOnTarget = target;
        Vector3 dir = target.position - transform.position;
        m_LookAngle = Quaternion.LookRotation(dir, Vector3.up).eulerAngles.y - (m_IsFront ? 0.0f : 180.0f);
    }

    public void UnLock()
    {
        m_LockOnTarget = null;
    }

    protected override void Awake()
    {
        base.Awake();
        // Lock or unlock the cursor.
        Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !m_LockCursor;
        m_PivotEulers = m_Pivot.rotation.eulerAngles;

        m_PivotTargetRot = m_Pivot.transform.localRotation;
        m_TransformTargetRot = transform.localRotation;
    }


    protected void Update()
    {
        HandleRotationMovement();
        if (m_LockCursor && Input.GetMouseButtonUp(0))
        {
            Cursor.lockState = m_LockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_LockCursor;
        }
        if(m_LockOnTarget != null)
        {
            Vector3 targetPos = Camera.main.WorldToScreenPoint(m_LockOnTarget.position);
            m_AimSprite.transform.position = targetPos;

            if (!Camera.main.pixelRect.Contains(targetPos))
                m_LockOnTarget = null;
        }
        else
        {
            m_AimSprite.transform.position = Camera.main.pixelRect.center;
        }
    }


    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    protected override void FollowTarget(float deltaTime)
    {
        if (m_Target == null) return;
        // Move the rig towards target position.
        transform.position = Vector3.Lerp(transform.position, m_Target.position, deltaTime * m_MoveSpeed);
    }


    private void HandleRotationMovement()
    {
        if (Time.timeScale < float.Epsilon)
            return;

        // Read the user input
        var x = CrossPlatformInputManager.GetAxis("Mouse X");
        var y = CrossPlatformInputManager.GetAxis("Mouse Y");

        // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
        
        m_LookAngle += x * m_TurnSpeed;

        //m_LookAngle = Mathf.Clamp(m_LookAngle, targetRotEulerY - m_TiltMinY, targetRotEulerY + m_TiltMaxY);
        if(m_Target != null)
        {
            float targetRotEulerY = m_Target.rotation.eulerAngles.y;

            float deltaangle = Quaternion.FromToRotation((m_IsFront ? m_Target.forward : -m_Target.forward), transform.forward).eulerAngles.y;

            if (deltaangle > 180.0f)
                deltaangle = -(360.0f - deltaangle);

            if (deltaangle < m_TiltMinY)
                m_LookAngle = targetRotEulerY + m_TiltMinY + 0.2f;

            if (deltaangle > m_TiltMaxY)
                m_LookAngle = targetRotEulerY + m_TiltMaxY - 0.2f;
        }

        //m_LookAngle += deltaangle;

        m_TransformTargetRot = Quaternion.Euler(0f, m_LookAngle, 0f);

        if (m_VerticalAutoReturn)
        {
            // For tilt input, we need to behave differently depending on whether we're using mouse or touch input:
            // on mobile, vertical input is directly mapped to tilt value, so it springs back automatically when the look input is released
            // we have to test whether above or below zero because we want to auto-return to zero even if min and max are not symmetrical.
            m_TiltAngle = y > 0 ? Mathf.Lerp(0, -m_TiltMin, y) : Mathf.Lerp(0, m_TiltMax, -y);
        }
        else
        {
            // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
            m_TiltAngle -= y * m_TurnSpeed;
            //// and make sure the new value is within the tilt range
            m_TiltAngle = Mathf.Clamp(m_TiltAngle, -m_TiltMin, m_TiltMax);
        }

        // Tilt input around X is applied to the pivot (the child of this object)
        m_PivotTargetRot = Quaternion.Euler(m_TiltAngle, m_PivotEulers.y, m_PivotEulers.z);

        if (m_TurnSmoothing > 0)
        {
            m_Pivot.localRotation = Quaternion.Slerp(m_Pivot.localRotation, m_PivotTargetRot, m_TurnSmoothing * Time.deltaTime);
            Vector3 targetRotEuler = m_TransformTargetRot.eulerAngles;
            Quaternion tempRot = Quaternion.Euler(targetRotEuler.x, m_IsFront ? targetRotEuler.y : targetRotEuler.y - 180.0f, targetRotEuler.z);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, tempRot, m_TurnSmoothing * Time.deltaTime);
        }
        else
        {
            m_Pivot.localRotation = m_PivotTargetRot;
            transform.localRotation = m_TransformTargetRot;
        }
    }
}
