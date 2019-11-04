using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemy : MonoBehaviour
{
    public float m_MaxSpeed;
    public float m_HitSpeed;

    public float m_SlowTime;

    float m_CurrentSpeed;

    Rigidbody m_Rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_CurrentSpeed = m_MaxSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        m_Rigidbody.AddForce(new Vector3(m_CurrentSpeed, 0.0f, 0.0f));
    }

    public void Hit()
    {
        StartCoroutine("Slow");
    }

    IEnumerable Slow()
    {
        m_CurrentSpeed = m_HitSpeed;
        yield return new WaitForSeconds(1000);
        m_CurrentSpeed = m_MaxSpeed;
    }
}
