using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlueMoveBackup : MonoBehaviour
{
    public enum CurrentState { idle, follow, trace, through, dead };
    public CurrentState curState = CurrentState.idle;


    private NavMeshAgent agent;

    private Transform tr;
    private GameObject redObj;
    private Transform redTr;

    public float traceMinDist;
    public float traceMaxDist;
    private float dist;

    private bool isDead = false;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        tr = GetComponent<Transform>();

        redObj = GameObject.FindWithTag("RED");

        StartCoroutine(CheckState());
        StartCoroutine(CheckForAction());
        
    }

    IEnumerator CheckState()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(0.2f);

            redTr = redObj.GetComponent<Transform>();

            dist = Vector3.Distance(tr.position, redTr.position);
            Debug.Log(dist);

            if (traceMinDist <= dist && dist <= traceMaxDist)
            {
                curState = CurrentState.trace;
            }
            else
            {
                curState = CurrentState.through;
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
                case CurrentState.through:
                    agent.isStopped = true;
                    StartCoroutine(Crusing());
                    break;
                case CurrentState.trace:
                    StartCoroutine(Tracking());
                    agent.isStopped = false;
                    break;
            }

            yield return null;
        }

    }

    IEnumerator Crusing()
    {
        tr.Translate(Vector3.forward * (agent.speed * 0.5f) * Time.deltaTime);
        yield return null;
    }
    IEnumerator Tracking()
    {
        agent.destination = redTr.position;
        yield return null;
    }
}
