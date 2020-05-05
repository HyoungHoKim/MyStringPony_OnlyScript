using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseMoveTest : MonoBehaviour
{
    //현재상태
    public enum CurrentState { idle, follow, trace, avoidF, avoidL, avoidR, dead };
    public CurrentState currentState = CurrentState.idle;

    //자신
    private Transform tr;
    public float moveSpeed = 20.0f;
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
    //private string[] avoidTags = { "RED", "REDALPHA", "BLUE", "BLUEALPHA" };

    //태그로 찾은 오브젝트들
    private GameObject[] alphaObjs;
    private GameObject[] enemyObjs;
    //private List<GameObject> avoidObjs = new List<GameObject>();

    //타겟
    private Transform targetAvoid;
    private Transform targetAlpha;
    private Transform targetEnemy;

    //타겟별 거리
    private float alphaTargetDist;
    private float enemyTargetDist;

    //회피용 레이캐스트
    private Ray rayFront;
    private Ray rayLeft;
    private Ray rayRight;
    private RaycastHit targetHit;
    private RaycastHit targetHitLeft;
    private RaycastHit targetHitRight;
    private int avoidMask;
    public float avoidFront = 10.0f;
    public float avoidSide = 3.0f;



    //public float enemyMaxDist = 250.0f;
    //public float enemyMinDist = 20.0f;



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

        //StartCoroutine(CheckState());
        StartCoroutine(CheckForAction());

    }

    /*IEnumerator CheckAll()
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
            if (targetAvoid != null)
            {
                Debug.DrawLine(transform.position, targetAvoid.position, Color.blue);
            }
        }
        
    }*/


    private void Update()
    {
        float trX = transform.position.x;
        float trY = transform.position.y;
        float trZ = transform.position.z;

        Vector3 trRay = new Vector3(trX, trY + 1.0f, trZ);

        //레이캐스트
        rayFront = new Ray(trRay, Vector3.forward);
        Debug.DrawRay(rayFront.origin, rayFront.direction * avoidFront, Color.white);

        /*rayLeft = new Ray(trRay, Vector3.left);
        Debug.DrawRay(rayLeft.origin, rayLeft.direction * avoidSide, Color.white);

        rayRight = new Ray(trRay, Vector3.right);
        Debug.DrawRay(rayRight.origin, rayRight.direction * avoidSide, Color.white);*/

        tr = GetComponent<Transform>();
        tr.Translate(Vector3.forward.normalized * moveSpeed * Time.deltaTime);

        //스테이트 판별
        if (Physics.Raycast(rayFront, out targetHit, avoidFront, avoidMask))//앞쪽
            currentState = CurrentState.avoidF;
        /*else if (Physics.Raycast(rayLeft, out targetHit, avoidSide, avoidMask))//왼쪽
            currentState = CurrentState.avoidL;
        else if (Physics.Raycast(rayRight, out targetHit, avoidSide, avoidMask))//오른쪽
            currentState = CurrentState.avoidR;*/
        else if (tag == "RED" || tag == "BLUE")
        {
            currentState = CurrentState.follow;
        }
        else
            currentState = CurrentState.trace;

    }

    //스테이트 지정
    IEnumerator CheckState()
    {
        //WaitForSeconds wait200ms = new WaitForSeconds(0.2f);

        while (!isDead)
        {
            yield return Y.Variables.waitForSeconds_200ms;



        }

    }

    //스테이트별 코루틴 실행
    IEnumerator CheckForAction()
    {
        while (!isDead)
        {
            switch (currentState)
            {
                case CurrentState.idle:
                    anim.SetBool(hashMoveF, false);
                    anim.SetBool(hashRotateL, false);
                    anim.SetBool(hashRotateR, false);
                    break;
                case CurrentState.avoidF:
                    Avoiding();
                    yield return new WaitForSeconds(1.0f);
                    break;
                case CurrentState.avoidL:
                    AvoidingLeft();
                    yield return new WaitForSeconds(1.0f);
                    break;
                case CurrentState.avoidR:
                    AvoidingRight();
                    yield return new WaitForSeconds(1.0f);
                    break;
                case CurrentState.follow:
                    Following();
                    break;
                case CurrentState.trace:
                    Tracing();
                    break;
            }
            yield return null;
        }
    }


    void Avoiding()
    {
        targetAvoid = targetHit.transform;
        Vector3 avoidDir = tr.InverseTransformPoint(targetAvoid.position);
        if (avoidDir.x < 2.0f)
        {
            anim.SetBool(hashMoveF, false);
            anim.SetBool(hashRotateL, true);
            anim.SetBool(hashRotateR, false);
            tr.Rotate(Vector3.up * (moveSpeed * -3.0f) * Time.deltaTime);
        }
        else if (avoidDir.x > 2.0f)
        {
            anim.SetBool(hashMoveF, false);
            anim.SetBool(hashRotateL, false);
            anim.SetBool(hashRotateR, true);
            tr.Rotate(Vector3.up * (moveSpeed * 3.0f) * Time.deltaTime);
        }
        else
        {
            anim.SetBool(hashRotateL, false);
            anim.SetBool(hashRotateR, false);
            anim.SetBool(hashMoveF, true);
        }
    }


    void AvoidingLeft()
    {
        anim.SetBool(hashMoveF, false);
        anim.SetBool(hashRotateL, true);
        anim.SetBool(hashRotateR, false);
        tr.Rotate(Vector3.up * (moveSpeed * -3.0f) * Time.deltaTime);
    }


    void AvoidingRight()
    {
        anim.SetBool(hashMoveF, false);
        anim.SetBool(hashRotateL, false);
        anim.SetBool(hashRotateR, true);
        tr.Rotate(Vector3.up * (moveSpeed * 3.0f) * Time.deltaTime);
    }

    //대장 따라가기
    void Following()
    {
        AlphaSearch();

        Vector3 avoidDir = tr.InverseTransformPoint(targetAlpha.position);
        if (avoidDir.x < 2.0f)
        {
            anim.SetBool(hashMoveF, false);
            anim.SetBool(hashRotateL, true);
            anim.SetBool(hashRotateR, false);
            tr.Rotate(Vector3.up * (moveSpeed * -3.0f) * Time.deltaTime);
        }
        else if (avoidDir.x > 2.0f)
        {
            anim.SetBool(hashMoveF, false);
            anim.SetBool(hashRotateL, false);
            anim.SetBool(hashRotateR, true);
            tr.Rotate(Vector3.up * (moveSpeed * 3.0f) * Time.deltaTime);
        }
        else
        {
            anim.SetBool(hashRotateL, false);
            anim.SetBool(hashRotateR, false);
            anim.SetBool(hashMoveF, true);
        }
    }


    //적 추적
    void Tracing()
    {
        EnemySearch();
        Vector3 avoidDir = tr.InverseTransformPoint(targetEnemy.position);
        if (avoidDir.x < 2.0f)
        {
            anim.SetBool(hashMoveF, false);
            anim.SetBool(hashRotateL, true);
            anim.SetBool(hashRotateR, false);
            tr.Rotate(Vector3.up * (moveSpeed * -3.0f) * Time.deltaTime);
        }
        else if (avoidDir.x > 2.0f)
        {
            anim.SetBool(hashMoveF, false);
            anim.SetBool(hashRotateL, false);
            anim.SetBool(hashRotateR, true);
            tr.Rotate(Vector3.up * (moveSpeed * 3.0f) * Time.deltaTime);
        }
        else
        {
            anim.SetBool(hashRotateL, false);
            anim.SetBool(hashRotateR, false);
            anim.SetBool(hashMoveF, true);
        }
    }

    //적 가까이에서 통과, 크게 회전

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

        if (targetAlpha != null)
        {
            if (tag == "RED")
                Debug.DrawLine(transform.position, targetAlpha.transform.position, Color.red);
            else if (tag == "BLUE")
                Debug.DrawLine(transform.position, targetAlpha.transform.position, Color.blue);
        }
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

            if (targetEnemy != null)
            {
                Debug.DrawLine(transform.position, targetEnemy.transform.position, Color.magenta);
            }
        }
    }
}
