using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ArrowMinigameManager : MonoBehaviour
{
    //게임 내에서 보여줄 목숨 수
    public GameObject life1, life2, life3;
    public static int life;

    //플레이어가 눌러야 할 화살표
    private List<GameObject> _awList = new List<GameObject>();
    public List<GameObject> awList
    {
        get { return _awList; }
        set { _awList = value; }
    }

    //어떤 패턴을 쓸 것인지 랜덤하게 결정.
    //패턴 자체는 이미 정해둔 패턴을 사용한다.
    public static int patternNum = 0;
    public static int[] pattern_1 = new int[9] { 3, 4, 3, 4, 4, 4, 1, 2, 3 };
    public static int[] pattern_2 = new int[9] { 1, 1, 2, 1, 3, 1, 1, 3, 2 };
    public static int[] pattern_3 = new int[9] { 3, 4, 3, 4, 1, 1, 2, 2, 4 };
    public static int[] pattern_4 = new int[9] { 3, 3, 4, 1, 2, 1, 3, 1, 2 };

    [Header("LEVEL")]
    public GameObject leftTurn;
    public int round = 10;
    public GameObject levelMenu;
    public int levelC;

    //20초 안에 게임을 진행해야 한다.
    [Header("TIME")]
    public GameObject timerText;
    public float time = 20.0f;
    public float timeSave;
    public bool isPause;

    public GameObject backBtr;
    public bool gameOver;

    //유니티 내에서 게임 오브젝트를 적용하는 부분.
    public GameObject arrowPrefab;

    [Header("Set Sprite")]
    public Sprite up;
    public Sprite down;
    public Sprite left;
    public Sprite right;

    //한 사이클이 맞는지 틀린지 확인하기 위해 만든 것.
    public ArrowCheck ac;

    //게임을 클리어하면 true.
    public static bool getSnack;

    //시작할 때 '난이도'를 결정해야 하기 때문에 Pause 상태로 시작.
    private void Awake()
    {
        isPause = true;
        Time.timeScale = 0f;
        levelMenu.SetActive(true);
    }

    void Start()
    {
        ac = this.GetComponent<ArrowCheck>();
        gameOver = false;

        life = 3;
        life1.gameObject.SetActive(true);
        life2.gameObject.SetActive(true);
        life3.gameObject.SetActive(true);

        this.timerText = GameObject.Find("Time");

        patternNum = Random.Range(1, 5);
        PatternMake(patternNum);
    }

    void Update()
    {
        if (isPause == true)
            return;

        if (life > 3)
            life = 3;

        //미니게임 목숨이 깎이는 경우.
        //좀 더 깔끔하게 만들 수 있었을 것으로 보인다.
        switch (life)
        {
            case 3:
                life1.gameObject.SetActive(true);
                life2.gameObject.SetActive(true);
                life3.gameObject.SetActive(true);
                this.time -= Time.deltaTime;
                this.timerText.GetComponent<Text>().text = this.time.ToString("N0");
                break;
            case 2:
                life1.gameObject.SetActive(true);
                life2.gameObject.SetActive(true);
                life3.gameObject.SetActive(false);
                this.time -= Time.deltaTime;
                this.timerText.GetComponent<Text>().text = this.time.ToString("N0");
                break;
            case 1:
                life1.gameObject.SetActive(true);
                life2.gameObject.SetActive(false);
                life3.gameObject.SetActive(false);
                this.time -= Time.deltaTime;
                this.timerText.GetComponent<Text>().text = this.time.ToString("N0");
                break;
            case 0:
                life1.gameObject.SetActive(false);
                life2.gameObject.SetActive(false);
                life3.gameObject.SetActive(false);
                backBtr.gameObject.SetActive(true);
                SetResult();
                //'미니게임'을 일단 해야 '다람쥐 게임'이 해금된다.
                if (Event.S.randomValue > 50)
                    Event.squirrelBox = true;
                gameOver = true;
                this.timeSave = this.time;
                timerText.GetComponent<Text>().text = this.timeSave.ToString("N0");
                Time.timeScale = 0f;
                break;
        }

        //시간 제한이 다 된 경우.
        if (time < 0f)
        {
            //'미니게임'을 일단 해야 '다람쥐 게임'이 해금된다.
            if (Event.S.randomValue > 50)
                Event.squirrelBox = true;
            backBtr.gameObject.SetActive(true);
            SetResult();
            gameOver = true;
            timerText.GetComponent<Text>().text = "0";
            Time.timeScale = 0f;
        }

        //한 사이클을 모두 맞춘 경우.
        //랜덤한 수 하나를 받아서, 패턴을 만드는 함수로 전달한다.
        if (ac.allCheck == true)
        {
            round--;
            leftTurn.GetComponent<Text>().text = "<b><size=40>LEFT</size> " + round + "</b>";
            patternNum = Random.Range(1, 5);
            PatternMake(patternNum);
            ac.allCheck = false;
        }

        //모든 사이클을 클리어.
        if (round <= 0)
        {
            if(Event.S.randomValue > 50)
                Event.squirrelBox = true;
            getSnack = true;
            if(levelC == 2)
            {
                FeedMenuManager.S.SnackAmount[2]++;
            }
            backBtr.gameObject.SetActive(true);
            SetResult();
            Time.timeScale = 0f;
        }
    }

    //Sprite를 변경하며 z축을 통해 상 하 좌 우를 구분한다.
    private void SetArrow(GameObject aw, int num)
    {
        if (num == 1)
        {
            aw.GetComponent<SpriteRenderer>().sprite = up;
            aw.transform.position = new Vector3(aw.transform.position.x, aw.transform.position.y, 1);
        }
        else if (num == 2)
        {
            aw.GetComponent<SpriteRenderer>().sprite = down;
            aw.transform.position = new Vector3(aw.transform.position.x, aw.transform.position.y, 2);
        }
        else if (num == 3)
        {
            aw.GetComponent<SpriteRenderer>().sprite = left;
            aw.transform.position = new Vector3(aw.transform.position.x, aw.transform.position.y, 3);
        }
        else
        {
            aw.GetComponent<SpriteRenderer>().sprite = right;
            aw.transform.position = new Vector3(aw.transform.position.x, aw.transform.position.y, 4);
        }
    }

    private void PatternMake(int pNum)
    {
        float xPos = -4.4f;
        List<GameObject> arrowL = new List<GameObject>();

        //랜덤한 인자를 받아서 생성.
        //이미 정해져 있는 패턴을 그대로 생성한다. (위에 선언되어 있는 'pattern_1', pattern_2'...)
        //좀 더 깔끔하게 만들 수 있었을 것으로 보인다.
        //xPos += 1.1f;  해당 내용은 1.1f만큼 오른쪽으로 이동해서 계속 생성한단 의미.
        if (pNum == 1)
        {
            for (int i = 0; i < 9; i++)
            {
                arrowL.Add(Instantiate(arrowPrefab));
                SetArrow(arrowL[i], pattern_1[i]);
                arrowL[i].transform.position = new Vector3(xPos, arrowL[i].transform.position.y,
                                                            arrowL[i].transform.position.z);
                xPos += 1.1f;
            }
        }
        else if (pNum == 2)
        {
            for (int i = 0; i < 9; i++)
            {
                arrowL.Add(Instantiate(arrowPrefab));
                SetArrow(arrowL[i], pattern_2[i]);
                arrowL[i].transform.position = new Vector3(xPos, arrowL[i].transform.position.y,
                                                            arrowL[i].transform.position.z);
                xPos += 1.1f;
            }
        }
        else if (pNum == 3)
        {
            for (int i = 0; i < 9; i++)
            {
                arrowL.Add(Instantiate(arrowPrefab));
                SetArrow(arrowL[i], pattern_3[i]);
                arrowL[i].transform.position = new Vector3(xPos, arrowL[i].transform.position.y,
                                                            arrowL[i].transform.position.z);
                xPos += 1.1f;
            }
        }
        else
        {
            for (int i = 0; i < 9; i++)
            {
                arrowL.Add(Instantiate(arrowPrefab));
                SetArrow(arrowL[i], pattern_4[i]);
                arrowL[i].transform.position = new Vector3(xPos, arrowL[i].transform.position.y,
                                                            arrowL[i].transform.position.z);
                xPos += 1.1f;
            }
        }

        awList = arrowL;
    }

    public void LoadScene()
    {
        SceneManager.LoadScene("RoomScene");
    }

    public void LevelEasy()
    {
        levelMenu.SetActive(false);
        Time.timeScale = 1f; //�߰�

        levelC = 1;
        round = 5;
        isPause = false;
        leftTurn.GetComponent<Text>().text = "<b><size=40>LEFT</size> " + round + "</b>";
        Debug.Log("Easy��ưŬ��");
    }

    public void LevelHard()
    {
        levelMenu.SetActive(false);
        Time.timeScale = 1f; //�߰�

        levelC = 2;
        round = 10;
        isPause = false;
        leftTurn.GetComponent<Text>().text = "<b><size=40>LEFT</size> " + round + "</b>";
        Debug.Log("Hard��ưŬ��");
    }

    void SetResult()
    {
        isPause = true;
        if (getSnack == true)
        {
            backBtr.transform.GetChild(3).GetComponent<Text>().text = "����!";
            backBtr.transform.GetChild(1).GetComponent<Text>().text = "x" + levelC;
        }
        else if (getSnack == false)
        {
            backBtr.transform.GetChild(3).GetComponent<Text>().text = "����!";
            backBtr.transform.GetChild(0).gameObject.SetActive(false);
            backBtr.transform.GetChild(1).gameObject.SetActive(false);
            backBtr.transform.GetChild(2).transform.position = new Vector3(0f, -0.1f, 1f);
        }
    }
}
