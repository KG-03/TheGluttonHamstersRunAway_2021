using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomSnack : MonoBehaviour
{
    static public RandomSnack S;

    public GameObject eventOb;
    public GameObject snackOn;
    public GameObject snackOff;

    public Event evt;

    //얻을 수 있는 과자의 최대 수.
    //이 값은 미니게임을 실행하면 초기화 된다.
    //해당 수를 static으로 관리하고 현재 snack을 따로 관리해야 했을 것으로 보인다.
    int maxSnack1 = 5;
    int maxSnack2 = 5;

    public static bool getSnack1;
    public static bool getSnack2;

    //각각 '랜덤 간식이 나올 확률'
    public int snackRandom1 = 50;
    public int snackRandom2 = 90;

    //먹이를 얻었다고 보여줄 시간초, 텍스트가 보이는 상황인지 알려주기, 간식을 얻고 있는 상태인지 확인, 현재 텍스트가 출력되고 있는지 아닌지.
    //textState : 간식을 얻었다고 명시를 해야 한다고 알림
    //textOn    : 실제로 UI가 동작하고 있음을 알림.
    float textTime = 3;
    bool textOn = false;
    bool randomSnackmg = true;
    bool textState = false;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("<color=red>RandomSnack.Awake()</color> - attempted to assign second RandomSnack.S!");
        }
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "RoomScene")
        {
            evt = eventOb.GetComponent<Event>();
        }
    }

    private void Update()
    {
        //~한 상황일 때.
        if (textOn == true)
        {
            textTime -= Time.deltaTime;
        }

        //위의 if문에서 결국 textTime이 0 이하가 되면 값 초기화.
        if (textTime < 0)
        {
            snackOn.gameObject.SetActive(false);
            snackOff.gameObject.SetActive(false);
            textOn = false;
            textState = false;
            randomSnackmg = true;
            textTime = 3f;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "NonMinigameOb" && Input.GetKey(KeyCode.Z))
        {
            getRandomSnack();
        }
    }

    void getRandomSnack()
    {
        //좀 더 깔끔하게 만들 수 있었을 것으로 보인다.
        if (maxSnack1 > 0 && Event.S.randomValue >= snackRandom1 && Event.S.randomValue < snackRandom2)
        {
            //이벤트 객체의 '랜덤 값'을 확인. 위에 지정된 값 사이라면 1번 간식 제공. (50~90 사이)
            if (textState == false)
            {
                textState = true;
                snackOn.gameObject.SetActive(true);
                textOn = true;
            }
            if (randomSnackmg == true)
            {
                getSnack1 = true;
                maxSnack1--;
            }
        }
        else if (maxSnack2 > 0 && Event.S.randomValue >= snackRandom2)
        {
            if (textState == false)
            {
                textState = true;
                snackOn.gameObject.SetActive(true);
                textOn = true;
            }
            if (randomSnackmg == true)
            {
                getSnack2 = true;
                maxSnack2--;
            }
        }
        else if (maxSnack1 <= 0 || maxSnack2 <= 0)
        {
            //만약 '현재 얻을 수 있는 간식이 없다'고 한다면
            if (textState == false)
            {
                textState = true;
                snackOff.gameObject.SetActive(true);
                textOn = true;
            }
        }
        else
        {
            if (textState == false)
            {
                textState = true;
                snackOff.gameObject.SetActive(true);
                textOn = true;
            }
        }

        randomSnackmg = false;
    }
}
