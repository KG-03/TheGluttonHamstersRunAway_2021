using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Event : MonoBehaviour
{
    static public Event S;
    private PlayerMovement pm;
    private GameManager gm;

    //'다람쥐' 게임이 실행되는가? '다람쥐' 게임을 진행했는가? 
    static public bool squirrelBox = false;
    static public bool squirrelGame = false;

    //돌발 이벤트에 관여되는 모든 게임 오브젝트들 설정.
    [Header("GameObject")]
    public GameObject player;
    public GameObject gameGM;
    public GameObject box;
    public GameObject o_eventTimerText;
    public GameObject o_eventWin;
    public GameObject o_eventLoss;
    public GameObject squirrel;

    //모습 변화를 담는 것
    [Header("Sprite")]
    public Sprite s_open;
    public Sprite s_close;

    //사운드 변화
    [Header("AudioClip")]
    public AudioClip a_open;
    public AudioClip a_close;
    public AudioClip a_eventStart;
    public AudioClip a_eventEnd;

    //돌발 이벤트 중에 '뒷 배경음악'을 변경
    [Header("AudioSource")]
    public AudioSource amain_musin;
    public AudioSource aeff_musin;

    //돌발 이벤트는 랜덤하게 실행되기 때문에 랜덤값이 필요했다.
    [SerializeField]
    private int _randomValue;
    public int randomValue
    {
        get { return _randomValue; }
        set { _randomValue = value; }
    }

    //돌발 이벤트가 진행된 뒤에는 '쿨타임'을 주어 너무 빠르게 이벤트가 진행되지 않도록 설정.
    public int randomValueMin = 65;
    private static float eventCooltime;
    private static float maxEventTimer = 10;

    //돌발 이벤트에서 실패했을 때 깎이는 값. HP의 개념.
    private static int _eventState = 3;
    public int eventState
    {
        get { return _eventState; }
        set { _eventState = value; }
    }

    //이벤트가 끝났을 때
    public bool eventEnd = false;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("<color=red>Event.Awake()</color> - attempted to assign second Event.S!");
        }
    }

    private void Start()
    {
        //게임이 진행중이라면
        if (!(SceneManager.GetActiveScene().name == "GameClear_Stay") &&
            !(SceneManager.GetActiveScene().name == "GameoverScene") &&
            !(SceneManager.GetActiveScene().name == "Gameover_Gachul") &&
            !(SceneManager.GetActiveScene().name == "GameStartScene"))
        {
            gm = gameGM.GetComponent<GameManager>();
            pm = player.GetComponent<PlayerMovement>();
        }

        eventCooltime = maxEventTimer;

        randomValue = Random.Range(1, 101);
    }
    
    void Update()
    {
        //쿨타임 값이 0 이하로 떨어질 때, 0부터 100까지 랜덤 값을 하나 받음. 이 값에 의해 돌발 이벤트가 진행되냐 진행되지 않냐가 나뉜다.
        eventCooltime -= Time.deltaTime;

        if (eventCooltime <= 0)
        {
            randomValue = Random.Range(1, 101);
            eventCooltime = maxEventTimer;
        }

        //squirrelBox으로 다람쥐와 대결하는 게임을 하기 위한 장치를 마련한다.
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            if (squirrelBox == true)
            {
                box.gameObject.SetActive(true);
            }
            else if (squirrelBox == false)
            {
                box.gameObject.SetActive(false);
            }
        }

        //여기서 randomValueMin 값보다 랜덤 값이 더 크다면 이벤트 진행. randomValueMin 값은 위에 선언되어 있다. (65로 지정되어 있다)
        if (SceneManager.GetActiveScene().name == "RoomScene" &&
            randomValue >= randomValueMin && squirrelGame == true)
        {
            SuddenEvent();
        }
    }

    //돌발 이벤트가 진행되는 시간.
    public float evtRunTime = 5f;
    public float evtRunTextTime = 3f;
    int i = 0;

    //돌발 이벤트가 진행되는 동안.
    void SuddenEvent()
    {
        OpenDoor();
        evtRunTime -= Time.deltaTime;

        o_eventTimerText.gameObject.SetActive(true);
        o_eventTimerText.GetComponent<Text>().text = "이벤트 발생! " + evtRunTime.ToString("N0") + "초";

        if (evtRunTime <= 0)
        {
            evtRunTime = 0;
            SuddenResult();
            o_eventTimerText.gameObject.SetActive(false);
        }
    }

    //돌발 이벤트의 결과.
    void SuddenResult()
    {
        CloseDoor();

        //제한 시간이 끝나고 플레이어가 케이지 안에 들어갔는지 확인.
        //케이지 안에 들어갔을 때 종료를 명시함. if문이 두 번 연달아 진행되지 않도록 설계한 것.
        if (pm.cageCollision == true && eventEnd == false)
        {
            o_eventWin.gameObject.SetActive(true);
            eventEnd = true;
        }
        else if (pm.cageCollision == false && eventEnd == false)
        {
            o_eventLoss.gameObject.SetActive(true);
            eventEnd = true;
            eventState--;
            Invoke("GoCageScene", 3f);
        }

        //종료가 명시된 즉시, 초기화할 것을 초기화.
        if (eventEnd == true)
        {
            eventCooltime = maxEventTimer;
            evtRunTextTime -= Time.deltaTime;
        }

        //evtRunTextTime: '돌발 이벤트의 결과'를 UI에 보여주는 시간을 의미.
        //3초간 보여주고 초기화할 것을 초기화.
        if (evtRunTextTime <= 0)
        {
            evtRunTime = 5f;
            eventEnd = false;

            o_eventWin.gameObject.SetActive(false);
            o_eventLoss.gameObject.SetActive(false);

            randomValue = Random.Range(1, 101);
            evtRunTextTime = 3f;
            i = 0;
        }
    }

    //문 열 때 일어나는 일. 애니메이션, 음악 요소 변경.
    //i값: 0 == '문이 열릴 때', 1 == '문이 닫힐 때'
    void OpenDoor()
    {
        if (i == 0)
        {
            i++;
            squirrel.gameObject.SetActive(true);
            this.GetComponent<SpriteRenderer>().sprite = s_open;
            aeff_musin.GetComponent<AudioSource>().clip = a_open;
            if (!aeff_musin.isPlaying)
                aeff_musin.Play();
            amain_musin.GetComponent<AudioSource>().clip = a_eventStart;
            amain_musin.Play();
        }
    }

    //문 닫을 때 일어나는 일. 애니메이션, 음악 요소 변경.
    void CloseDoor()
    {
        if (i == 1)
        {
            i++;
            squirrel.gameObject.SetActive(false);
            this.GetComponent<SpriteRenderer>().sprite = s_close;
            aeff_musin.GetComponent<AudioSource>().clip = a_close;
            if (!aeff_musin.isPlaying)
                aeff_musin.Play();
            amain_musin.GetComponent<AudioSource>().clip = a_eventEnd;
            amain_musin.Play();
        }
    }

    //돌발 이벤트에서 실패하면 '케이지 안으로' 다시 돌려보낸다.
    void GoCageScene()
    {
        SceneManager.LoadScene("CageScene");
    }
}
