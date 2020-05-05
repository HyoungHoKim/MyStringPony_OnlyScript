using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Y
{
    // 변수들
    public partial class Game_Scene : MonoBehaviour
    {
        readonly float float_speed = 3.0f;
        readonly float float_rotation_speed = 2.0f;

        Vector3 vector3_move_direction = Vector3.zero;

        Transform transform_camera_rig;
        CharacterController character_controller;
    }

    // 유니티 함수
    public partial class Game_Scene : MonoBehaviour
    {
        void Start()
        {
            Init();
        }

        void Update()
        {
            vector3_move_direction.z = Input.GetAxis("Vertical") * float_speed;
            transform_camera_rig.Rotate(0, Input.GetAxis("Horizontal") * float_rotation_speed, 0);

            Vector3 global_direction = transform_camera_rig.TransformDirection(vector3_move_direction);
            character_controller.Move(global_direction * Time.deltaTime);
        }
    }

    // 함수
    public partial class Game_Scene : MonoBehaviour
    {
        void Init()
        {
            var camera = transform.Find("[CameraRig]/Camera");

            var horse = transform.Find("Horse");
            var horse_local_position = new Vector3(camera.localPosition.x, camera.localPosition.y - 1.6f, camera.localPosition.z);
            horse.localPosition = horse_local_position;

            transform_camera_rig = transform.Find("[CameraRig]");
            character_controller = transform.Find("[CameraRig]").GetComponent<CharacterController>();
        }
    }
}