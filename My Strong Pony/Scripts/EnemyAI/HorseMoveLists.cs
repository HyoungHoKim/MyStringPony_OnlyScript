using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HorseMoveLists : MonoBehaviour
{
    //현재상태
    public enum CurrentState { idle, trace, avoid, dead, victory, defeat };
    public CurrentState currentState = CurrentState.idle;

    private EnemyManager enemyMgr;

    //자신이 가지고 있는 컴포넌트
    public Transform tr;
    public float moveSpeed = 10.0f;
    private float moveSpeedCal;
    public NavMeshAgent navAgent;
    public Animator animator;
    private Transform originPos;
    public AudioClip[] horseClip;
    private AudioSource horseAudio;
    private AudioSource deadAudio;

    private float humanHp = 100;


    //애니메이션
    private readonly int hashMoveF = Animator.StringToHash("MoveF");


    //EnemyManager의 배열에서 타겟을 찾아옮
    private List<GameObject> targetObjs;
    private Transform targetTr;


    //회피용 레이캐스트
    Vector3 origin;
    private RaycastHit castHit;
    public Transform hittedTr;

    private int avoidMask;
    public float radius = 7.0f;
    public float maxDistance = 1.0f;


    //게임매니저 참조 while문 조건
    private bool isGameOver;
    private bool stay;
    private bool redWin;
    private bool blueWin;
    private bool draw;


    //Enemy 스크립트용 bool
    public bool isVictory = true;
    public bool isDefeat  = true;


    //while문 조건
    public  bool isHorseDead = false;

    private void Awake()
    {
        //기본 컴포넌트 초기화

        navAgent = this.GetComponent<NavMeshAgent>();
        animator = this.GetComponent<Animator>();
        avoidMask = LayerMask.GetMask("KNIGHT");

        deadAudio = this.gameObject.AddComponent<AudioSource>();
        horseAudio = this.GetComponent<AudioSource>();
        isGameOver = false;
        stay = true;
        blueWin = false;
        redWin = false;
        draw = false;
        humanHp = 100.0f;
    }
    void Start()
    {
        
    }
    private void OnEnable()
    {
        StartCoroutine(CheckMyTag());
        StartCoroutine(GameMgrBool());
        StartCoroutine(CheckState());
        StartCoroutine(CheckForAction());
        currentState = CurrentState.idle;
        animator.enabled = true;
    }



    IEnumerator GameMgrBool()
    {
        while (!isHorseDead)
        {
            yield return new WaitForSeconds(0.5f);

            isGameOver = GameManager.instance.isGameOver;
            stay       = GameManager.instance.stay;
            redWin     = GameManager.instance.redWin;
            blueWin    = GameManager.instance.blueWin;
            draw       = GameManager.instance.draw;
            humanHp    = GetComponent<Enemy>().floatHp;
        }
    }


    void HorseRun()
    {
        tr.Translate(Vector3.forward.normalized
                           * moveSpeed
                           * Time.deltaTime);
    }


    IEnumerator CheckMyTag()
    {
        switch (tag)
        {
            case "REDALPHA":
                targetObjs = EnemyManager.instance.blueAlphaList;
                break;
            case "RED":
                targetObjs = EnemyManager.instance.redAlphaList;
                break;
            case "BLUEALPHA":
                targetObjs = EnemyManager.instance.redAlphaList;
                break;
            case "BLUE":
                targetObjs = EnemyManager.instance.blueAlphaList;
                break;
        }
        StartCoroutine(TargetCheck());              //따라갈 가장 가까운 타겟 검색
        yield return null;
    }


    //스테이트 지정
    IEnumerator CheckState()
    {
        while (!isHorseDead)
        {
            //yield return new WaitForSeconds(0.2f);
            yield return null;
            //스피어캐스트 실행
            origin = new Vector3(tr.position.x, tr.position.y, (tr.position.z + 1.0f) - maxDistance);
            hittedTr = null;


            //게임매니저 게임플레이
            if (humanHp >0.0f && !isGameOver && !stay && !redWin && !blueWin && !draw)
            {
                if (Physics.SphereCast(origin
                                    , radius
                                    , tr.forward
                                    , out castHit
                                    , maxDistance
                                    , avoidMask
                                    , QueryTriggerInteraction.UseGlobal))
                {
                    hittedTr = castHit.transform;
                    currentState = CurrentState.avoid;
                }
                else if (!isGameOver)
                {
                    currentState = CurrentState.trace;
                }
            }
            else if (humanHp > 0.0f && !isGameOver && stay && !redWin && !blueWin && !draw) //게임매니저 스테이
            {
                currentState = CurrentState.idle;
            }
            else if (humanHp > 0.0f && isGameOver && !stay && redWin && !blueWin && !draw)  //게임매니저 레드승
            {
                StartCoroutine(RedVictory());
            }
            else if (humanHp > 0.0f && isGameOver && !stay && !redWin && blueWin && !draw)  //게임매니저 블루승
            {
                StartCoroutine(BlueVictory());
            }
            else if (humanHp > 0.0f && isGameOver && !stay && !redWin && !blueWin && draw)  // 게임매니저 비김
            {
                currentState = CurrentState.defeat;
            }

            if (isHorseDead)
            {
                currentState = CurrentState.dead;
            }
        }
    }

    //따라갈 가장 가까운 타겟 검색
    IEnumerator TargetCheck()
    {
        while (!isGameOver && !isHorseDead)
        {
            yield return new WaitForSeconds(0.5f);
            float closestSqr = Mathf.Infinity;  //가장 가까운 거리
            Transform closestTarget = null;     //가장 가까운 타겟
            float targetDist;                   //자신과 타겟과의 거리

            if (targetObjs != null)
            {
                for (int i = 0; i < targetObjs.Count; i++)        //Enemy매니저의 배열에 등록된 오브젝트 개수까지 검색
                {                    
                    Vector3 objectPos = targetObjs[i].transform.position;   //찾은 타겟의 포지션
                    targetDist = (objectPos - tr.position).sqrMagnitude;    //자신과 타겟과의 현재 거리

                    if(targetDist < closestSqr)                     //타겟과의 현재 거리가 가장 가까운 거리보다 가까우면
                    {
                        closestSqr = targetDist;                    //현재 거리가 가장 가까운 거리가 됨
                        closestTarget = targetObjs[i].transform;    //가장 가까운 타겟은 지금 거리계산한 타겟
                    }
                }                
                targetTr = closestTarget; //타겟은 가장 가까운 타겟
            }
            yield return null;
        }        
    }
    

    IEnumerator RedVictory()
    {
        if (this.gameObject.CompareTag("RED")
         || this.gameObject.CompareTag("REDALPHA"))
        {
            currentState = CurrentState.victory;
        }
        else
        {
            currentState = CurrentState.defeat;
        }
        yield return null;
    }
    IEnumerator BlueVictory()
    {
        if (this.gameObject.CompareTag("BLUE")
         || this.gameObject.CompareTag("BLUEALPHA"))
        {
            currentState = CurrentState.victory;
        }
        else
        {
            currentState = CurrentState.defeat;
        }
        yield return null;
    }


    //스테이트별 코루틴 실행
    IEnumerator CheckForAction()
    {
        while (!isHorseDead)
        {
            switch (currentState)
            {
                    //기본
                case CurrentState.idle:
                    animator.SetBool(hashMoveF, false);
                    navAgent.isStopped = true;
                    break;


                    //회피
                case CurrentState.avoid:
                    animator.SetBool(hashMoveF, true);
                    navAgent.isStopped = false;
                    HorseRun();
                    Avoiding();
                    break;


                    //추적
                case CurrentState.trace:
                    animator.SetBool(hashMoveF, true);
                    navAgent.isStopped = false;
                    HorseRun();
                    TargetTrace();
                    break;


                    //승리
                case CurrentState.victory:
                    animator.SetBool(hashMoveF, true);
                    navAgent.isStopped = false;
                    isVictory = true;
                    isDefeat = false;
                    HorseRun();
                    TargetTrace();
                    break;


                    //패배
                case CurrentState.defeat:
                    animator.SetBool(hashMoveF, false);
                    navAgent.isStopped = true;
                    isVictory = false;
                    isDefeat = true;
                    break;


                    //죽음
                case CurrentState.dead:
                    animator.SetBool(hashMoveF, false);
                    navAgent.isStopped = true;
                    isVictory = false;
                    isDefeat = false;
                    break;


            }
            yield return null;
        }
    }


    //장애물 피하기
    void Avoiding()
    {
        if(castHit.transform != null)
        {
            Vector3 avoidDir = tr.InverseTransformPoint(castHit.transform.position);
            animator.SetBool(hashMoveF, true);

            if (avoidDir.x <= 0.0f)
            {
                tr.Rotate(Vector3.up * (moveSpeedCal * -5.0f) * Time.deltaTime);
            }
            else
            {
                tr.Rotate(Vector3.up * (moveSpeedCal * 5.0f) * Time.deltaTime);
            }
        }        
    }

    //Navmesh 추적
    void TargetTrace()
    {
        if (!isHorseDead && targetTr != null)
        {
            navAgent.destination = targetTr.position;
        }
        else
        {
            navAgent.isStopped = true;
        }
    }

    private void OnDisable()
    {
        isHorseDead = false;
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        currentState = CurrentState.idle;
        navAgent.isStopped = true;
        animator.enabled = false;
    }

    /*public void Dead()
    {
        isHorseDead = true;
        navAgent.isStopped = true;
        navAgent.enabled = false;
    }*/

    /*
    void SetNavMeshAgentDestination(Transform tr)
    {
        // MissingReferenceEception 으로 Null 체크
        if (navAgent != null && tr != null)
        {
            navAgent.destination = tr.position;
        }
    }*/

    /*private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + tr.forward * hitDistance);
        Gizmos.DrawWireSphere(origin + tr.forward * hitDistance, radius);
    }*/
}
