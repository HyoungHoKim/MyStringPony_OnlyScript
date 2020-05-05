using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    private Transform transform_camera;
    private Transform transform_this;

    void Start()
    {
        transform_this = GetComponent<Transform>();
        StartCoroutine(CameraCheck());
    }
    IEnumerator CameraCheck()
    {
        while(transform_camera == null)
        {
            yield return null;
            transform_camera = Camera.main.transform;
        }        
    }
    void LateUpdate()
    {
        transform_this.LookAt(transform_camera.position);
    }
}