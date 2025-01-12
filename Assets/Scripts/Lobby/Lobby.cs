﻿using ExitGames.Client.Photon;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Lobby : MonoBehaviourPunCallbacks

{
    
    [Header("로그인 패널")]
    public GameObject LoginPanel;
    public InputField PlayerNameInput;

    [Header("로그인 체크 패널")]
    public GameObject CheckPanel;
    public Text CheckPlayerName;

    [Header("로비 패널")]
    public GameObject LobbyPanel;

    [Header("옵션 패널")]
    public GameObject OptionPanel;
    public Slider BGMSlider;
    public Text BGMText;
    public Slider SFXSlider;
    public Text SFXText;
    private float BeforeBGM;
    private float BeforeSFX;


    [Header("방 생성 패널")]
    public GameObject CreateRoomPanel;
    public InputField RoomNameInput;
    public InputField MaxPlayerInput;

    [Header("방 리스트 패널")]
    public GameObject RoomListPanel;
    public GameObject RoomListContent;
    public GameObject RoomListEntryPrefab;

    [Header("방 내부 패널")]
    public GameObject InRoomPanel;
    public GameObject InRoomPlayerList;
    public GameObject RedList;
    public GameObject BlueList;
    public Button StartGameButton;
    public GameObject PlayerListEntryPrefab;
    public GameObject Time;
    public GameObject TimeClocking;
    public int TeamType;
    public bool started;
    public int ReadyCount;
    public int PlusCount;
    public bool ReadyBool;
    public const string PLAYER_STARTED = "PlayerStarted";

    private Dictionary<string, RoomInfo> cachedRoomList;
    //서버상 확인된 방정보.
    private Dictionary<string, GameObject> roomListEntries;
    //플레이어에게 보여줄 방리스트
    private Dictionary<int, GameObject> playerListEntries;
    //방 내부에있는 플레이어리스트


    #region 초기화
    public void Awake()
    {
        //씬 전체 동기화
        PhotonNetwork.AutomaticallySyncScene = true;

        //초기화
        cachedRoomList = new Dictionary<string, RoomInfo>();
        roomListEntries = new Dictionary<string, GameObject>();

        //플레이어 이름 임의 초기화
        PlayerNameInput.text = "플레이어" + Random.Range(1000, 10000);

    }

    public void Update()
    {

        BGMText.text = Mathf.Round(BGMSlider.value).ToString();
        SFXText.text = Mathf.Round(SFXSlider.value).ToString();

        Debug.Log(started);
        if (started==true)
        {
            Time.SetActive(true);
            TimeClocking.GetComponent<TimeClocker>().Clock();
        }

    }
    #endregion

    #region 네트워크

    //오류 설정------------------------------------------------------------------
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        SetActivePanel(LobbyPanel.name);
        Debug.Log("Fail");
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        SetActivePanel(LobbyPanel.name);
        Debug.Log("Fail");
    }
    //---------------------------------------------------------------------------

    public override void OnConnectedToMaster()//서버 입장
    {
        this.SetActivePanel(LobbyPanel.name);
        Debug.Log("입장");
        TeamType = 0;
        //플레이어의 팀 초기화
        //룸으로 들어가서 초기화됨.
        Hashtable team = new Hashtable() { { "Team_Number_Select", TeamType } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(team);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        ClearRoomListView();
        UpdateCachedRoomList(roomList);
        UpdateRoomListView();
    }

    public override void OnLeftLobby()
    {
        cachedRoomList.Clear();
        ClearRoomListView();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room");
        SetActivePanel(InRoomPanel.name);

        if(playerListEntries==null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }

        foreach (Player p in PhotonNetwork.PlayerList)
        {
            GameObject entry = Instantiate(PlayerListEntryPrefab);
            entry.transform.SetParent(InRoomPlayerList.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<PlayerListEntry>().Initialize(p.ActorNumber, p.NickName);

            object isPlayerReady;
            object team;
            if (p.CustomProperties.TryGetValue(GameSceneManager.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }

            //기존사람들에게 팀 선택된거 보여주기.
            if (p.CustomProperties.TryGetValue("Team_Number_Select", out team)) ;
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerTeam((int)team);
                Debug.Log((int)team);
            }
            Debug.Log("온 조인더 룸");
            
            //else if(!p.CustomProperties.TryGetValue(GameSceneManager.PLAYER_READY, out isPlayerReady))
            //{
            //    Debug.Log("방입장레드false");
            //    entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            //}


            playerListEntries.Add(p.ActorNumber, entry);
        }

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
        Hashtable props = new Hashtable
        {
                {GameSceneManager.PLAYER_LOADED_LEVEL, false}
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public override void OnLeftRoom()
    {
        SetActivePanel(LobbyPanel.name);

        foreach(GameObject entry in playerListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        playerListEntries.Clear();
        playerListEntries = null;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)//방입장.
    {
        GameObject entry = Instantiate(PlayerListEntryPrefab);
        entry.transform.SetParent(InRoomPlayerList.transform);
        entry.transform.localScale = Vector3.one;
        entry.GetComponent<PlayerListEntry>().Initialize(newPlayer.ActorNumber, newPlayer.NickName);

        playerListEntries.Add(newPlayer.ActorNumber, entry);

        //레디되면 해당 버튼 활성화
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
        //PlayerListEntryPrefab.GetComponent<PlayerListEntry>().MasterImageChange();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)//방퇴장
    {
        Destroy(playerListEntries[otherPlayer.ActorNumber].gameObject);
        playerListEntries.Remove(otherPlayer.ActorNumber);

        StartGameButton.gameObject.SetActive(CheckPlayersReady());
        //PlayerListEntryPrefab.GetComponent<PlayerListEntry>().MasterImageChange();
    }

    public override void OnMasterClientSwitched(Player newMasterClient)//방장 나갈시.
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
        {
            StartGameButton.gameObject.SetActive(CheckPlayersReady());
        }
        //PlayerListEntryPrefab.GetComponent<PlayerListEntry>().MasterImageChange();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)//플레이어 준비상태 확인.
    {
        Debug.Log("사용됨");
        if (playerListEntries == null)
        {
            playerListEntries = new Dictionary<int, GameObject>();
        }


        GameObject entry;
        if (playerListEntries.TryGetValue(targetPlayer.ActorNumber, out entry))
        {
            object isPlayerReady;
            if (changedProps.TryGetValue(GameSceneManager.PLAYER_READY, out isPlayerReady))
            {
                entry.GetComponent<PlayerListEntry>().SetPlayerReady((bool)isPlayerReady);
            }
            //entry.GetComponent<PlayerListEntry>().SetPlayerTeam(targetPlayer.teamnum);
            Debug.Log("플레이어 준비상태 확인");
            Debug.Log(targetPlayer.NickName);
            object team;
            //플레이어 팀 확인.
            if(changedProps.TryGetValue("Team_Number_Select", out team))
            {
                Debug.Log(targetPlayer.NickName);
                Debug.Log(team);
                entry.GetComponent<PlayerListEntry>().SetPlayerTeam((int)team);
                if((int)team==0)
                {
                    ReadyBool = false;
                }
                else if((int)team!=0)
                {
                    ReadyBool = true;
                }

            }

            //int num = targetPlayer.CustomProperties.TryGetValue(GameSceneManager.PLAYER_TEAM, out num);
            //Debug.Log(entry.GetComponent<PlayerListEntry>().teamnum);
        }
        //--------------------------------------------시험
        //foreach (Player p in PhotonNetwork.PlayerList)
        //{

        //}

        //-------------------------------------------------
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
        
    }
    #endregion
    #region UI동작
    public void OnLoginButtonClicked()//로그인 버튼클릭
    {
        string playerName = PlayerNameInput.text;

        if (!playerName.Equals(""))
        {
            PhotonNetwork.LocalPlayer.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
        else
        {
            Debug.LogError("Player Name is invalid.");

        }
    }
    public void OnYesButtonClicked()//체크패널 활성화
    {
        this.SetActivePanel(CheckPanel.name);
        CheckPlayerName.text = PlayerNameInput.text;
    }

    public void OnNoButtonClicked()//이름 재설정
    {
        this.SetActivePanel(LoginPanel.name);
    }

    public void OnBackButtonClicked()//뒤로가기 버튼 클릭.
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.LeaveLobby();
        }

        SetActivePanel(LobbyPanel.name);
    }

    public void OnCreateRoomButtonClicked()//방생성 버튼 클릭
    {
        string roomName = RoomNameInput.text;
        roomName = (roomName.Equals(string.Empty)) ? "Room " + Random.Range(1000, 10000) : roomName;

        byte maxPlayers;
        byte.TryParse(MaxPlayerInput.text, out maxPlayers);
        maxPlayers = (byte)Mathf.Clamp(maxPlayers, 4, 6);

        RoomOptions options = new RoomOptions { MaxPlayers = maxPlayers };

        PhotonNetwork.CreateRoom(roomName, options, null);
    }

    public void OnCreateRoomNoButtonClicked()//방생성 취소 버튼 클릭
    {
        SetActivePanel(LobbyPanel.name);
    }


    public void OnLeaveGameButtonClicked()//방나가기 버튼 클릭
    {
        //플레이어 팀 기본 초기화
        Hashtable team = new Hashtable() { { "Team_Number_Select", 0 } };
        PhotonNetwork.LocalPlayer.SetCustomProperties(team);
        PhotonNetwork.LeaveRoom();
    }

    public void OnRoomListButtonClicked()//조인게임클릭
    {
        if (!PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinLobby();
        }

        SetActivePanel(RoomListPanel.name);
    }

    public void OnOptionButtonClicked()//옵션 버튼 클릭
    {
        this.SetActivePanel(OptionPanel.name);
        BeforeSFX = SFXSlider.value;
        BeforeBGM = BGMSlider.value;
    }

    public void OnOptionOKButtonClicked()
    {
        SetActivePanel(LobbyPanel.name);
    }
    
    public void OnOptionExitButtonCicked()
    {
        BGMSlider.value=BeforeBGM;
        SFXSlider.value=BeforeSFX;
        SetActivePanel(LobbyPanel.name);
    }

    public void OnNewGameButtonClicked()
    {
        this.SetActivePanel(CreateRoomPanel.name);
    }

    public void OnStartGameButtonClicked()//시작버튼을 누르면
    {
        CStart();
        //started = true;
        //Hashtable starting = new Hashtable
        //{
        //    { Lobby.PLAYER_STARTED,started  }
        //};
        //PhotonNetwork.CurrentRoom.SetCustomProperties(starting);
    }
    #endregion
    #region 내부동작
 
    public void Bestarted(bool ready)
    {
        Time.SetActive(true);
        TimeClocking.GetComponent<TimeClocker>().Clock();
    }

    private bool CheckPlayersReady()//플레이어 레디 확인. 여기서 처리하자.
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return false;
        }
        int count = 0;
        int rcount=0;
        int bcount=0;
        foreach (Player p in PhotonNetwork.PlayerList)
        {
            object isPlayerReady;
            object team;
            if (p.CustomProperties.TryGetValue(GameSceneManager.PLAYER_READY, out isPlayerReady))
            {
                if (!(bool)isPlayerReady)
                {
                    return false;
                }
                //플레이어 팀 확인
                if(p.CustomProperties.TryGetValue("Team_Number_Select",out team))
                {
                    if((int)team==1)
                    {
                        rcount += 1;
                    }
                    else if((int)team==2)
                    {
                        bcount += 1;
                    }
                }
                count += 1;
            }
            else
            {
                return false;
            }
        }
        //방 열기 위한 조건부. 4인과 플레이어수 4또는 6일때. 플레이어 수 맞을때.
        Debug.Log(count);
        Debug.Log(rcount);
        Debug.Log(bcount);
        //if((count==2||count==4||count==6)&&rcount==bcount)
        //{
        //    return true;
        //}
        return true;
    }

    private void ClearRoomListView()//방 초기화
    {
        foreach (GameObject entry in roomListEntries.Values)
        {
            Destroy(entry.gameObject);
        }
        roomListEntries.Clear();
    }

    public void LocalPlayerPropertiesUpdated()
    {
        //시작버튼 부분 활성화.
        StartGameButton.gameObject.SetActive(CheckPlayersReady());
    }


    private void SetActivePanel(string activePanel)//패널 켜주기
    {
        LoginPanel.SetActive(activePanel.Equals(LoginPanel.name));
        CheckPanel.SetActive(activePanel.Equals(CheckPanel.name));
        LobbyPanel.SetActive(activePanel.Equals(LobbyPanel.name));
        OptionPanel.SetActive(activePanel.Equals(OptionPanel.name));
        CreateRoomPanel.SetActive(activePanel.Equals(CreateRoomPanel.name));
        RoomListPanel.SetActive(activePanel.Equals(RoomListPanel.name));
        InRoomPanel.SetActive(activePanel.Equals(InRoomPanel.name));

    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        //해당 RoomInfo에서 저장정보를 가지고있는 List 라는 roomList 만큼의 방 정보를 읽는다.
        {
            // Remove room from cached room list if it got closed, became invisible or was marked as removed
            //닫히거나, 보이지 않게 되거나, 제거된 것으로 표시된 캐시에있는 룸목록에서 룸 제거.
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                //서버에 올라가있는 룸리스트들중 해당 정보의 이름을가진것이 있으면
                {
                    cachedRoomList.Remove(info.Name);
                    //해당 이름을 가진 정보를 삭제한다.
                }

                continue;
            }

            // Update cached room info
            //서버에있는 방 정보 리스트를 새로 불러온다.
            if (cachedRoomList.ContainsKey(info.Name))
            {
                cachedRoomList[info.Name] = info;
                //cachedRoomList에 해당 이름 에 정보를 넣는다.
            }
            // Add new room info to cache
            //새로운 방정보가 서버에 올라간경우
            else
            {
                cachedRoomList.Add(info.Name, info);
                //해당 정보를 추가한다.
            }
        }
    }

    private void UpdateRoomListView()
    {
        foreach (RoomInfo info in cachedRoomList.Values)
        {
            GameObject entry = Instantiate(RoomListEntryPrefab);
            entry.transform.SetParent(RoomListContent.transform);
            entry.transform.localScale = Vector3.one;
            entry.GetComponent<RoomListEntry>().Initialize(info.Name, (byte)info.PlayerCount, info.MaxPlayers);
            roomListEntries.Add(info.Name, entry);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(Lobby.PLAYER_STARTED))
        {
            bool star = (bool)propertiesThatChanged[Lobby.PLAYER_STARTED];
            started = star;
        }
    }

    public void CStart()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        //게임시작 버튼
        PhotonNetwork.LoadLevel("BGscene");
    }
    #endregion

}
