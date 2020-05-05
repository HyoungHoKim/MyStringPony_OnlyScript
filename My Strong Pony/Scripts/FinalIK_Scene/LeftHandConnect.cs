using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class LeftHandConnect : MonoBehaviour
{
    public FullBodyBipedIK ik;

    public Transform LeftHandAnkleTarget;
    
    private void LateUpdate()
    {
        ik.solver.leftHandEffector.position = LeftHandAnkleTarget.position;
        ik.solver.leftHandEffector.rotation = LeftHandAnkleTarget.rotation;
        ik.solver.leftHandEffector.positionWeight = 1f;
        ik.solver.leftHandEffector.rotationWeight = 1f;


    }
}
