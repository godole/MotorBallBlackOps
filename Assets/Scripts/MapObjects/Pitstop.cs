using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitstop : MonoBehaviour
{
    public Transform m_EndPoint;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        var character = other.gameObject.GetComponent<CharacterBase>();
        if(character != null)
        {
            character.EnterPitstop(m_EndPoint.position, m_EndPoint.rotation);
        }
    }
}
