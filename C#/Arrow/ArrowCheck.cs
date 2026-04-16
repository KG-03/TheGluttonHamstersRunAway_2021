using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCheck : MonoBehaviour
{
    public ArrowMinigameManager aGm;

    int index = 0;
    static int indexMax = 9;
    bool checkOK = false;

    [Header("Audio")]
    public AudioSource arrowAudio;
    public AudioClip check_T;
    public AudioClip check_F;
    public AudioClip check_S;

    //전부 다 맞췄는지 확인.
    private bool _allCheck = false;
    public bool allCheck
    {
        get { return _allCheck; }
        set { _allCheck = value; }
    }

    private void Start()
    {
        aGm = this.GetComponent<ArrowMinigameManager>();
    }

    void Update()
    {
        if (aGm.isPause == true)
            return;

        if (index < indexMax)
        {
            if (aGm.gameOver == false)
            {
                CheckArrow();
            }
        }
        else
        {
            //한 사이클을 모두 끝냈을 때.
            arrowAudio.clip = check_S;
            arrowAudio.Play();

            index = 0;
            aGm.awList.Clear();
            allCheck = true;
        }
    }

    public void CheckArrow()
    {
        //ArrowMinigameManager.cs에서 이미 화살표를 생성할 때 z축을 기준으로 상 하 좌 우를 구분지어 두었기 때문에 가능한 방법.
        if (aGm.awList[index].transform.position.z == 1)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                arrowAudio.clip = check_T;
                arrowAudio.Play();

                Destroy(aGm.awList[index]);
                index++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                arrowAudio.clip = check_F;
                arrowAudio.Play();

                ArrowMinigameManager.life--;
            }
        }
        else if (aGm.awList[index].transform.position.z == 2)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                arrowAudio.clip = check_T;
                arrowAudio.Play();

                Destroy(aGm.awList[index]);
                index++;
            }
            else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                arrowAudio.clip = check_F;
                arrowAudio.Play();

                ArrowMinigameManager.life--;
            }
        }
        else if (aGm.awList[index].transform.position.z == 3 )
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                arrowAudio.clip = check_T;
                arrowAudio.Play();

                Destroy(aGm.awList[index]);
                index++;
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                arrowAudio.clip = check_F;
                arrowAudio.Play();

                ArrowMinigameManager.life--;
            }
        }
        else if (aGm.awList[index].transform.position.z == 4)
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                arrowAudio.clip = check_T;
                arrowAudio.Play();

                Destroy(aGm.awList[index]);
                index++;
            }
            else if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                arrowAudio.clip = check_F;
                arrowAudio.Play();

                ArrowMinigameManager.life--;
            }
        }
    }
}
