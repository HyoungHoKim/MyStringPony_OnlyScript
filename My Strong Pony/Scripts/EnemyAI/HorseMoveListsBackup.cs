using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HorseMoveListsBackup : MonoBehaviour
{
    //현재상태
    public enum CurrentState { idle, follow, trace, avoid, dead };
    public CurrentState currentState = CurrentState.idle;

    //자신
    private Transform tr;
    public float moveSpeedMin = 0.0f;
    public float moveSpeedMax = 10.0f;
    private float moveSpeedCal;

    //네비메쉬
    private NavMeshAgent agent;

    //애니메이션
    private Animator anim;
    private readonly int hashMoveF = Animator.StringToHash("MoveF");
    private readonly int hashRotateL = Animator.StringToHash("RotateL");
    private readonly int hashRotateR = Animator.StringToHash("RotateR");

    //검색 태그
    private string[] redAlphaTags = { "REDALPHA" };
    private string[] blueAlphaTags = { "BLUEALPHA" };
    private string[] redTags = { "RED", "REDALPHA" };
    private string[] blueTags = { "BLUE", "BLUEALPHA" };
    private string[] enemyTags;
    private string[] avoidTags = { "RED", "REDALPHA", "BLUE", "BLUEALPHA" };

    //태그로 찾은 오브젝트들
    private GameObject[] alphaObjs;
    private GameObject[] enemyObjs;
    private List<GameObject> avoidObjs = new List<GameObject>();

    //타겟
    private Transform targetAvoid;
    private Transform targetAlpha;
    private Transform targetEnemy;

    //타겟별 거리
    private float alphaTargetDist;
    private float enemyTargetDist;
    private float avoidTargetDist;

    //회피용 레이캐스트
    private Ray rayF;
    private Ray rayL;
    private Ray rayR;
    private RaycastHit targetHit;
    private int avoidMask;

    //최소 회피 거리
    public float avoidMinRange = 10.0f;




    //while문 조건
    private bool isDead = false;

    void Start()
    {
        //자신의 태그 판별
        switch (tag)
        {
            case "REDALPHA":
                enemyTags = blueAlphaTags;
                break;
            case "RED":
                alphaObjs = GameObject.FindGameObjectsWithTag("REDALPHA");
                enemyTags = blueTags;
                break;
            case "BLUEALPHA":
                enemyTags = redAlphaTags;
                break;
            case "BLUE":
                alphaObjs = GameObject.FindGameObjectsWithTag("BLUEALPHA");
                enemyTags = redTags;
                break;
        }
        anim = GetComponent<Animator>();
        avoidMask = LayerMask.GetMask("KNIGHT");
        agent = GetComponent<NavMeshAgent>();

        StartCoroutine(CheckState());
        //StartCoroutine(CheckAll());
        StartCoroutine(CheckForAction());


    }

    IEnumerator CheckAll()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);

            float closestDistSqr = Mathf.Infinity;
            Transform closestAvoid = null;

            //태그 가지고있는애들 전부 검색
            foreach (string tag in avoidTags)
            {
                //자기자신 제외
                avoidObjs.AddRange(GameObject.FindGameObjectsWithTag(tag));
                if (avoidObjs.IndexOf(this.gameObject) > -1)
                {
                    avoidObjs.RemoveAt(avoidObjs.IndexOf(this.gameObject));
                }

                //집단 안에 제일 가까운애 검색
                foreach (GameObject avoidObj in avoidObjs)
                {

                    Vector3 objectPos = avoidObj.transform.position;
                    avoidTargetDist = (objectPos - transform.position).sqrMagnitude;

                    if (avoidTargetDist < closestDistSqr)
                    {
                        closestDistSqr = avoidTargetDist;
                        closestAvoid = avoidObj.transform;
                    }
                }
                targetAvoid = closestAvoid;
            }
            Debug.Log(targetAvoid.gameObject.name);
            if (targetAvoid != null)
            {
                Debug.DrawLine(transform.position, targetAvoid.position, Color.yellow);
            }
        }

    }

    //스테이트 지정
    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return Y.Variables.waitForSeconds_200ms;

            if (Physics.Raycast(rayF, out targetHit, avoidMinRange, avoidMask))//회피 우선
            {
                currentState = CurrentState.avoid;
                yield return new WaitForSeconds(2.0f);

            }
            /*else if(Physics.Raycast(rayL, out targetHit, avoidMinRange, avoidMask))
            {
                currentState = CurrentState.avoid;
                Debug.Log(targetHit);
                yield return new WaitForSeconds(1.0f);
            }
            else if (Physics.Raycast(rayR, out targetHit, avoidMinRange, avoidMask))
            {
                currentState = CurrentState.avoid;
                Debug.Log(targetHit);
                //yield return new WaitForSeconds(1.0f);
            }*/
            else if (tag == "RED" || tag == "BLUE")//대장을 제외한 나머지는 기본상태 follow
            {
                currentState = CurrentState.follow;
            }
            else
                currentState = CurrentState.trace;//대장은 기본 trace
        }

    }

    //스테이트별 코루틴 실행
    IEnumerator CheckForAction()
    {
        while (!isDead)
        {
            //레이캐스트 실행
            float trX = transform.position.x;
            float trY = transform.position.y;
            float trZ = transform.position.z;

            Vector3 trRay = new Vector3(trX, trY + 1.0f, trZ);

            rayF = new Ray(trRay, transform.forward);
            Debug.DrawRay(rayF.origin, rayF.direction * avoidMinRange, Color.white);

            /*rayL = new Ray(trRay, -transform.right);
            rayR = new Ray(trRay, transform.right);
            Debug.DrawRay(rayL.origin, rayL.direction * avoidMinRange, Color.white);
            Debug.DrawRay(rayR.origin, rayR.direction * avoidMinRange, Color.white);*/



            tr = GetComponent<Transform>();

            //속도 보간 계산 하면서 이동
            moveSpeedCal = Mathf.Lerp(moveSpeedMin, moveSpeedMax, Time.time);
            tr.Translate(Vector3.forward.normalized * moveSpeedCal * Time.deltaTime);

            switch (currentState)
            {
                case CurrentState.idle:
                    AnimationIdle();
                    break;
                case CurrentState.follow:
                    Following();
                    break;
                case CurrentState.avoid:
                    Avoiding();
                    break;
                case CurrentState.trace:
                    Tracing();
                    break;
            }
            yield return null;
        }
    }
    void AnimationIdle()
    {
        anim.SetBool(hashMoveF, false);
        anim.SetBool(hashRotateL, false);
        anim.SetBool(hashRotateR, false);
    }
    void AnimationF()
    {
        anim.SetBool(hashMoveF, true);
        anim.SetBool(hashRotateL, false);
        anim.SetBool(hashRotateR, false);
    }
    void AnimationL()
    {
        anim.SetBool(hashMoveF, false);
        anim.SetBool(hashRotateL, true);
        anim.SetBool(hashRotateR, false);
    }
    void AnimationR()
    {
        anim.SetBool(hashMoveF, false);
        anim.SetBool(hashRotateL, false);
        anim.SetBool(hashRotateR, true);
    }

    //대장 따라가기
    void Following()
    {
        AlphaSearch();

        AnimationF();


        //타겟 위치로 방향 돌림
        Vector3 dir = targetAlpha.position - tr.position;
        Quaternion ro = Quaternion.LookRotation(dir);
        tr.rotation = Quaternion.Lerp(tr.rotation, ro, Time.deltaTime * 1.0f);

        /*Vector3 avoidDir = tr.InverseTransformPoint(targetAlpha.position);
        if (avoidDir.x <= 0.0f)
        {
            AnimationL();
            tr.Rotate(Vector3.up * (moveSpeedCal * -3.0f) * Time.deltaTime);
        }
        else
        {
            AnimationR();
            tr.Rotate(Vector3.up * (moveSpeedCal * 3.0f) * Time.deltaTime);
        }*/
    }


    //적 추적
    void Tracing()
    {
        EnemySearch();

        AnimationF();

        //타겟 위치로 방향 돌림
        agent.destination = targetEnemy.position;
        //Vector3 dir = targetEnemy.position - tr.position;
        //Quaternion ro = Quaternion.LookRotation(dir);
        //tr.rotation = Quaternion.Lerp(tr.rotation, ro, Time.deltaTime * 1.0f);

        /*Vector3 avoidDir = tr.InverseTransformPoint(targetEnemy.position);
        if (avoidDir.x <= 0.0f)
        {
            AnimationL();
            tr.Rotate(Vector3.up * (moveSpeedCal * -3.0f) * Time.deltaTime);
        }
        else
        {
            AnimationR();
            tr.Rotate(Vector3.up * (moveSpeedCal * 3.0f) * Time.deltaTime);
        }*/
    }

    //적 가까이에서 통과, 크게 회전
    void Avoiding()
    {
        //targetAvoid = targetHit.transform;
        //Vector3 avoidDir = tr.InverseTransformPoint(targetAvoid.position);

        AnimationL();

        //회피는 왼쪽으로만
        tr.Rotate(Vector3.up * (moveSpeedCal * -10.0f) * Time.deltaTime);
        /*if (avoidDir.x <= 0.0f)
        {
            AnimationL();
            tr.Rotate(Vector3.up * (moveSpeedCal * -5.0f) * Time.deltaTime);
        }
        else
        {
            AnimationR();
            tr.Rotate(Vector3.up * (moveSpeedCal * 5.0f) * Time.deltaTime);
        }*/
    }
    void AlphaSearch()
    {
        float closestDistSqr = Mathf.Infinity;
        Transform closestAlpha = null;

        //집단 안에 제일 가까운애 검색
        if (alphaObjs != null)
        {
            foreach (GameObject alPhaObj in alphaObjs)
            {
                Vector3 objectPos = alPhaObj.transform.position;
                alphaTargetDist = (objectPos - transform.position).sqrMagnitude;

                if (alphaTargetDist < closestDistSqr)
                {
                    closestDistSqr = alphaTargetDist;
                    closestAlpha = alPhaObj.transform;
                }
            }
            targetAlpha = closestAlpha;
        }

        /*if (targetAlpha != null)
        {
            if( tag == "RED")
                Debug.DrawLine(transform.position, targetAlpha.transform.position, Color.red);
            else if( tag == "BLUE")
                Debug.DrawLine(transform.position, targetAlpha.transform.position, Color.blue);
        }*/
    }

    void EnemySearch()
    {
        float closestDistSqr = Mathf.Infinity;
        Transform closestEnemy = null;

        //태그 가지고있는애들 전부 검색
        foreach (string tag in enemyTags)
        {
            enemyObjs = GameObject.FindGameObjectsWithTag(tag);

            //집단 안에 제일 가까운애 검색
            foreach (GameObject enemyObj in enemyObjs)
            {
                Vector3 objectPos = enemyObj.transform.position;
                enemyTargetDist = (objectPos - transform.position).sqrMagnitude;

                if (enemyTargetDist < closestDistSqr)
                {
                    closestDistSqr = enemyTargetDist;
                    closestEnemy = enemyObj.transform;
                }
            }
            targetEnemy = closestEnemy.transform;

            /*if (targetEnemy != null)
            {
                Debug.DrawLine(transform.position, targetEnemy.transform.position, Color.magenta);
            }*/
        }
    }
}
