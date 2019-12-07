using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandleSlider : MonoBehaviour
{
    public float m_MaxAngle;
    public float m_StartAngle;
    public float m_Width;
    public float m_Height;

    public Vector3 m_Anchor;

    public bool m_IsLeft;
    
    
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetValue(float value)
    {
        float deltaX = (m_IsLeft ? -1.0f : 1.0f) * Mathf.Sin((value * (m_MaxAngle + m_StartAngle) * Mathf.Deg2Rad)) * m_Width + (m_Anchor.x - 0.5f) * m_Width;
        float deltaY = -Mathf.Cos((value * (m_MaxAngle + m_StartAngle) * Mathf.Deg2Rad)) * m_Height + m_Anchor.y * m_Height;

        transform.localPosition = new Vector3(deltaX, deltaY, transform.localPosition.z);
    }
}
