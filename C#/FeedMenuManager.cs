using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FeedMenuManager : MonoBehaviour
{
    static public FeedMenuManager S;

    //메뉴 열 때 쓰는 버튼, '열렸는지 확인하는 상태', 애니메이션
    public Button feedMenuBtn;
    private bool state = false;
    private Animator uiAni;

    //간식 오브젝트 담는 공간. 
    public GameObject[] snack;
    private static int[] _snackAmount = new int[6];

    //사운드. 먹을 때 사운드, UI를 조작할 때 나는 사운드.
    private AudioSource sound;

    public AudioClip eatSource;
    public AudioClip uiSource;

    //본래 계획은 음식마다 차는 포만감이 달라야 했는데, 회의를 통해 밸런스를 조절해서 모든 음식으로부터 얻을 수 있는 포만감을 동일하게 설정했었다.
    public int[] snackUse = { 90, 90, 90, 90, 90, 90 };

    public int this[int index]
    {
        get { return _snackAmount[index]; }
        set
        {
            //왜 6 이하만 확인했는지 알 수 없다. 마이너스 값을 확인하지 않은 이유가 있었는지 모르겠다.
            if (index <= 6)
            {
                _snackAmount[index] = value;
            }
            else
            {
                //본래 디버그 문구가 있었으나, 텍스트가 깨진 탓에 내용을 임의로 변경.
                Debug.LogError("FeedMenuManager: _snackAmount[id] 호출이 잘못되었습니다.");
            }
        }
    }

    public Text[] snackAmountText = new Text[6];

    public GameManager gm;

    private void Awake()
    {
        if (S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("<color=red>FeedMenuManager.Awake()</color> - attempted to assign second FeedMenuManager.S!");
        }
    }

    //게임 진행 화면에서만 활성화 되도록 한다.
    void Start()
    {
        if (!(SceneManager.GetActiveScene().name == "GameClear_Stay") &&
            !(SceneManager.GetActiveScene().name == "GameoverScene") &&
            !(SceneManager.GetActiveScene().name == "Gameover_Gachul") &&
            !(SceneManager.GetActiveScene().name == "GameStartScene"))
        {
            gm = this.GetComponentInParent<GameManager>();
            uiAni = this.GetComponent<Animator>();
            sound = this.gameObject.GetComponent<AudioSource>();
        }
    }
    
    void Update()
    {
        //탭 키를 누르면 UI창이 나오도록 하는 설정.
        //유니티 내부의 애니메이션을 많이 사용했다.
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            state = !state;
            if (state == true)
            {
                sound.clip = uiSource;
                sound.Play();
                uiAni.SetTrigger("Show");
                
            }
            else if (state == false)
            {
                uiAni.SetTrigger("Hide");
            }
        }

        //게임 진행 상황이라면 활성화.
        if (!(SceneManager.GetActiveScene().name == "GameClear_Stay") &&
            !(SceneManager.GetActiveScene().name == "GameoverScene") &&
            !(SceneManager.GetActiveScene().name == "Gameover_Gachul") &&
            !(SceneManager.GetActiveScene().name == "GameStartScene"))
        {
            SnackSetActive();
            UseSnake();
            AddSnack();
        }
    }

    //간식이 한 개라도 생기면 UI 활성화.
    private void SnackSetActive()
    {
        if (_snackAmount[0] < 1)
        {
            snack[0].SetActive(false);
        }
        else
        {
            snack[0].SetActive(true);
            snackAmountText[0].text = $"수량: {_snackAmount[0]}개";
        }

        if (_snackAmount[1] < 1)
        {
            snack[1].SetActive(false);
        }
        else
        {
            snack[1].SetActive(true);
            snackAmountText[1].text = $"수량: {_snackAmount[1]}개";
        }

        if (_snackAmount[2] < 1)
        {
            snack[2].SetActive(false);
        }
        else
        {
            snack[2].SetActive(true);
            snackAmountText[2].text = $"수량: {_snackAmount[2]}개";
        }

        if (_snackAmount[3] < 1)
        {
            snack[3].SetActive(false);
        }
        else
        {
            snack[3].SetActive(true);
            snackAmountText[3].text = $"수량: {_snackAmount[3]}개";
        }

        if (_snackAmount[4] < 1)
        {
            snack[4].SetActive(false);
        }
        else
        {
            snack[4].SetActive(true);
            snackAmountText[4].text = $"수량: {_snackAmount[4]}개";
        }

        if (_snackAmount[5] < 1)
        {
            snack[5].SetActive(false);
        }
        else
        {
            snack[5].SetActive(true);
            snackAmountText[5].text = $"수량: {_snackAmount[5]}개";
        }
    }

    //간식을 먹었을 때.
    //UI창이 열려 있을 때만 사용할 수 있도록 설정해두었다.
    //코드를 더 깔끔하게 만들 수 있었을 것으로 보인다.
    public void UseSnake()
    {
        if (state == true)
        {
            //현재 포만감이 최대치가 아니고, 간식이 1개 이상 있으며, 특정 키를 눌렀을 때.
            if (gm.curFull < 1000 && _snackAmount[0] > 0 && Input.GetKeyDown(KeyCode.Alpha1))
            {
                sound.clip = eatSource;
                sound.Play();
                if (gm.curFull + snackUse[0] < gm.maxFull)
                {
                    _snackAmount[0]--;
                    gm.curFull += snackUse[0];
                }
                else
                {
                    _snackAmount[0]--;
                    gm.curFull = gm.maxFull;
                }
            }
            else if (gm.curFull < 1000 && _snackAmount[1] > 0 && Input.GetKeyDown(KeyCode.Alpha2))
            {
                sound.clip = eatSource;
                sound.Play();
                if (gm.curFull + snackUse[1] < gm.maxFull)
                {
                    _snackAmount[1]--;
                    gm.curFull += snackUse[1];
                }
                else
                {
                    _snackAmount[1]--;
                    gm.curFull = gm.maxFull;
                }
            }
            else if (gm.curFull < 1000 && _snackAmount[2] > 0 && Input.GetKeyDown(KeyCode.Alpha3))
            {
                sound.clip = eatSource;
                sound.Play();
                if (gm.curFull + snackUse[2] < gm.maxFull)
                {
                    _snackAmount[2]--;
                    gm.curFull += snackUse[2];
                }
                else
                {
                    _snackAmount[2]--;
                    gm.curFull = gm.maxFull;
                }
            }
            else if (gm.curFull < 1000 && _snackAmount[3] > 0 && Input.GetKeyDown(KeyCode.Alpha4))
            {
                sound.clip = eatSource;
                sound.Play();
                if (gm.curFull + snackUse[3] < gm.maxFull)
                {
                    _snackAmount[3]--;
                    gm.curFull += snackUse[3];
                }
                else
                {
                    _snackAmount[3]--;
                    gm.curFull = gm.maxFull;
                }
            }
            else if (gm.curFull < 1000 && _snackAmount[4] > 0 && Input.GetKeyDown(KeyCode.Alpha5))
            {
                sound.clip = eatSource;
                sound.Play();
                if (gm.curFull + snackUse[4] < gm.maxFull)
                {
                    _snackAmount[4]--;
                    gm.curFull += snackUse[4];
                }
                else
                {
                    _snackAmount[4]--;
                    gm.curFull = gm.maxFull;
                }
            }
            else if (gm.curFull < 1000 && _snackAmount[5] > 0 && Input.GetKeyDown(KeyCode.Alpha6))
            {
                sound.clip = eatSource;
                sound.Play();
                if (gm.curFull + snackUse[5] < gm.maxFull)
                {
                    _snackAmount[5]--;
                    gm.curFull += snackUse[5];
                }
                else
                {
                    _snackAmount[5]--;
                    gm.curFull = gm.maxFull;
                }
            }
        }
    }

    //과자를 얻었을 때 쓰는 함수.
    public void AddSnack()
    {
        if(Time.timeScale < 1f)
        {
            Time.timeScale = 1f;
        }
        
        if (DropMinigameManager.getSnack == true)
        {
            _snackAmount[0]++;
            DropMinigameManager.getSnack = false;
        }
        else if (ArrowMinigameManager.getSnack == true)
        {
            _snackAmount[2]++;
            ArrowMinigameManager.getSnack = false;
        }
        else if (PlayerWin.getSnack == true)
        {
            _snackAmount[3]++;
            PlayerWin.getSnack = false;
        }
        else if (RandomSnack.getSnack1 == true)
        {
            _snackAmount[4]++;
            RandomSnack.getSnack1 = false;
        }
        else if (RandomSnack.getSnack2 == true)
        {
            _snackAmount[5]++;
            RandomSnack.getSnack2 = false;
        }
    }

    //UI창은 마우스로도 조작할 수 있도록 만들어 두었다.
    public void OnMouseUpAsButton()
    {
        state = !state;
        if (state == true)
        {
            sound.clip = uiSource;
            sound.Play();
            uiAni.SetTrigger("Show");
        }
        else if (state == false)
        {
            uiAni.SetTrigger("Hide");
        }
    }
}
