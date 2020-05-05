using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class View : MonoBehaviour
{
    public float viewAngle = 15.0f;    //시야각
    public float viewDistance = 5.0f; //시야거리

    public LayerMask TargetMask;    //Enemy 레이어마스크 지정을 위한 변수
    public LayerMask ObstacleMask;  //Obstacle 레이어마스크 지정 위한 변수

    private Transform tr;

    void Awake()
    {
        tr = GetComponent<Transform>();
    }

    void Update()
    {
        DrawView();             //Scene뷰에 시야범위 그리기
        FindVisibleTargets();   //Enemy인지 Obstacle인지 판별
    }

    public Vector3 DirFromAngle(float angleInDegrees)
    {
        //탱크의 좌우 회전값 갱신
        angleInDegrees += transform.eulerAngles.y;
        //경계 벡터값 반환
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public void DrawView()
    {
        Vector3 leftBoundary = DirFromAngle(-viewAngle * 0.5f);
        Vector3 rightBoundary = DirFromAngle(viewAngle * 0.5f);
        Debug.DrawLine(tr.position, tr.position + leftBoundary * viewDistance, Color.blue);
        Debug.DrawLine(tr.position, tr.position + rightBoundary * viewDistance, Color.blue);
    }

    public void FindVisibleTargets()
    {
        //시야거리 내에 존재하는 모든 컬라이더 받아오기
        Collider[] targets = Physics.OverlapSphere(tr.position, viewDistance, TargetMask);

        for (int i = 0; i < targets.Length; i++)
        {
            Transform target = targets[i].transform;

            //탱크로부터 타겟까지의 단위벡터
            Vector3 dirToTarget = (target.position - tr.position).normalized;

            //tr.forward와 dirToTarget은 모두 단위벡터이므로 내적값은 두 벡터가 이루는 각의 Cos값과 같다.
            //내적값이 시야각/2의 Cos값보다 크면 시야에 들어온 것이다.
            if (Vector3.Dot(tr.forward, dirToTarget) > Mathf.Cos((viewAngle * 0.5f) * Mathf.Deg2Rad))
            //if (Vector3.Angle(tr.forward, dirToTarget) < viewAngle/2)
            {
                float distToTarget = Vector3.Distance(tr.position, target.position);

                if (!Physics.Raycast(tr.position, dirToTarget, distToTarget, ObstacleMask))
                {
                    Debug.DrawLine(tr.position, target.position, Color.red);
                }
            }
        }
    }
}
