using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour
{
    [SerializeField]
    Slider m_ThrowGageSlider;

    public Slider ThrowGageSlider { get => m_ThrowGageSlider; set => m_ThrowGageSlider = value; }

    // Start is called before the first frame update
    void Start()
    {
        ThrowGageSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
