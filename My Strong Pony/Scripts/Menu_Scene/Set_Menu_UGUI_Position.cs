using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 카메라의 Position 값을 가져와 이 오브젝트에 설정한다
public class Set_Menu_UGUI_Position : MonoBehaviour
{
    [SerializeField] Vector3 vector3_position;

    void Start()
    {
        var camera = GameObject.Find("Camera").GetComponent<Transform>();

        var transform_this = GetComponent<Transform>();

        transform_this.position = camera.position + vector3_position;
    }
}