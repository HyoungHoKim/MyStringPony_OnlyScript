using Valve.VR;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;

// 멤버 변수들
public partial class Base_Laser_Pointer : MonoBehaviour
{
    protected SteamVR_Behaviour_Pose pose;
    protected SteamVR_Input_Sources hand;

    [SerializeField] protected bool bool_is_left_hand;
    protected SteamVR_Input_Sources left_hand;
    protected SteamVR_Input_Sources right_hand;

    protected SteamVR_Action_Vibration haptic;

    protected LineRenderer line_renderer;

    protected SteamVR_Action_Boolean trigger;
    protected float float_max_distance;

    protected Color color_yellow;
    protected Color color_clicked_red;

    protected RaycastHit hit;
    protected Transform transform_this;

    protected GameObject game_object_prev;
    protected GameObject game_object_current;

    protected SteamVR_Action_Boolean menuAction;
}

// 유니티 함수들
public partial class Base_Laser_Pointer : MonoBehaviour
{
    protected void Start()
    {
        transform_this = GetComponent<Transform>();

        trigger = SteamVR_Actions.default_InteractUI;
        menuAction = SteamVR_Actions.default_MenuButton;

        float_max_distance = 30.0f;

        color_yellow = Color.yellow;
        color_clicked_red = Color.red;

        //pose = GetComponent<SteamVR_Behaviour_Pose>();
        //hand = pose.inputSource;

        left_hand = SteamVR_Input_Sources.LeftHand;
        right_hand = SteamVR_Input_Sources.RightHand;
        haptic = SteamVR_Actions.default_Haptic;

        Create_Line_Renderer();
    }

    protected void Update()
    {
        var current = new PointerEventData(EventSystem.current);

        if (Physics.Raycast(transform_this.position, transform_this.forward, out hit, float_max_distance))
        {
            line_renderer.SetPosition(1, new Vector3(0, 0, hit.distance));

            game_object_current = hit.collider.gameObject;

            if (game_object_current != game_object_prev)
            {
                ExecuteEvents.Execute(game_object_current, current, ExecuteEvents.pointerEnterHandler);

                ExecuteEvents.Execute(game_object_prev, current, ExecuteEvents.pointerExitHandler);

                game_object_prev = game_object_current;
            }

            if (bool_is_left_hand && trigger.GetStateDown(left_hand))
            {
                ExecuteEvents.Execute(game_object_current, current, ExecuteEvents.pointerClickHandler);
                Excute_Action();
            }
            else if (false == bool_is_left_hand && trigger.GetStateDown(right_hand))
            {
                ExecuteEvents.Execute(game_object_current, current, ExecuteEvents.pointerClickHandler);
                Excute_Action();
            }
        }
        else
        {
            if (game_object_prev != null)
            {
                ExecuteEvents.Execute(game_object_prev, current, ExecuteEvents.pointerExitHandler);
                game_object_prev = null;
            }
        }

        if (bool_is_left_hand && trigger.GetStateDown(left_hand))
        {
            Excute_Action_Trigger();
        }
        else if (false == bool_is_left_hand && trigger.GetStateDown(right_hand))
        {
            Excute_Action_Trigger();
        }

        // Menu 버튼
        if (bool_is_left_hand && menuAction.GetStateDown(left_hand))
        {
            Excute_Menu_Action();
        }
        else if (false == bool_is_left_hand && menuAction.GetStateDown(right_hand))
        {
            Excute_Menu_Action();
        }
    }
}

// 함수들
public partial class Base_Laser_Pointer : MonoBehaviour
{
    protected void Create_Line_Renderer()
    {
        line_renderer = this.gameObject.AddComponent<LineRenderer>();
        line_renderer.useWorldSpace = false;
        line_renderer.receiveShadows = false;

        line_renderer.positionCount = 2;
        line_renderer.SetPosition(0, Vector3.zero);
        line_renderer.SetPosition(1, new Vector3(0, 0, float_max_distance));

        line_renderer.startWidth = 0.03f;
        line_renderer.endWidth = 0.005f;

        line_renderer.material = new Material(Shader.Find("Unlit/Color"));
        line_renderer.material.color = this.color_yellow;
    }

    public void Haptic()
    {
        if (bool_is_left_hand)
        {
            haptic.Execute(0.2f, 0.2f, 50.0f, 0.5f, left_hand);
        }
        else
        {
            haptic.Execute(0.2f, 0.2f, 50.0f, 0.5f, left_hand);
        }
    }
}

public partial class Base_Laser_Pointer : MonoBehaviour
{
    protected virtual void Excute_Action()
    {

    }

    protected virtual void Excute_Action_Trigger()
    {

    }

    protected virtual void Excute_Menu_Action()
    {

    }
}