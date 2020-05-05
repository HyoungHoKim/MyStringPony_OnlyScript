using Sym = Sym4D.Sym4DEmulator;

using TMPro;
using Valve.VR;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Y
{
    public class Sym4_Test_Scene : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        [SerializeField] protected bool bool_is_left_hand;
        SteamVR_Input_Sources left_hand;
        SteamVR_Input_Sources right_hand;

        SteamVR_Action_Boolean trigger;
        SteamVR_Action_Boolean menuAction;

        public int xPort; //좌석장비의 통신 포트
        public int wPort; //바람장비의 통신 포트

        readonly WaitForSeconds ws = new WaitForSeconds(1.5f);

        void OnDestroy()
        {
            Sym.Sym4D_X_EndContents();
            Sym.Sym4D_W_EndContents();
        }

        void Awake()
        {
            // VR
            trigger = SteamVR_Actions.default_InteractUI;
            menuAction = SteamVR_Actions.default_MenuButton;
            left_hand = SteamVR_Input_Sources.LeftHand;
            right_hand = SteamVR_Input_Sources.RightHand;


            //포트번호 설정
            xPort = Sym.Sym4D_X_Find();
            wPort = Sym.Sym4D_W_Find();

            //Sym4D-X100 COM Port Open  및 컨텐츠 시작을 장치에 전달
            Sym.Sym4D_X_StartContents(xPort);
            Sym.Sym4D_W_StartContents(wPort);

            //Roll, Pitch 최대허용 각도(0 ~ 100) : 0도 ~ 10도
            Sym.Sym4D_X_SetConfig(100, 100);
            Sym.Sym4D_W_SetConfig(100);            
        }

        void Update()
        {
            // Menu 버튼
            if (bool_is_left_hand && menuAction.GetStateDown(left_hand))
            {
                text.text = "Sym.Sym4D_X_SendMosionData(0, 0);";
                Sym.Sym4D_X_SendMosionData(0, 0);
                Sym.Sym4D_W_SendMosionData(0);

            }
            else if (false == bool_is_left_hand && menuAction.GetStateDown(right_hand))
            {
                text.text = "Sym.Sym4D_X_SendMosionData(100, 0);";
                Sym.Sym4D_X_SendMosionData(100, 0);
                Sym.Sym4D_W_SendMosionData(50);
            }

            if (bool_is_left_hand && trigger.GetStateDown(left_hand))
            {
                text.text = "Sym.Sym4D_X_SendMosionData(0, 100);";
                Sym.Sym4D_X_SendMosionData(0, 100);
                Sym.Sym4D_W_SendMosionData(100);
            }
            else if (false == bool_is_left_hand && trigger.GetStateDown(right_hand))
            {
                text.text = "Sym.Sym4D_X_SendMosionData(-0, -100);";
                Sym.Sym4D_X_SendMosionData(0, -100);
                Sym.Sym4D_W_SendMosionData(75);
            }
        }






        public void OnInit()
        {
            StartCoroutine(Init());
        }

        //포트 번호 초기화 및 기본 테스트
        IEnumerator Init()
        {
            //print("Init");
            ////포트번호 설정
            //xPort = Sym.Sym4D_X_Find();
            //wPort = Sym.Sym4D_W_Find();

            //yield return ws;
            //Debug.Log($"X Port={xPort} W Port={wPort}");

            ////Sym4D-X100 COM Port Open  및 컨텐츠 시작을 장치에 전달
            //Sym.Sym4D_X_StartContents(xPort);
            //yield return ws;

            // 선생님이 안된다고 말씀하셨음.
            //Sym.Sym4D_X_New_SendVibration(100);
            //yield return ws;

            ////Roll, Pitch 최대허용 각도(0 ~ 100) : 0도 ~ 10도
            //Sym.Sym4D_X_SetConfig(100, 100);
            //yield return ws;

            //Sym4D-X100에 모션 데이터 전달(-100 ~ +100)
            Sym.Sym4D_X_SendMosionData(0, 0);
            yield return ws;

            Sym.Sym4D_X_SendMosionData(100, 100);
            yield return ws;

            Sym.Sym4D_X_SendMosionData(-100, -100);
            yield return ws;

            Sym.Sym4D_X_SendMosionData(100, -100);
            yield return ws;

            Sym.Sym4D_X_SendMosionData(-100, 100);
            yield return ws;

            Sym.Sym4D_X_SendMosionData(0, 0);
            yield return ws;

            Sym.Sym4D_X_EndContents();
            yield return ws;


            //Wind 테스트
            Sym.Sym4D_W_StartContents(wPort);
            yield return ws;

            Sym.Sym4D_W_SendMosionData(0);
            yield return new WaitForSeconds(1);

            Sym.Sym4D_W_SendMosionData(50);
            yield return new WaitForSeconds(5);

            Sym.Sym4D_W_SendMosionData(100);
            yield return new WaitForSeconds(5);

            Sym.Sym4D_W_SendMosionData(50);
            yield return new WaitForSeconds(5);

            Sym.Sym4D_W_SendMosionData(0);
            yield return ws;

            Sym.Sym4D_W_EndContents();
        }

        float prevJoyX, prevJoyY;
        float currJoyX, currJoyY;
        bool isFinished = false;

        //void Update()
        //{
        //    currJoyX = Input.GetAxis("Horizontal");
        //    currJoyY = Input.GetAxis("Vertical");
        //    Debug.Log($"joyX={currJoyX} / joyY={currJoyY}");
        //    if (currJoyX != prevJoyX)
        //    {
        //        //Change Roll
        //        prevJoyX = currJoyX;
        //        StartCoroutine(ChangeRollNPitch());
        //    }

        //    if (currJoyY != prevJoyY)
        //    {
        //        //Change Pitch
        //        prevJoyY = currJoyY;
        //        StartCoroutine(ChangeRollNPitch());
        //    }
        //}

        IEnumerator ChangeRollNPitch()
        {
            yield return new WaitForSeconds(0.1f);

            //Sym4D-X100 COM Port Open  및 컨텐츠 시작을 장치에 전달
            Sym.Sym4D_X_StartContents(xPort);
            yield return new WaitForSeconds(0.1f);

            Sym.Sym4D_X_SendMosionData((int) -currJoyX * 100, (int) currJoyY * 100);


            yield return new WaitForSeconds(0.1f);
        }
    }
}