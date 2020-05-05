using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public enum CurrentState { poolLoopOff, poolLoopOn };
    public CurrentState currentState = CurrentState.poolLoopOff;


    public static EnemyManager instance = null;
    //Enemy 프리팹
    public GameObject redAlphaPref;
    public GameObject blueAlphaPref;
    public GameObject redPref;
    public GameObject bluePref;


    //오브젝트 풀링
    public List<GameObject> redAlphaList  = new List<GameObject>();
    public List<GameObject> blueAlphaList = new List<GameObject>();
    public List<GameObject> redList       = new List<GameObject>();
    public List<GameObject> blueList      = new List<GameObject>();


    //Enemy 스폰포인트 
    private Transform[] redAlphaSpawn;
    private Transform[] blueAlphaSpawn;
    private Transform[] redSpawn;
    private Transform[] blueSpawn;


    //Enemy 종류별 스폰포인트 개수
    private int rALength;
    private int bALength;
    private int rLength;
    private int bLength;
    

    void Awake()
    {
        //싱글톤
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }

        EnemySpawnPoints();  //Enemy 스폰포인트 검색
        EnemyCreatePooling();//Enemy 생성
        StartCoroutine(PoolLoopCheck());
    }
    



    void EnemySpawnPoints()
    {
        //Enemy 종류별 스폰포인트 찾기
        redAlphaSpawn  = GameObject.Find("RedAlphaSpawn").GetComponentsInChildren<Transform>();
        blueAlphaSpawn = GameObject.Find("BlueAlphaSpawn").GetComponentsInChildren<Transform>();
        redSpawn       = GameObject.Find("RedSpawn").GetComponentsInChildren<Transform>();
        blueSpawn      = GameObject.Find("BlueSpawn").GetComponentsInChildren<Transform>();


        //Enemy 종류별 스폰포인트 개수
        rALength = redAlphaSpawn.Length;
        bALength = blueAlphaSpawn.Length;
        rLength  = redSpawn.Length;
        bLength  = blueSpawn.Length;
    }


    //Enemy 생성 메소드
    void EnemyCreatePooling()
    {
        //Enemy를 생성해 차일드화할 부모 오브젝트 생성
        GameObject redAlphaPools  = new GameObject("RedAlphaPools");
        GameObject blueAlphaPools = new GameObject("BlueAlphaPools");
        GameObject redPools       = new GameObject("RedPools");
        GameObject bluePools      = new GameObject("BluePools");


        //Enemy 스폰위치에 생성
        for (int i = 0; i < rALength - 1; i++)                                              //레드알파 프리팹 생성
        {                                                                                   //
            var obj = Instantiate<GameObject>(redAlphaPref, redAlphaSpawn[i].transform);    //부모 오브젝트에 생성
                obj.transform.SetParent(redAlphaPools.transform);                           //부활하지 않는 Enemy라서 시작지점에 바로 이동
            
                obj.name = "RedAlpha_" + i.ToString("00");                                  //
                redAlphaList.Add(obj);                                                      //레드알파 배열에 넣기

        }
        for (int i = 0; i < bALength - 1; i++)                                              //블루알파 프리팹 생성
        {                                                                                   //
            var obj = Instantiate<GameObject>(blueAlphaPref, blueAlphaSpawn[i].transform); //부모오브젝트에 생성
                obj.transform.SetParent(blueAlphaPools.transform);                          //부활하지 않는 Enemy라서 시작지점에 바로 이동
                obj.name = "BlueAlpha_" + i.ToString("00");                                 //
                blueAlphaList.Add(obj);                                                     //블루알파 배열에 넣기

        }
        for (int i = 0; i < rLength - 1; i++)                                               //레드 프리팹을 생성
        {                                                                                   //
            var obj = Instantiate<GameObject>(redPref, redSpawn[i].transform);              //부모오브젝트에 생성
            obj.transform.SetParent(redPools.transform);                                    //
                obj.name = "Red_" + i.ToString("00");                                       //
                //obj.SetActive(false);                                                     //
                redList.Add(obj);                                                           //

        }
        for (int i = 0; i < bLength - 1; i++)                                               //블루 프리팹을 생성
        {                                                                                   //
            var obj = Instantiate<GameObject>(bluePref, blueSpawn[i].transform);            //부모오브젝트에 생성
            obj.transform.SetParent(bluePools.transform);                                   //
                obj.name = "Blue_" + i.ToString("00");                                      //
                //obj.SetActive(false);                                                     //
                blueList.Add(obj);                                                          //

        }
    }


    //2초마다 스폰 루프
    IEnumerator PoolLoopCheck()
    {
        while (!GameManager.instance.isGameOver)
        {
            yield return new WaitForSeconds(0.8f);
            if (!GameManager.instance.stay)
            {
                EnemySpawnLoop();
            }
        }
    }



    public GameObject GetRed()
    {
        for (int i = 0; i < redList.Count; i++)
        {
            if (redList[i].activeSelf == false)
            {
                return redList[i];
            }
        }
        return null;
    }
    
    
    public GameObject GetBlue()
    {
        for (int i = 0; i < blueList.Count; i++)
        {
            if (blueList[i].activeSelf == false)
            {
                return blueList[i];
            }
        }
        return null;
    }


    //Enemy 졸병 생성 루프
    void EnemySpawnLoop()
    {
        if (GetRed() != null)                                              //비활성화된(죽은) 레드졸병이 있을 경우
        {
            int spawnIndex = Random.Range(0, redSpawn.Length);      //랜덤 스폰포인트에 생성
            GetRed().transform.position = redSpawn[spawnIndex].transform.position;
            GetRed().transform.rotation = redSpawn[spawnIndex].transform.rotation;
            GetRed().SetActive(true);
        }


        if (GetBlue() != null)                                             //비활성화된(죽은) 블루졸병이 있을 경우
        {
            int spawnIndex = Random.Range(0, blueSpawn.Length);     //랜덤 스폰포인트에 생성
            GetBlue().transform.position = blueSpawn[spawnIndex].transform.position;
            GetBlue().transform.rotation = blueSpawn[spawnIndex].transform.rotation;
            GetBlue().SetActive(true);
        }
    }
}
