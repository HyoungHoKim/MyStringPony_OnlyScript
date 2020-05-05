
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Scene_Laser_Pointer : Base_Laser_Pointer
{
    public void Move_Tutorial_Scene()
    {
        //MoveScene("Tutorial_Scene");
        MoveScene("Enemy_Test");
    }

    public void Move_Game_Scene()
    {
        MoveScene("Lab_CheolWoo");
        //MoveScene("Move_Test_Scene");
    }

    public void Menu_Text_Scene()
    {
        MoveScene("Hit_Test_Scene");
    }

    public void Application_Quit()
    {
        Application.Quit();
    }

    void MoveScene(string sceneName)
    {
        Y.Loading_Scene.stringNextScene = sceneName;

        var menu_fade = GameObject.FindObjectOfType<Menu_Fade>();
        menu_fade.Start_Fade();
    }
}