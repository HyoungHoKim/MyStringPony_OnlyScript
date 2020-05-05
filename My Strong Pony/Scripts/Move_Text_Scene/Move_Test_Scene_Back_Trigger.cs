using Sym = Sym4D.Sym4DEmulator;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Y
{
    public class Move_Test_Scene_Back_Trigger : MonoBehaviour
    {
        bool boolReinsLeft;
        bool boolReinsRight;
        Move_Test_Scene move_test_scene;

        readonly int intSym4DRoll = 100;

        void Awake()
        {
            move_test_scene = GameObject.FindObjectOfType<Move_Test_Scene>();
        }

        void OnTriggerEnter(Collider other)
        {
            if (false == move_test_scene.CanMoveOrTurn() || false == move_test_scene.isMove)
            {
                return;
            }

            if (other.CompareTag("REINS_LEFT"))
            {
                boolReinsLeft = true;
            }

            if (other.CompareTag("REINS_RIGHT"))
            {
                boolReinsRight = true;
            }
        }

        void OnTriggerStay(Collider other)
        {
            if (false == move_test_scene.CanMoveOrTurn() || false == move_test_scene.isMove)
            {
                return;
            }

            bool isStop = boolReinsLeft && boolReinsRight;

            if (isStop)
            {
                move_test_scene.SpeedDown();

                //move_test_scene.SetSym4DRoll(0);
                //move_test_scene.SetSym4DPitch(0);
                //move_test_scene.Sym4DMosion();
            }
            else if (boolReinsLeft)
            {
                move_test_scene.Turn(true);

                move_test_scene.SetSym4DRoll(intSym4DRoll);
                move_test_scene.Sym4DMosion();
            }
            else if (boolReinsRight)
            {
                move_test_scene.Turn(false);

                move_test_scene.SetSym4DRoll(intSym4DRoll * -1);
                move_test_scene.Sym4DMosion();
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (false == move_test_scene.CanMoveOrTurn() || false == move_test_scene.isMove)
            {
                return;
            }

            if (other.CompareTag("REINS_LEFT"))
            {
                move_test_scene.SetSym4DRoll(0);
                boolReinsLeft = false;
            }

            if (other.CompareTag("REINS_RIGHT"))
            {
                move_test_scene.SetSym4DRoll(0);
                boolReinsRight = false;
            }

            //if (boolReinsLeft && boolReinsRight)
            //{
            //    move_test_scene.SetSym4DRoll(0);
            //    move_test_scene.SetSym4DPitch(0);
            //    move_test_scene.Sym4DMosion();
            //}
        }

    }
}