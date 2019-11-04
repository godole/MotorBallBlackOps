using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform m_TargetTransform;

    public Vector3 m_Position;
    public Vector3 m_Rotation;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPosition()
    {
        transform.rotation = Quaternion.Euler(m_Rotation.x, m_Rotation.y, m_Rotation.z);
        transform.position = m_Position + m_TargetTransform.position;
    }
}
