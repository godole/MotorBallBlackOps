using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour
{
    [SerializeField] Slider m_ThrowGageSlider;
    [SerializeField] UIWeaponInfo[] m_WeaponInfo;
    [SerializeField] Image[] m_DashImage;
    [SerializeField] Image[] m_CrossheadImage;
    [SerializeField] Text[] m_TeamScoreText;
    [SerializeField] Slider m_HealthSlider;
    [SerializeField] Slider m_BatterySlider;

    public Slider ThrowGageSlider { get => m_ThrowGageSlider; set => m_ThrowGageSlider = value; }
    public UIWeaponInfo[] WeaponInfo { get => m_WeaponInfo; set => m_WeaponInfo = value; }

    // Start is called before the first frame update
    void Start()
    {
        ThrowGageSlider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDash(int dashCount)
    {
        for(int i = 0; i < m_DashImage.Length; ++i)
        {
            m_DashImage[i].enabled = i < dashCount;
        }
    }

    public void SetLockOn(bool bIs)
    {
        if(bIs)
        {
            m_CrossheadImage[0].gameObject.SetActive(true);
            m_CrossheadImage[1].gameObject.SetActive(false);
        }
        else
        {
            m_CrossheadImage[0].gameObject.SetActive(false);
            m_CrossheadImage[1].gameObject.SetActive(true);
        }
    }

    public void SetRedTeamScore(int score)
    {
        m_TeamScoreText[0].text = score.ToString();
    }

    public void SetBlueTeamScore(int score)
    {
        m_TeamScoreText[1].text = score.ToString();
    }

    public void SetHealthValue(float value)
    {
        m_HealthSlider.value = value;
    }

    public void SetBatteryValue(float value)
    {
        m_BatterySlider.value = value;
    }
}
