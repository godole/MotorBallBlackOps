using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndSceneManager : MonoBehaviour
{
    [SerializeField] Text m_WinnerTeamText;

    private void OnEnable()
    {
        object winnerTeamID;

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if(props.ContainsKey(GameSceneManager.WINNER_TEAM))
        {
            winnerTeamID = props[GameSceneManager.WINNER_TEAM];

            if((int)winnerTeamID == GameSceneManager.RED_TEAM)
            {
                m_WinnerTeamText.text = "Red Win!!!!";
            }

            else if ((int)winnerTeamID == GameSceneManager.BLUE_TEAM)
            {
                m_WinnerTeamText.text = "Blue Win!!!!";
            }
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
