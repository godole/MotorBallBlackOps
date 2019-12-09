using UnityEngine;
using System.Collections;

public class ParticleAutoDestroy : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;

    void Start()
    {
    }

    void Update()
    {
        if (ps)
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}