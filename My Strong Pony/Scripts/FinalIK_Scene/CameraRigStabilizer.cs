using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class CameraRigStabilizer : MonoBehaviour
{
    public Transform HMD;
    public float heigth;
    public Transform headBonePos;

    private float basicOffset;
    public float idealHeight;


    private void Start()
    {
        //UnityEngine.XR.InputTracking.disablePositionalTracking = true;

        //Valve.VR.OpenVR.System.ResetSeatedZeroPose();
        UnityEngine.XR.InputTracking.Recenter();

        basicOffset = heigth / 100 / 2.0f - 0.1f;

        if (HMD.localPosition.y + basicOffset > idealHeight)
        {
            transform.localPosition = new Vector3(0, basicOffset - (HMD.localPosition.y + basicOffset - idealHeight), 0);
        }
        else
        {
            transform.localPosition = new Vector3(+0.051f, basicOffset + (idealHeight - (HMD.localPosition.y + basicOffset)), 0);
        }
    }

    private void Update()
    {
        if (HMD.localPosition.y + basicOffset > idealHeight)
        {
            transform.localPosition = new Vector3(0, basicOffset - (HMD.localPosition.y + basicOffset - idealHeight), 0);
        }
        else
        {
            transform.localPosition = new Vector3(+0.051f, basicOffset + (idealHeight - (HMD.localPosition.y + basicOffset)), 0);
        }

    }

}
