using TMPro;
using Valve.VR;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// [CameraRig] > Controller(right) > Cube_Sword에 스크립트 추가
namespace Y
{
    // 오른손 컨트롤러 velocity의 거리를 재어 빠름을 text에 표시한다.

    public class Hit_Test_Scene : MonoBehaviour
    {
        [SerializeField] SteamVR_Behaviour_Pose controllerPose;
        [SerializeField] TMP_Text tmpTextVelocityMagnitude;

        private void OnTriggerEnter(Collider other)
        {
            bool isEnemyBody = other.CompareTag("ENEMY_BODY");
            bool isEnemyHead = other.CompareTag("ENEMY_HEAD");

            if (isEnemyBody || isEnemyHead)
            {
                print(other.gameObject.name);
                var enemy = other.gameObject.GetComponentInParent<Enemy>();

                // 수치 판정
                float magnitude = controllerPose.GetVelocity().magnitude;

                if (float.IsNaN(magnitude))
                {
                    tmpTextVelocityMagnitude.text = $"";
                }
                else
                {
                    tmpTextVelocityMagnitude.text = $"{controllerPose.GetVelocity().magnitude}";
                    print($"attack velocity = {controllerPose.GetVelocity().magnitude}");

                    float damage = isEnemyHead ? 30.0f : 20.0f;
                    if (enemy != null)
                    {
                        enemy.HitDamage(damage);
                    }
                }

            }
        }

    }
}