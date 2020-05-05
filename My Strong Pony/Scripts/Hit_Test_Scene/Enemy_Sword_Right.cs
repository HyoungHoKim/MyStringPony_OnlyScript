using Y;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy_Sword_Right : MonoBehaviour
{
    Enemy enemySelf;
    string stringEnemySelfTagName;
    bool boolIsEnemySelfRed;

    void Start()
    {
        
    }
    private void OnEnable()
    {
        enemySelf = GetComponentInParent<Enemy>();
        stringEnemySelfTagName = enemySelf.gameObject.tag;
        boolIsEnemySelfRed = stringEnemySelfTagName.Contains("RED");
    }
    void OnTriggerEnter(Collider other)
    {
        bool isEnemyBody = other.CompareTag("ENEMY_BODY");
        bool isPlayerBip01Spine = other.CompareTag("Player");

        if (isEnemyBody)
        {
            // 사용자가 아닐 경우
            var hitEnemy = other.gameObject.GetComponentInParent<Enemy>();
            if (hitEnemy != null)
            {
                string hitEnemyTagName = hitEnemy.gameObject.tag;

                bool isHitEnemyRed = hitEnemyTagName.Contains("RED");

                if (boolIsEnemySelfRed && isHitEnemyRed)
                {
                    return;
                }

                if (false == boolIsEnemySelfRed && false == isHitEnemyRed)
                {
                    return;
                }

                float damage = Y.Variables.GetDamage(stringEnemySelfTagName, hitEnemyTagName);
                hitEnemy.HitDamage(damage);
                return;
            }
        }

        if (isPlayerBip01Spine)
        {
            // 사용자인 경우
            var hitPlayer = other.gameObject.GetComponentInParent<Move_Test_Scene>();
            if (hitPlayer != null)
            {
                string hitPlayerTagName = hitPlayer.gameObject.tag;

                bool isHitPlayerRed = hitPlayerTagName.Contains("RED");

                if (boolIsEnemySelfRed && isHitPlayerRed)
                {
                    return;
                }

                if (false == boolIsEnemySelfRed && false == isHitPlayerRed)
                {
                    return;
                }

                float damage = Y.Variables.GetDamage(stringEnemySelfTagName, Y.Variables.stringPlayer);

                hitPlayer.HitDamage(damage);
                return;
            }
        }        
    }
}