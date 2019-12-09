using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDraw : MonoBehaviour
{
    [SerializeField]
    GameObject m_DebugDrawBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DrawBox(Vector3 center, Vector3 size, Quaternion rot)
    {
        GameObject test = Instantiate(m_DebugDrawBox, center, rot);
        test.transform.localScale = size;
        Destroy(test, 1.0f);
    }
}
