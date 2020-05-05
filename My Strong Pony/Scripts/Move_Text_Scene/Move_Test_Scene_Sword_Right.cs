using Valve.VR;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Y
{
    public class Move_Test_Scene_Sword_Right : MonoBehaviour
    {
        //[SerializeField] SteamVR_Behaviour_Pose controllerPose;

        Move_Test_Scene move_test_scene;
        Transform transform_move_test_scene;

        string stringPlayerSelfTeamTagName;

        void Start()
        {
            move_test_scene = GetComponentInParent<Move_Test_Scene>();
            stringPlayerSelfTeamTagName = move_test_scene.gameObject.tag;
            transform_move_test_scene = move_test_scene.transform;
        }

        void OnTriggerEnter(Collider other)
        {
            bool isEnemyBody = other.CompareTag("ENEMY_BODY");
            
            if (isEnemyBody)
            {
                var enemy = other.gameObject.GetComponentInParent<Enemy>();
                string enemyTagName = enemy.GetTag();

                bool isPlayerRed = stringPlayerSelfTeamTagName.Contains("RED");
                bool isEnemyRed = enemyTagName.Contains("RED");

                if (isPlayerRed && isEnemyRed)
                {
                    return;
                }

                if (false == isPlayerRed && false == isEnemyRed)
                {
                    return;
                }

                if (CheckHit(other))
                {
                    float damage = Y.Variables.GetDamage(Y.Variables.stringPlayer, enemyTagName);
                    enemy.HitDamage(damage);
                    move_test_scene.HapticRight();
                }

                return;
            }

            // 플레이어인 경우
            var swordRight = other.GetComponentInChildren<Move_Test_Scene_Sword_Right>();
            if (swordRight != null)
            {
                var player = other.gameObject.GetComponentInParent<Move_Test_Scene>();
                string otherPlayerTeamTag = player.gameObject.tag;

                if (stringPlayerSelfTeamTagName.Contains("RED") && otherPlayerTeamTag.Contains("RED"))
                {
                    return;
                }

                if (stringPlayerSelfTeamTagName.Contains("BLUE") && otherPlayerTeamTag.Contains("BLUE"))
                {
                    return;
                }

                if (CheckHit(other))
                {
                    float damage = Y.Variables.GetDamage(Y.Variables.stringPlayer, Y.Variables.stringPlayer);
                    player.HitDamage(damage);
                }
            }
        }

        bool CheckHit(Collider other)
        {
            // 수치 판정
            //float magnitude = controllerPose.GetVelocity().magnitude;
            //if (false == float.IsNaN(magnitude))
            //print($"attack velocity = {controllerPose.GetVelocity().magnitude}");

            var enemyPosition = other.transform.position;
            Vector3 avoidDirection = transform_move_test_scene.InverseTransformPoint(enemyPosition);

            //if (avoidDirection.x <= 0.0f && magnitude >= 0.5f)
            if (avoidDirection.x <= 0.0f)    
            {
                return true;
            }

            return false;
        }
    }
}