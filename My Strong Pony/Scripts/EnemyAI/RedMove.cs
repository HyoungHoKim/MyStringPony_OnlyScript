using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RedMove : MonoBehaviour
{
    public enum CurrentState { idle, follow, trace, through, dead };
    public CurrentState curState = CurrentState.idle;


    private NavMeshAgent agent;

    private GameObject redAlphaObj;
    private Transform redAlphaTr;

    private Transform tr;
    private GameObject blueObj;
    private Transform blueTr;    

    public float traceMinDist;
    public float traceMaxDist;
    private float dist;

    public float throughTimer;
    int randomRo;

    private bool isDead = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tr = GetComponent<Transform>();

        redAlphaObj = GameObject.FindWithTag("REDALPHA");
        blueObj = GameObject.FindWithTag("BLUE");

        StartCoroutine(CheckState());
        StartCoroutine(CheckForAction());


    }

    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);

            redAlphaTr = redAlphaObj.GetComponent<Transform>();
            blueTr = blueObj.GetComponent<Transform>();

            dist = Vector3.Distance(tr.position, blueTr.position);

            if ( traceMaxDist < dist)
            {
                curState = CurrentState.follow;
            }
            else if ( traceMinDist <= dist && dist <= traceMaxDist)
            {
                curState = CurrentState.trace;
            }
            else if ( traceMinDist > dist )
            {
                curState = CurrentState.through;
                agent.isStopped = false;
                agent.speed = 0.0f;

                randomRo = Random.Range(0, 2);
                Debug.Log(randomRo);
                yield return new WaitForSecondsRealtime(throughTimer);
            }
        }

    }

    IEnumerator CheckForAction()
    {
        while (!isDead)
        {
            switch (curState)
            {
                case CurrentState.idle:
                    agent.isStopped = true;
                    break;
                case CurrentState.follow:
                    agent.isStopped = false;
                    agent.speed = 10.0f;
                    agent.destination = redAlphaTr.position;                    
                    break;
                case CurrentState.through:
                    tr.Translate(Vector3.forward * 10.0f * Time.deltaTime);

                    
                    if (randomRo == 0)
                    {
                        tr.Rotate(Vector3.up * 50 * Time.deltaTime);
                    }
                    else
                    {
                        tr.Rotate(Vector3.up * -50 * Time.deltaTime);
                    }

                    break;
                case CurrentState.trace:
                    agent.destination = blueTr.position;
                    agent.isStopped = false;
                    agent.speed = 10.0f;
                    break;
            }

            yield return null;
        }
        
    }
}
