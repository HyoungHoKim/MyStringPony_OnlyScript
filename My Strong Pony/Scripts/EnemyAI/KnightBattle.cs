using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightBattle : MonoBehaviour
{
    private Transform tr;

    //애니메이션
    private Animator anim;
    private readonly int hashMoveF = Animator.StringToHash("MoveF");
    private readonly int hashRotateL = Animator.StringToHash("RotateL");
    private readonly int hashRotateR = Animator.StringToHash("RotateR");

    //검색 태그
    private string[] redTags = { "RED", "REDALPHA" };
    private string[] blueTags = { "BLUE", "BLUEALPHA" };
    private string[] enemyTags;

    //태그로 찾은 오브젝트들
    private GameObject[] enemyObjs;

    //타겟
    private Transform targetEnemy;

    //타겟 거리
    public float enemyCloseDist = 5.0f;
    private float enemyTargetDist;




    private bool isDead = false;
    void Start()
    {
        switch (tag)
        {
            case "REDALPHA":
                enemyTags = blueTags;
                break;
            case "RED":
                enemyTags = blueTags;
                break;
            case "BLUEALPHA":
                enemyTags = redTags;
                break;
            case "BLUE":
                enemyTags = redTags;
                break;
        }
        Debug.Log(enemyTags);
        anim = GetComponent<Animator>();
        StartCoroutine(EnemyCheck());
    }

    IEnumerator EnemyCheck()
    {
        while (!isDead)
        {
            yield return null;
            EnemySearch();

            float currDist = (targetEnemy.position - transform.position).sqrMagnitude;
            if (targetEnemy != null)
            {
            }
            
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

            /*if (targetEnemy != null)
            {
                Debug.DrawLine(transform.position, targetEnemy.transform.position, Color.black);
            }*/
        }
    }
    void EnemyAttack()
    {
        Vector3 avoidDir = tr.InverseTransformPoint(targetEnemy.position);

        if (avoidDir.x <= 0.0f)
        {
            Debug.Log("Left");
        }
        else
        {
            Debug.Log("Right");
        }
    }
}
