using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorseMoveBackup : MonoBehaviour
{
    public enum CurrentState { idle, follow, trace, through, dead };
    public CurrentState currentState = CurrentState.idle;

    private Transform tr;

    private GameObject alphaObj;
    private Transform alphaTr;

    private GameObject enemyObj;
    private Transform enemyTr;
    
    public float enemyMinDist = 5.0f;
    public float enemyMaxDist = 30.0f;

    public float speed = 10.0f;

    Vector3 enemyDir;

    private bool isDead = false;
    void Start()
    {
        //자신의 태그 판별
        switch (tag)
        {
            case "REDALPHA":
                alphaObj = GameObject.FindWithTag("Player");
                enemyObj = GameObject.FindWithTag("RED");
                break;
            case "RED":
                alphaObj = GameObject.FindWithTag("REDALPHA");
                enemyObj = GameObject.FindWithTag("BLUE");
                break;
            case "BLUEALPHA":
                alphaObj = GameObject.FindWithTag("Player");
                enemyObj = GameObject.FindWithTag("RED");
                break;
            case "BLUE":
                alphaObj = GameObject.FindWithTag("BLUEALPHA");
                enemyObj = GameObject.FindWithTag("RED");
                break;
        }
        
        StartCoroutine(CheckState());
        StartCoroutine(CheckForAction());
    }

    //스테이트 지정
    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);
            if (enemyObj != null)
            {
                //위치 판별
                tr = GetComponent<Transform>();

                if (alphaObj != null)
                    alphaTr = alphaObj.GetComponent<Transform>();

                enemyTr = enemyObj.GetComponent<Transform>();

                float dist = Vector3.Distance(tr.position, enemyTr.position);

                //거리 판별, 스테이트 지정
                if (enemyMaxDist < dist && alphaObj != null)
                {
                    currentState = CurrentState.follow;
                }
                else if (enemyMinDist > dist)
                {
                    enemyDir = tr.InverseTransformPoint(enemyTr.position);
                    currentState = CurrentState.through;
                    yield return new WaitForSeconds(5.0f);
                }
                else
                {
                    currentState = CurrentState.trace;
                }
            }
            else
                currentState = CurrentState.idle;
            

            
            
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
                    break;
                case CurrentState.follow:
                    StartCoroutine(Following());
                    break;
                case CurrentState.through:
                    StartCoroutine(Throughing());
                    break;
                case CurrentState.trace:
                    StartCoroutine(Tracing());
                    break;
            }
            yield return null;
        }
    }

    //대장 따라가기
    IEnumerator Following()
    {
        tr.Translate(Vector3.forward.normalized * (speed * 0.7f) * Time.deltaTime);
        Vector3 dir = alphaTr.position - tr.position;
        Quaternion ro = Quaternion.LookRotation(dir);
        tr.rotation = Quaternion.Slerp(tr.rotation, ro, Time.deltaTime * 1.5f);
        yield return null;
    }

    //적 추적
    IEnumerator Tracing()
    {
        tr.Translate(Vector3.forward.normalized * speed * Time.deltaTime);

        Vector3 dir = enemyTr.position - tr.position;
        Quaternion ro = Quaternion.LookRotation(dir);
        tr.rotation = Quaternion.Slerp(tr.rotation, ro, Time.deltaTime * 1f);
        yield return null;
    }

    //적 가까이에서 통과, 크게 회전
    IEnumerator Throughing()
    {        
        tr.Translate(Vector3.forward.normalized * (speed) * Time.deltaTime);
        
        if (enemyDir.x <= 0.0f)
            tr.Rotate(Vector3.up * (speed * 4.0f) * Time.deltaTime);
        else
            tr.Rotate(Vector3.up * (speed * -4.0f) * Time.deltaTime);

        yield return null;
    }
}
