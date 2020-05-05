using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

public class RidingHorsePose : MonoBehaviour
{
    public FullBodyBipedIK ik;

    public Transform LeftLegAnkleTarget, RightLegAnkleTarget, BodyTarget;
    
    private void LateUpdate()
    {
        ik.solver.leftFootEffector.position = LeftLegAnkleTarget.position;
        ik.solver.leftFootEffector.rotation = LeftLegAnkleTarget.rotation;
        ik.solver.leftFootEffector.positionWeight = 1f;
        ik.solver.leftFootEffector.rotationWeight = 1f;

        ik.solver.rightFootEffector.position = RightLegAnkleTarget.position;
        ik.solver.rightFootEffector.rotation = RightLegAnkleTarget.rotation;
        ik.solver.rightFootEffector.positionWeight = 1f;
        ik.solver.rightFootEffector.rotationWeight = 1f;

        ik.solver.bodyEffector.position = BodyTarget.position;
        ik.solver.bodyEffector.rotation = BodyTarget.rotation;
        ik.solver.bodyEffector.positionWeight = 1f;
        ik.solver.bodyEffector.rotationWeight = 1f;
    }
}
