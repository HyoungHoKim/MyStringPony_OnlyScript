using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereCast : MonoBehaviour
{
    public GameObject currentHitObject;

    public float sphereRadius = 3.0f;
    public float maxDistance = 1.0f;
    int avoidMask;

    

    private Vector3 origin;
    private Vector3 direction;

    private float currentHitDistance;

    private void Start()
    {
        avoidMask = LayerMask.GetMask("KNIGHT");
    }
    void Update()
    {

        float trX = transform.position.x;
        float trY = transform.position.y;
        float trZ = transform.position.z;


        origin = new Vector3(trX, trY, (trZ - 2.0f) - maxDistance);
        direction = transform.forward;
        RaycastHit targetHit;

        if (Physics.SphereCast(origin, sphereRadius, direction, out targetHit, maxDistance, avoidMask, QueryTriggerInteraction.UseGlobal))
        {
            currentHitObject = targetHit.transform.gameObject;
            currentHitDistance = targetHit.distance;
        }
        else
        {
            currentHitDistance = maxDistance;
            currentHitObject = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(origin, origin + direction * currentHitDistance);
        Gizmos.DrawWireSphere(origin + direction * currentHitDistance, sphereRadius);
    }
}
