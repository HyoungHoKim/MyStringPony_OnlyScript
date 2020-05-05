using Valve.VR;
using UnityEngine.UI;
using Sym = Sym4D.Sym4DEmulator;

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace Y
{
    public partial class Move_Test_Scene : MonoBehaviour
    {
        //(철우) 알아보기 쉽게 주석 추가
        //플레이어 손, 말 애니메이터 컨트롤러
        readonly int intAnimatorIsGrab = Animator.StringToHash("Is_Grab");
        readonly int intAnimatorHashMoveF = Animator.StringToHash("MoveF");

        //플레이어 동작시 사운드
        AudioSource audioSource;        
        [SerializeField] AudioClip[ ] arrayAudioClipSword;
        [SerializeField] AudioClip audioClipRun;
        [SerializeField] AudioClip audioClipStop;

        Animator animatorLeftHand;
        Animator animatorRightHand;
        Animator animatorHorse;

        //햅틱
        SteamVR_Action_Vibration haptic;

        //트리거와 메뉴 버튼
        SteamVR_Action_Boolean trigger;
        SteamVR_Action_Boolean menuAction;

        //바이브 스틱 인식
        SteamVR_Input_Sources leftHand;
        SteamVR_Input_Sources rightHand;

        //왼손, 오른손 Velocity 판별 
        // None이면, [CameraRig]/Controller (left) 연결하세요
        [SerializeField] SteamVR_Behaviour_Pose controllerPoseLeft;
        // None이면, [CameraRig]/Controller (right) 연결하세요
        [SerializeField] SteamVR_Behaviour_Pose controllerPoseRight;

        bool boolTriggerLeft;
        bool boolTriggerRight;


        //방패와 검
        //방패는 주석처리
        //[SerializeField] GameObject gameObjectShieldLeft;
        [SerializeField] GameObject gameObjectSwordRight;

        // 이동 관련
        bool boolIsMove = true;
        float floatHorseSpeed = 0.0f;
        Rigidbody rigidbodyPlayer;
        [SerializeField] GameObject gameObjectCubeFrontCollider1;
        [SerializeField] GameObject gameObjectCubeBackCollider1;

        //float floatInputVertical;

        // 메뉴 선택 관련
        Transform transformCameraEye;

        //메뉴 레이캐스트
        Ray ray;
        RaycastHit raycastHit;

        readonly float floatMaxDistance = 10.0f;
        readonly int intLayerMask = 1 << 10;

        GameObject gameObjectPrevButton;
        GameObject gameObjectCurrentButton;

        //메뉴 바라볼 때 원모양 차는 효과
        Image imageFill;
        readonly float floatMaxFillTime = 1.5f;
        float floatPassedFillTime = 0.0f;

        bool boolIsClickedFill = false;

        //메뉴 캔버스
        GameObject gameObjectCanvasMenu;
        [SerializeField] GameObject gameObjectCanvasDeath;

        // Sym4D 관련
        int intSym4DPortX;
        int intSym4DPortW;

        int intSym4DRoll = 0;
        int intSym4DPitch = 0;

        float floatTimer = 0;

        // HP
        float floatHp = 100.0f;
    }

    public partial class Move_Test_Scene : MonoBehaviour
    {
        void OnDestroy()
        {
            Sym.Sym4D_X_EndContents();
            Sym.Sym4D_W_EndContents();
        }

        void Awake()
        {
            //포트번호 설정
            intSym4DPortX = Sym.Sym4D_X_Find();
            intSym4DPortW = Sym.Sym4D_W_Find();

            //Sym4D-X100 COM Port Open  및 컨텐츠 시작을 장치에 전달
            Sym.Sym4D_X_StartContents(intSym4DPortX);
            Sym.Sym4D_W_StartContents(intSym4DPortW);

            //Roll, Pitch 최대허용 각도(0 ~ 100) : 0도 ~ 10도
            Sym.Sym4D_X_SetConfig(100, 100);
            Sym.Sym4D_W_SetConfig(100);
        }

        void Start()
        {
            Init();
        }

        void OnEnable()
        {
            if (rigidbodyPlayer != null)
            {
                rigidbodyPlayer.isKinematic = false;
            }

            //floatInputVertical = 0f;
        }

        void OnDisable()
        {
            rigidbodyPlayer.isKinematic = true;
        }

        void Update()
        {
            ClickTrigger();
            ClickMenuButton();
            //InputValue();
            InputRaycast();
        }

        void FixedUpdate()
        {
            if (boolIsMove)
            {
                Move();
            }
        }
    }

    public partial class Move_Test_Scene : MonoBehaviour
    {
        void Init()
        {
            //오디오소스
            audioSource = GetComponent<AudioSource>();


            //손과 말 애니메이터 컨트롤러
            animatorLeftHand = GameObject.Find("Medieval_LHands").GetComponent<Animator>();
            animatorRightHand = GameObject.Find("Medieval_RHands").GetComponent<Animator>();
            animatorHorse = transform.Find("AdamKnight_Horse").GetComponent<Animator>();

            //바이브 스틱 관련
            haptic = SteamVR_Actions.default_Haptic;
            trigger = SteamVR_Actions.default_InteractUI;
            menuAction = SteamVR_Actions.default_MenuButton;

            leftHand = SteamVR_Input_Sources.LeftHand;
            rightHand = SteamVR_Input_Sources.RightHand;

            //(철우) 방패 안써서 주석처리합니다
            //gameObjectShieldLeft.SetActive(false);
            gameObjectSwordRight.SetActive(false);

            // 이동 관련
            rigidbodyPlayer = GetComponent<Rigidbody>();

            // 메뉴 선택 관련
            transformCameraEye = GameObject.Find("Camera (eye)").transform;
            gameObjectCanvasMenu = transform.Find("[CameraRig]/Canvas/Menu").gameObject;
            //gameObjectCanvasDeath = transform.Find("[CameraRig]/Camera (head)/HeadCanvas/Dead").gameObject;


            //5초 뒤 말 고삐 콜라이더 활성화
            Invoke("ActiveMoveCollider", 5.0f);
        }

        //말 고삐 콜라이더 활성화
        public void ActiveMoveCollider()
        {
            gameObjectCubeBackCollider1.SetActive(true);
            gameObjectCubeFrontCollider1.SetActive(true);
        }

        //말 고삐 콜라이더 비활성화
        public void HideMoveCollider()
        {
            gameObjectCubeBackCollider1.SetActive(false);
            gameObjectCubeFrontCollider1.SetActive(false);
        }

        //(철우) 상황에 따라 햅틱 패턴 나눕니다
        public void HapticRight()
        {
            haptic.Execute(0.2f, 0.2f, 50.0f, 0.5f, rightHand);
        }

        public void HapticLeft()
        {
            haptic.Execute(0.2f, 0.2f, 50.0f, 0.5f, leftHand);
        }


        //(철우) 왼쪽 방패 안써서 주석처리 합니다
        /*void GrabLeft(bool isGrab)
        {
            boolTriggerLeft = isGrab;
            animatorLeftHand.SetBool(intAnimatorIsGrab, isGrab);
            gameObjectShieldLeft.SetActive(isGrab);
        }*/

        void GrabRight(bool isGrab)
        {
            boolTriggerRight = isGrab;
            animatorRightHand.SetBool(intAnimatorIsGrab, isGrab);
            gameObjectSwordRight.SetActive(isGrab);
        }

        void ClickTrigger()
        {
            if (trigger.GetStateDown(rightHand))
            {
                HapticRight();
                GrabRight(true);

                //(철우) 무기 뽑을 때 사운드 하나만 쓸겁니다
                //int index = Random.Range(0, arrayAudioClipSword.Length);
                PlayAudioSource(arrayAudioClipSword[1]);
            }

            if (trigger.GetStateUp(rightHand))
            {
                GrabRight(false);
            }

            //(철우) 왼쪽 방패 안써서 주석처리
            /*if (trigger.GetStateDown(leftHand))
            {
                HapticLeft();
                GrabLeft(true);

                int index = Random.Range(0, arrayAudioClipSword.Length);
                PlayAudioSource(arrayAudioClipSword[index]);
            }*/

            /*if (trigger.GetStateUp(leftHand))
            {
                GrabLeft(false);
            }*/
        }

        void ClickMenuButton()
        {
            if (menuAction.GetStateDown(leftHand) || menuAction.GetStateDown(rightHand))
            {
                gameObjectCanvasMenu.SetActive(true);

                InitPlayer();
                IsMove(false);
            }
        }

        //void InputValue()
        //{
        //    floatInputVertical = Input.GetAxis(Y.Variables.stringVertical);
        //}


        //말 이동 로직
        void Move()
        {
            Vector3 movement = transform.forward * floatHorseSpeed * Time.deltaTime;
            movement.y -= 2.0f * Time.deltaTime;
            rigidbodyPlayer.MovePosition(rigidbodyPlayer.position + movement);
            animatorHorse.SetBool(intAnimatorHashMoveF, floatHorseSpeed > 0.0f ? true : false);

            // 이동시 Sym4D 동작
            floatTimer += Time.fixedDeltaTime;

            if (floatTimer >= 0.5f)
            {
                floatTimer = 0.0f;
                if (floatHorseSpeed > 0.0f)
                {
                    StopCoroutine(Sym4DFrontBack());
                    StartCoroutine(Sym4DFrontBack());
                }
                else
                {
                    StopCoroutine(Sym4DFrontBack());
                    StopCoroutine(Sym4DStop());
                    StartCoroutine(Sym4DStop());
                }
            }
        }


        //Sym4D 코루틴
        IEnumerator Sym4DFrontBack()
        {
            intSym4DPitch = 100;
            Sym4DMosion();
            yield return Y.Variables.waitForSeconds_200ms;
            intSym4DPitch = -100;
            Sym4DMosion();
            yield return Y.Variables.waitForSeconds_200ms;
            Sym.Sym4D_W_SendMosionData(0);
        }

        IEnumerator Sym4DStop()
        {
            yield return Y.Variables.waitForSeconds_200ms;
            intSym4DRoll = 0;
            intSym4DPitch = 0;
            Sym4DMosion();
        }

        //Sym4D 관련 로직
        public void Sym4DMosion()
        {
            Sym.Sym4D_W_SendMosionData(100);
            Sym.Sym4D_X_SendMosionData(intSym4DRoll, intSym4DPitch);
        }

        public void SetSym4DPitch(int pitch)
        {
            intSym4DPitch = pitch;
        }

        public void SetSym4DRoll(int roll)
        {
            intSym4DRoll = roll;
        }

        public void Death()
        {
            gameObjectCanvasDeath.SetActive(true);

            InitPlayer();
            IsMove(false);
        }

        public void InitPlayer()
        {
            rigidbodyPlayer.MovePosition(rigidbodyPlayer.position + Vector3.zero);

            intSym4DRoll = 0;
            intSym4DPitch = 0;

            Sym.Sym4D_W_SendMosionData(0);
            Sym.Sym4D_X_SendMosionData(0, 0);
        }


        //이동 조작 관련 로직

        public void Turn(bool isLeft)
        {
            int turnHorizontal = isLeft ? -1 : 1;

            float turn = turnHorizontal * Y.Variables.floatHorseTurnSpeed * Time.deltaTime;
            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0);
            rigidbodyPlayer.MoveRotation(rigidbodyPlayer.rotation * turnRotation);

            if (isLeft)
            {
                HapticLeft();
            }
            else
            {
                HapticRight();
            }
        }


        //속도 상승
        public void SpeedUp()
        {
            // Velocity 값으로 스피드 업 실행 분기
            float magnitudeLeft = controllerPoseLeft.GetVelocity().magnitude;
            float magnitudeRight = controllerPoseRight.GetVelocity().magnitude;

            bool doSpeedUp = magnitudeLeft >= 0.5f && magnitudeRight >= 0.5f;

            if (doSpeedUp)
            {
                PlayAudioSource(audioClipRun);

                floatHorseSpeed = Mathf.Clamp(floatHorseSpeed + 10.0f, 0.0f, Y.Variables.floatMaxHorseSpeed);
                HapticLeft();
                HapticRight();
            }
        }


        //속도 하강
        public void SpeedDown()
        {
            PlayAudioSource(audioClipStop);

            floatHorseSpeed = Mathf.Clamp(floatHorseSpeed - 0.5f, 0.0f, Y.Variables.floatMaxHorseSpeed);
            HapticLeft();
            HapticRight();
        }


        //고삐 콜라이더 체크
        public bool CanMoveOrTurn()
        {
            bool checkLeft = (false == boolTriggerLeft);
            bool checkRight = (false == boolTriggerRight);
            return checkLeft && checkRight;
        }






        //바라보는 레이캐스트
        void InputRaycast()
        {
            if (false == gameObjectCanvasMenu.activeSelf)
            {
                return;
            }

            ray = new Ray(transformCameraEye.position, transformCameraEye.forward);
            //Debug.DrawRay(ray.origin, ray.direction * floatMaxDistance, Color.green);

            if (Physics.Raycast(ray, out raycastHit, floatMaxDistance, intLayerMask))
            {
                GazeButton();
            }
            else
            {
                ReleaseButton();
            }
        }


        //버튼 바라봤을 때 시계방향 채워지는 효과
        void GazeButton()
        {
            PointerEventData data = new PointerEventData(EventSystem.current);

            if (raycastHit.collider.gameObject.layer == 10)
            {
                gameObjectCurrentButton = raycastHit.collider.gameObject;

                imageFill = gameObjectCurrentButton.transform.Find("Image").GetComponent<Image>();

                if (gameObjectCurrentButton != gameObjectPrevButton)
                {
                    InitImageFill();

                    ExecuteEvents.Execute(gameObjectCurrentButton, data, ExecuteEvents.pointerEnterHandler);
                    ExecuteEvents.Execute(gameObjectPrevButton, data, ExecuteEvents.pointerExitHandler);

                    gameObjectPrevButton = gameObjectCurrentButton;
                }
                else
                {
                    if (boolIsClickedFill)
                    {
                        return;
                    }

                    floatPassedFillTime += Time.deltaTime;

                    imageFill.fillAmount = floatPassedFillTime / floatMaxFillTime;

                    if (floatPassedFillTime >= floatMaxFillTime)
                    {
                        ExecuteEvents.Execute(gameObjectCurrentButton, data, ExecuteEvents.pointerClickHandler);
                        boolIsClickedFill = true;
                    }
                }
            }
            else
            {
                ReleaseButton();
            }
        }


        //버튼 바라보지 않을 때
        void ReleaseButton()
        {
            InitImageFill();

            PointerEventData data = new PointerEventData(EventSystem.current);

            if (gameObjectPrevButton != null)
            {
                ExecuteEvents.Execute(gameObjectPrevButton, data, ExecuteEvents.pointerExitHandler);
                gameObjectPrevButton = null;
            }
        }


        //원형 시계방향 채워지기
        void InitImageFill()
        {
            floatPassedFillTime = 0.0f;

            boolIsClickedFill = false;

            if (imageFill != null)
            {
                imageFill.fillAmount = 0.0f;
            }

            if (gameObjectPrevButton != null)
            {
                gameObjectPrevButton.transform.Find("Image").GetComponent<Image>().fillAmount = 0.0f;
            }
        }



        //메뉴 클릭시 동작 로직
        public void MenuResume()
        {
            gameObjectCanvasMenu.SetActive(false);

            InitPlayer();
            IsMove(true);
        }

        public void MenuRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void MenuToMain()
        {
            Loading_Scene.stringNextScene = "Menu_Scene";
            SceneManager.LoadScene("Loading_Scene");
        }

        public void MenuExit()
        {
            Application.Quit();
        }



        //오디오소스 실행관련
        public void PlayAudioSource(AudioClip clip)
        {
            bool isSameClip = (audioSource.clip == clip);
            bool canPlaying = (false == audioSource.isPlaying);

            if (isSameClip && canPlaying)
            {
                audioSource.Play();
            }
            else if (false == isSameClip)
            {
                audioSource.clip = clip;
                audioSource.Play();
            }
        }


        


        //메뉴 실행할 때, 플레이어 죽었을 때 움직임과 Sym4D 정지를 위한 로직
        public void IsMove(bool enable)
        {
            boolIsMove = enable;
            floatHorseSpeed = 0.0f;

            gameObjectCubeBackCollider1.SetActive(enable);
            gameObjectCubeFrontCollider1.SetActive(enable);

            animatorHorse.SetBool(intAnimatorHashMoveF, false);

            rigidbodyPlayer.isKinematic = !enable;
            GetComponent<CapsuleCollider>().isTrigger = !enable;
        }



        //피격시 데미지 계산
        public void HitDamage(float damage)
        {
            if (floatHp > 0)
            {
                floatHp -= damage;
                //Debug.Log($"사용자 데미지 받아 남은 체력:{floatHp}");

                if (floatHp <= 0)
                {
                    //Debug.Log($"사용자 죽음 체력:{floatHp}");
                    Death();
                }
            }
        }

    }
}