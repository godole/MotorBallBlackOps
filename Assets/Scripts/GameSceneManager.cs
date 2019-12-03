using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using UnityEngine.UI;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    static GameSceneManager Instance;
    // Start is called before the first frame update
    public GameObject m_ProgrammerGround;
    public GameObject m_ArtistGround;
    public GameObject m_Gate;
    public GameObject m_Character;

    public Transform[] m_StartPosition;
    public Transform m_BallStartPosition;
    public CustomFreeLookCam m_Cam;

    public Text[] m_PlayerName;
    public Text m_RedScore;
    public Text m_BlueScore;
    public Text m_CurrentVelocity;
    public Slider[] m_HPUI;

    public Text m_MaxBulletCapacity;
    public Text m_CurBulletCapacity;

    public static string BALL_OWNER_CHANGE = "BallOwnerChange";
    public static string CREATE_POSITION = "CreatePosition";
    public static string CREATE_ROTATION = "CreateRotation";
    public static int RED_TEAM = 1;
    public static int BLUE_TEAM = 2;

    public int m_LocalID;

    public float m_PlayerRespawnTime;

    UserInput m_UserInput;

    Transform m_CreatePosition;

    Slider m_ThrowGageSlider;

    public Transform CreatePosition { get => m_CreatePosition; set => m_CreatePosition = value; }
    public static GameSceneManager getInstance
    {
        get
        {
            if (Instance == null)
            {
                Instance = FindObjectOfType(typeof(GameSceneManager)) as GameSceneManager;
            }
            return Instance;
        }
    }

    public Slider ThrowGageSlider { get => m_ThrowGageSlider; set => m_ThrowGageSlider = value; }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Hashtable props = new Hashtable
            {
                {RED_TEAM.ToString(), 0}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            Hashtable props2 = new Hashtable
            {
                {BLUE_TEAM.ToString(), 0}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props2);
        }
        int id = 1;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.ActorNumber == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                m_LocalID = id;
            }
            id++;
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Ball", m_BallStartPosition.position, new Quaternion(), 0);
        }

        ThrowGageSlider = GameObject.Find("ThrowGage").GetComponent<Slider>();
        ThrowGageSlider.gameObject.SetActive(false);

        SetCreatePosition(m_StartPosition[m_LocalID - 1]);
        CreatePlayer(m_StartPosition[m_LocalID - 1]);
    }

    // Update is called once per frame
    void Update()
    {
        int i = 0;
        foreach (var player in PhotonNetwork.PlayerList)
        {
            m_PlayerName[i].text = player.NickName;
            ++i;
        }

        var props = PhotonNetwork.CurrentRoom.CustomProperties;

        if (props.ContainsKey(RED_TEAM.ToString()))
        {
            int score = (int)props[RED_TEAM.ToString()];
            m_RedScore.text = score.ToString();
        }

        if (props.ContainsKey(BLUE_TEAM.ToString()))
        {
            int score = (int)props[BLUE_TEAM.ToString()];
            m_BlueScore.text = score.ToString();
        }
    }

    void CreatePlayer(Transform pos)
    {
        var character = PhotonNetwork.Instantiate("TestPlayer", pos.position, pos.rotation, 0);

        m_UserInput = gameObject.GetComponent<UserInput>();
        m_UserInput.m_Client = character.GetComponent<CharacterBase>();
        m_Cam.SetTarget(character.transform);
        character.GetComponent<CharacterBase>().m_Cam = m_Cam;
    }

    public void RevivePlayer()
    {
        Hashtable props = PhotonNetwork.CurrentRoom.CustomProperties;
        m_CreatePosition.position = (Vector3)props[CREATE_POSITION];
        m_CreatePosition.rotation = (Quaternion)props[CREATE_ROTATION];
        StartCoroutine(CreatePlayerDelay());
    }

    IEnumerator CreatePlayerDelay()
    {
        yield return new WaitForSeconds(m_PlayerRespawnTime);
        CreatePlayer(m_CreatePosition);
    }

    public void SetCreatePosition(Transform pos)
    {
        CreatePosition = pos;

        Hashtable props = new Hashtable
            {
                {CREATE_POSITION, pos.position}
            };
        props.Add(CREATE_ROTATION, pos.rotation);
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }

    
}
