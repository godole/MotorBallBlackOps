using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Photon.Pun;

public class PlayerListEntry : MonoBehaviour
{
    [Header("UI 자료")]
    public Text PlayerNameText;

    //public Image ClientReadyImage;
    //public Image PlayerHostImage;
    //public Sprite BOK;
    //public Sprite AOK;
    //public Button PlayerReadyButton;

    public Button Red;
    public Button Blue;

    public GameObject Redlist;
    public GameObject Bluelist;
    public GameObject Waitlist;

    private int ownerId;
    private bool isPlayerReady;
    public int teamnum;
    //teamnum이 0일때는 로비, 1로 되면 블루 -1 은 레드.

    public void Awake()
    {

    }

    public void Start()
    {
        Red = GameObject.Find("UTRED").GetComponent<Button>();
        Blue = GameObject.Find("UTBLUE").GetComponent<Button>();
        Redlist = GameObject.Find("RedPlayerList");
        Bluelist = GameObject.Find("BluePlayerList");
        Waitlist = GameObject.Find("WaitPlayerList");
        GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

        //로컬 플레이어와 해당 프리팹주인의 이름이 같지않으면= 내것이아니면.
        if (PhotonNetwork.LocalPlayer.ActorNumber != ownerId)
        {
            //PlayerReadyButton.gameObject.SetActive(false);
        }
        else
        {

            Hashtable initialProps = new Hashtable() { {GameSceneManager.PLAYER_READY, isPlayerReady }};
            PhotonNetwork.LocalPlayer.SetCustomProperties(initialProps);
            PhotonNetwork.LocalPlayer.SetScore(0);


            //팀을 선택했을때.
            if(teamnum==0)
            {
                isPlayerReady = false;

                Hashtable props = new Hashtable() { { GameSceneManager.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<Lobby>().LocalPlayerPropertiesUpdated();
                }
            }

            Red.onClick.AddListener(() => {
                teamnum = 1;
                transform.SetParent(Redlist.transform);
                isPlayerReady = true;
                Hashtable props = new Hashtable() { { GameSceneManager.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<Lobby>().LocalPlayerPropertiesUpdated();
                }
                Hashtable team = new Hashtable() { { "Team_Number_Select", teamnum } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(team);
            });
            Blue.onClick.AddListener(() => {
                teamnum = 2;
                transform.SetParent(Bluelist.transform);
                isPlayerReady = true;
                Hashtable props = new Hashtable() { { GameSceneManager.PLAYER_READY, isPlayerReady } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(props);

                if (PhotonNetwork.IsMasterClient)
                {
                    FindObjectOfType<Lobby>().LocalPlayerPropertiesUpdated();
                }
                Hashtable team = new Hashtable() { { "Team_Number_Select", teamnum } };
                PhotonNetwork.LocalPlayer.SetCustomProperties(team);
            });

            
            //----------------------------------------------------------------------------------------------------------------
            //플레이어가 버튼을 눌렀을시.
            //PlayerReadyButton.onClick.AddListener(() =>
            //{
            //    isPlayerReady = !isPlayerReady;
            //    //SetPlayerReady(isPlayerReady);

            //    Hashtable props = new Hashtable() { { GameSceneManager.PLAYER_READY, isPlayerReady } };
            //    PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            //    //방장이면 시작버튼 활성화.
            //    if (PhotonNetwork.IsMasterClient)
            //    {                  
            //        FindObjectOfType<Lobby>().LocalPlayerPropertiesUpdated();
            //    }
            //});
            //----------------------------------------------------------------------------------------------------------------------
        }
    }
    public void Update()
    {
        //foreach (Player p in PhotonNetwork.PlayerList)
        //{
        //    if (p.ActorNumber == ownerId)
        //    {
        //        SetPlayerTeam(p.teamnum);   
        //    }
        //    //}
        //    //마스터면 해당 UI 올려주기(문제는 이제 필요없음)
        //    //if (ownerId == PhotonNetwork.MasterClient.MClinetId())
        //    //{
        //    //    PlayerHostImage.color = new Color(PlayerHostImage.color.r, PlayerHostImage.color.g, PlayerHostImage.color.b, 1.0f);
        //    //}
        //    //else
        //    //{
        //    //    PlayerHostImage.color = new Color(PlayerHostImage.color.r, PlayerHostImage.color.g, PlayerHostImage.color.b, 0.0f);
        //    //}
        //}
    }
    //플레이어닉을 오너 닉으로 바꿈.
    public void Initialize(int playerId, string playerName)
    {
        ownerId = playerId;
        PlayerNameText.text = playerName;
    }

    //플레이어 레디 확인.,
    public void SetPlayerReady(bool playerReady)
    {
        //PlayerReadyButton.GetComponent<Image>().sprite = playerReady ? AOK : BOK;
        //ClientReadyImage.GetComponent<Image>().sprite = playerReady ? AOK : BOK;
    }

    public void SetPlayerTeam(int Team)
    {
        Redlist = GameObject.Find("RedPlayerList");
        Bluelist = GameObject.Find("BluePlayerList");
        Waitlist = GameObject.Find("WaitPlayerList");
        if (Team==0)
        {
            transform.SetParent(Waitlist.transform);
            Debug.Log("노멀");
        }
        else if(Team==1)
        {
            transform.SetParent(Redlist.transform);
            Debug.Log("레드");
        }
        else if(Team==2)
        {
            transform.SetParent(Bluelist.transform);
            Debug.Log("블루");
        }
    }


    //public void MasterImageChange()
    //{
    //    if(PhotonNetwork.IsMasterClient)
    //    {
    //        PlayerHostImage.color = new Color(PlayerHostImage.color.r, PlayerHostImage.color.g, PlayerHostImage.color.b, 1.0f);
    //    }
    //    else
    //    {
    //        PlayerHostImage.color = new Color(PlayerHostImage.color.r, PlayerHostImage.color.g, PlayerHostImage.color.b, 0.0f);
    //    }
    //}
}
