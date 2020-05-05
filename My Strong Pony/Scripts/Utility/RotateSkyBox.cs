using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSkyBox : MonoBehaviour
{
    public float RotateSpeed = 1.0f;
    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", RotateSpeed * Time.time);
        
    }
}
