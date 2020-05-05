using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Y
{
    public class Move_Test_Scene_Front_Trigger : MonoBehaviour
    {
        bool boolReinsLeft;
        bool boolReinsRight;
        Move_Test_Scene move_text_scene;        

        void Awake()
        {
            move_text_scene = GameObject.FindObjectOfType<Move_Test_Scene>();
        }

        void InitBoolReins()
        {
            boolReinsLeft = false;
            boolReinsRight = false;
        }

        void OnTriggerEnter(Collider other)
        {
            if (false == move_text_scene.CanMoveOrTurn() || false == move_text_scene.isMove)
            {
                return;
            }

            bool ReinsLeft = other.CompareTag("REINS_LEFT");
            bool ReinsRight = other.CompareTag("REINS_RIGHT");

            if (ReinsLeft)
            {
                boolReinsLeft = true;
            }

            if (ReinsRight)
            {
                boolReinsRight = true;
            }

            if (boolReinsLeft && boolReinsRight)
            {
                InitBoolReins();
                move_text_scene.SpeedUp();
            }
        }

        //void OnTriggerExit(Collider other)
        //{
        //    if (false == move_text_scene.CanMoveOrTurn())
        //    {
        //        InitBoolReins();
        //        return;
        //    }

        //    bool reins = other.CompareTag("REINS_LEFT") || other.CompareTag("REINS_RIGHT");

        //    if (reins)
        //    {
        //        bool isSpeedUp = boolReinsLeft && boolReinsRight;
        //        if (isSpeedUp)
        //        {
        //            move_text_scene.SpeedUp();
        //            InitBoolReins();
        //        }
        //    }
        //}
    }
}