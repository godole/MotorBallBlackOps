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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
