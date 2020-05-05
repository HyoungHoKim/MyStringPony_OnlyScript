using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //플레이어 프리팹
    public GameObject PlayerPref;
    //public GameObject bluePlayerPref;


    //플레이어 리스트
    public List<GameObject> PlayerList = new List<GameObject>();
    //public List<GameObject> bluePlayerList = new List<GameObject>();
    public int MultiplayCheck; //1이면 싱글 2면 멀티


    //플레이어 스폰지역
    private Transform redPlayerSpawn;
    private Transform bluePlayerSpawn;


    void Start()
    {
        //플레이어 스폰포인트 찾기
        redPlayerSpawn = GameObject.Find("RedPlayerSpawn").transform;
        bluePlayerSpawn = GameObject.Find("BluePlayerSpawn").transform;

        
        PlayerCreatePooling();              //플레이어 풀 생성
        StartCoroutine(PoolLoopCheck());    //플레이어 풀 루프 체크
    }
    

    //플레이어 생성 루프
    void PlayerCreatePooling()
    {
        //플레이어의 부모 오브젝트가 될 오브젝트 만들기
        GameObject PlayerParent = new GameObject("PlayerPools");
        //GameObject blueParent = new GameObject("BluePlayerPools");

            var obj = Instantiate<GameObject>(PlayerPref, redPlayerSpawn.transform);
            obj.transform.SetParent(PlayerParent.transform);
            obj.name = "RedPlayer";
            obj.tag = "REDALPHA";
            PlayerList.Add(obj);

        //MultiplayCheck가 2이면 멀티라서 2P생성함
        if (PlayerList.Count < MultiplayCheck)
        {
            var bobj = Instantiate<GameObject>(PlayerPref, bluePlayerSpawn.transform);
            bobj.transform.SetParent(PlayerParent.transform);
            bobj.name = "BLUEPlayer";
            bobj.tag = "BLUEALPHA";
            PlayerList.Add(bobj);
        }
    }

    public GameObject GetRedPlayer()
    {
        if (PlayerList[0].activeSelf == false)
        {
            return PlayerList[0];
        }
        return null;
    }

    public GameObject GetBluePlayer()
    {
        if (PlayerList[1].activeSelf == false)
        {
            return PlayerList[1];
        }
        return null;
    }

    IEnumerator PoolLoopCheck()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.2f);
            if (!GameManager.instance.stay)
            {
                yield return new WaitForSeconds(2.8f);
                PlayerSpawnLoop();
            }
        }
    }

    

    void PlayerSpawnLoop()
    {
        if (GetRedPlayer() != null)                                              //비활성화된(죽은) 레드졸병이 있을 경우
        {
            GetRedPlayer().transform.position = redPlayerSpawn.transform.position;
            GetRedPlayer().transform.rotation = redPlayerSpawn.transform.rotation;
            GetRedPlayer().SetActive(true);
        }


        if (GetBluePlayer() != null)                                             //비활성화된(죽은) 블루졸병이 있을 경우
        {
            GetBluePlayer().transform.position = bluePlayerSpawn.transform.position;
            GetBluePlayer().transform.rotation = bluePlayerSpawn.transform.rotation;
            GetBluePlayer().SetActive(true);
        }
    }
}
