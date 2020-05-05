using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;


    //게임 진행 상태
    public enum CurrentState { stay, play, redWin, blueWin, draw };
    public CurrentState currentState = CurrentState.stay;
    

    //게임 종료 판별
    public bool isGameOver = false;
    public bool stay       = true;
    public bool redWin     = false;
    public bool blueWin    = false;
    public bool draw       = false;

    public AudioClip[] backgroundMusic;
    private AudioSource audioSource;


    //싱글톤 로직
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        audioSource = this.GetComponent<AudioSource>();
        StartCoroutine(StateCheck());     //게임상태 체크
        StartCoroutine(CheckForAction()); //게임 상태별 행동
    }


    // 게임 상태 체크 (대기, 플레이, 레드 승리, 블루 승리, 무승부)
    IEnumerator StateCheck()
    {        
        yield return new WaitForSeconds(5.0f);                  //게임 로딩하고 5초 뒤부터 체크 시작
        while (!isGameOver)                                        //bool isGameOver = true가 될 때까지 계속
        {
            yield return new WaitForSeconds(0.2f);


            //각 팀 대장 개수 판별
            int redActive = 0;
            for (int i = 0; i < EnemyManager.instance.redAlphaList.Count; i++)
            {
                if (EnemyManager.instance.redAlphaList[i].activeSelf == true)
                {
                    redActive++;
                }
            }
            int blueActive = 0;
            for (int i = 0; i < EnemyManager.instance.blueAlphaList.Count; i++)
            {
                if (EnemyManager.instance.blueAlphaList[i].activeSelf == true)
                {
                    blueActive++;
                }
            }

            int blueAmount = blueActive;
            int redAmount = redActive;

            //스테이트 판별
            if      (redAmount >= 1
                 && blueAmount >= 1)  //양팀에 대장이 1명 이상 남았을 때 플레이
            {                
                currentState = CurrentState.play;
            }            
            else if (redAmount >= 1
                 && blueAmount <= 0)  //블루알파가 다 죽었을 때 레드 승리
            {
                currentState = CurrentState.redWin;
            }            
            else if (redAmount <= 0   //레드알파가 다 죽었을 때 블루 승리
                 && blueAmount >= 1) 
            {
                currentState = CurrentState.blueWin;
            }            
            else if (redAmount <= 0   //양쪽 다 죽었을 때 무승부(승리판정 났을 때 서로 다 죽인 경우 오류 방지)
                 && blueAmount <= 0)  
            {
                currentState = CurrentState.draw;
            }
        }
    }


    //게임 상태별 행동
    IEnumerator CheckForAction()
    {
        while (!isGameOver)
        {
            yield return new WaitForSeconds(0.2f);

            switch (currentState)
            {
                    //대기
                case CurrentState.stay:
                    isGameOver = false;
                    stay       = true;
                    redWin     = false;
                    blueWin    = false;
                    draw       = false;

                    StartCoroutine(BattleHornMusic());
                    break;


                    //플레이
                case CurrentState.play:
                    isGameOver = false;
                    stay       = false;
                    redWin     = false;
                    blueWin    = false;
                    draw       = false;
                    break;


                    //레드 승
                case CurrentState.redWin:
                    isGameOver = true;
                    stay       = false;
                    redWin     = true;
                    blueWin    = false;
                    draw       = false;

                    audioSource.clip = backgroundMusic[1];
                    audioSource.Play();
                    break;


                    //블루 승
                case CurrentState.blueWin:
                    isGameOver = true;
                    stay       = false;
                    redWin     = false;
                    blueWin    = true;
                    draw       = false;

                    audioSource.clip = backgroundMusic[1];
                    audioSource.Play();
                    break;


                    //비김
                case CurrentState.draw:
                    isGameOver = true;
                    stay       = false;
                    redWin     = false;
                    blueWin    = false;
                    draw       = true;

                    audioSource.clip = backgroundMusic[1];
                    audioSource.Play();
                    break;
            }
        }        
    }
    IEnumerator BattleHornMusic()
    {
        audioSource.clip = backgroundMusic[0];
        audioSource.Play();
        StartCoroutine(loopBgm());
        yield return null;
    }
    IEnumerator loopBgm()
    {
        while (!isGameOver)
        {
            yield return null;
            if (!audioSource.isPlaying)
            {
                int bgmCount = backgroundMusic.Length - 1;
                audioSource.clip = backgroundMusic[Random.Range(2, bgmCount)];
                audioSource.volume = 1.0f;
                audioSource.loop = false;
                audioSource.Play();
            }
        }
    }
}
