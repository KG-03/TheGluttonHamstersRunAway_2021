using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static public GameManager S;

    //게임의 가장 중요한 시스템인 '포만감'을 관리하기 위한 변수
    [SerializeField]
    public Slider fullBar;
    public Text Full;

    //게임에서 '돌발 이벤트'를 관리할 때 썼던 변수.
    public List<GameObject> hartList;
    private static int _hart = 2;
    public int hart
    {
        get { return _hart; }
        set { _hart = value; }
    }

    public GameObject eventOb;
    public Event evt;

    //게임의 '포만감'을 관리하기 위한 변수
    private float _maxFull = 1000f;
    private static float _curFull = 100f;

    public float maxFull
    {
        get { return _maxFull; }
    }

    public float curFull
    {
        get { return _curFull; }
        set { _curFull = value; }
    }

    //게임 시작할 때 '인트로'를 보여주기 위한 변수.
    private static bool _intro = false;
    public bool intro
    {
        get { return _intro; }
        set { _intro = value; }
    }

    //게임의 '포만감'을 채우기 위한 창을 불러오는 부분.
    public FeedMenuManager feedMenu;

    //'포만감'에 따라 표정 아이콘 변화하는 부분
    [Header("Set in Inspector : Face")]
    public Image face;
    public Sprite face_90;
    public Sprite face_60;
    public Sprite face_30;

    //게임의 플레이어
    public GameObject Player;
    private static Vector2 curPlayerPos;
    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("<color=red>GameManager.Awake()</color> - attempted to assign second GameManager.S!");
        }
    }

    //씬 이름이 'RommScene'라면 일어나는 일들.
    //왜 나눠서 관리하고 있었는지는 불명.
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
            evt = eventOb.GetComponent<Event>();
            feedMenu = this.GetComponentInChildren<FeedMenuManager>();

        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            Player.transform.position = curPlayerPos;
            fullBar.value = (float)_curFull / (float)_maxFull;
        }
    }

    //주기적으로 확인하는 것들.
    //'포만감'의 상태가 어떤지, '돌발 이벤트에 몇 번 실패'했는지, '지금 열어둔 창이 어디인지'에 따라서 '씬 이동'을 시키는 모습.
    void Update()
    {
        if (curFull <= 0 && SceneManager.GetActiveScene().name == "RoomScene")
        {
            SceneManager.LoadScene("GameoverScene");
        }
        else if (_curFull >= _maxFull && SceneManager.GetActiveScene().name == "RoomScene")
        {
            SceneManager.LoadScene("GameClear_Stay");
        }
        else if (hart < 0 && SceneManager.GetActiveScene().name == "RoomScene")
        {
            SceneManager.LoadScene("Gameover_Gachul");
        }

        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            curPlayerPos = Player.transform.position;

            Full.text = curFull.ToString("F0") + " / 1000";

            HandleHp();
            FaceChange();
            HartOn();

            //squirrelGame : 경쟁자이자 조력자인 캐릭터 조우 후에 돌발 이벤트가 일어나도록 설정.
            if (Event.squirrelGame == true)
            {
                HartCheck();
            }
        }
    }

    //현재 포만감 가시화
    private void HandleHp()
    {
        fullBar.value = (float)_curFull / (float)_maxFull;
    }

    //현재 포만감에 의한 UI 변화
    private void FaceChange()
    {
        //표정 아이콘 변화 함수
        if (fullBar.value > 0.6)
        {
            face.sprite = face_90;
        }
        else if (fullBar.value <= 0.6 && fullBar.value > 0.3)
        {
            face.sprite = face_60;
        }
        else if (fullBar.value <= 0.3 && fullBar.value > 0)
        {
            face.sprite = face_30;
        }
    }

    bool hartOn = false;

    //한 번만 실행되는 함수.
    //이렇게 설계된 이유는 본 함수가 게임 도중에 추가되는 UI였기 때문인 것으로 기억.
    private void HartOn()
    {
        if(Event.squirrelGame == true && hartOn == false)
        {
            hartList[0].gameObject.SetActive(true);
            hartList[1].gameObject.SetActive(true);
            hartList[2].gameObject.SetActive(true);

            hartOn = true;
        }
    }
    
    //게임 매니저에서 관리하는 하트 수와 Event.cs에서 관리하는 하트 수가 따로 존재. 이를 동시에 확인하는 방법.
    //evt.eventState만 참조해서 위의 3가지 if문이 실행되면 hart가 순식간에 많이 빠져나가는 문제가 발생했던 것으로 기억.
    //따라서 게임 매니저 내에서도 하트 수를 관리하며 '하나만 줄어들도록' 설계한 것으로 보인다.
    private void HartCheck()
    {
        if (evt.eventState == 2 && hart == 2)
        {
            hartList[hart].transform.GetChild(0).gameObject.SetActive(false);
            hart--;
        }

        if (evt.eventState == 1 && hart == 1)
        {
            hartList[hart].transform.GetChild(0).gameObject.SetActive(false);
            hart--;
        }

        if (evt.eventState == 0 && hart == 0)
        {
            hartList[hart].transform.GetChild(0).gameObject.SetActive(false);
            hart--;
        }

        if (evt.eventState == 2)
        {
            hartList[2].transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (evt.eventState == 1)
        {
            hartList[2].transform.GetChild(0).gameObject.SetActive(false);
            hartList[1].transform.GetChild(0).gameObject.SetActive(false);
        }
        else if (evt.eventState == 0)
        {
            hartList[2].transform.GetChild(0).gameObject.SetActive(false);
            hartList[1].transform.GetChild(0).gameObject.SetActive(false);
            hartList[0].transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}

