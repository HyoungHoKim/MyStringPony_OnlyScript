using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Hit_Test_Scene_Laser_Pointer : Base_Laser_Pointer
{
    [SerializeField] GameObject gameobjectCanvasMenu;
    [SerializeField] GameObject gameobjectCubeSword;

    protected override void Excute_Action()
    {
        
    }

    protected override void Excute_Menu_Action()
    {
        gameobjectCubeSword.SetActive(false);
        gameobjectCanvasMenu.SetActive(true);
    }

    public void OnButtonResume()
    {
        // Time.timeScale 사용하면 페이드 처리가 되지 않는다
        gameobjectCubeSword.SetActive(true);
        gameobjectCanvasMenu.SetActive(false);
    }

    public void OnButtonRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void OnButtonToMain()
    {
        // fade 후 씬 이동
        Y.Loading_Scene.stringNextScene = "Menu_Scene";
        var fade = GameObject.FindObjectOfType<Hit_Test_Scene_Fade>();
        
        
        fade.Start_Fade();
    }

    public void OnButtonExit()
    {
        Application.Quit();
    }
}