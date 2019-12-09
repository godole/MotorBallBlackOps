using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    static UIController Instance;

    public static UIController getInstance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(UIController)) as UIController;
            }
            return Instance;
        }
    }

    public PlayPanel PlayPanel { get => m_PlayPanel; set => m_PlayPanel = value; }
    public PitstopPanel PitstopPanel { get => m_PitstopPanel; set => m_PitstopPanel = value; }
    
    [SerializeField] PlayPanel m_PlayPanel;
    [SerializeField] PitstopPanel m_PitstopPanel;
    [SerializeField] GameObject m_SelectPitoutPanel;
    [SerializeField] RectTransform m_MinimapBackground;
    [SerializeField] RectTransform m_MinimapTrack;
    [SerializeField] Vector2 m_SelectPitoutSize;
    [SerializeField] Camera m_MinimapCamera;

    Vector2 m_MinimapStartPos;
    Vector2 m_MinimapStartSize;

    // Start is called before the first frame update
    void Start()
    {
        m_MinimapStartPos = m_MinimapBackground.localPosition;
        m_MinimapStartSize = m_MinimapBackground.rect.size;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectPitout()
    {
        m_SelectPitoutPanel.SetActive(true);
        m_MinimapBackground.localPosition = Vector3.zero;
        m_MinimapTrack.localPosition = Vector3.zero;
        
        m_MinimapBackground.sizeDelta = m_SelectPitoutSize;
        m_MinimapTrack.sizeDelta = m_SelectPitoutSize;
    }

    public void Pitout()
    {
        m_MinimapBackground.localPosition = m_MinimapStartPos;
        m_MinimapTrack.localPosition = m_MinimapStartPos;

        m_MinimapBackground.sizeDelta = m_MinimapStartSize;
        m_MinimapTrack.sizeDelta = m_MinimapStartSize;
        PlayPanel.gameObject.SetActive(true);
        m_SelectPitoutPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }
}
