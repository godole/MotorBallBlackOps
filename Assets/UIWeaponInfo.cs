using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIWeaponInfo : MonoBehaviour
{
    [SerializeField] Image m_BackgroundImage;
    [SerializeField] Sprite[] m_BackgroundSprite;
    [SerializeField] Image m_MeleeOnOfImage;
    [SerializeField] Sprite m_MeleeOnSprite;
    [SerializeField] Sprite m_MeleeOffSprite;
    [SerializeField] Slider m_ChargeSlider;
    [SerializeField] Text m_CapacityText;

    public static string WEAPONTYPE_RANGE = "weapontype_range";
    public static string WEAPONTYPE_MELEE = "weapontype_melee";

    public Image BackgroundImage { get => m_BackgroundImage; set => m_BackgroundImage = value; }
    public Image MeleeOnOfImage { get => m_MeleeOnOfImage; set => m_MeleeOnOfImage = value; }
    public Slider ChargeSlider { get => m_ChargeSlider; set => m_ChargeSlider = value; }
    public Text CapacityText { get => m_CapacityText; set => m_CapacityText = value; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetWeaponType(string type)
    {
        if(type == WEAPONTYPE_RANGE)
        {
            BackgroundImage.sprite = m_BackgroundSprite[0];
            m_MeleeOnOfImage.enabled = false;
            m_CapacityText.enabled = true;
            m_ChargeSlider.gameObject.SetActive(false);
        }
        else if(type == WEAPONTYPE_MELEE)
        {
            BackgroundImage.sprite = m_BackgroundSprite[1];
            m_MeleeOnOfImage.enabled = true;
            m_CapacityText.enabled = false;
            m_ChargeSlider.gameObject.SetActive(false);
        }
    }

    public void MeleeReady()
    {
        m_MeleeOnOfImage.sprite = m_MeleeOnSprite;
    }

    public void MeleeNotReady()
    {
        m_MeleeOnOfImage.sprite = m_MeleeOffSprite;
    }
}
