using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu_Fade : Base_Fade
{
    protected override void Excute_Action()
    {
        SceneManager.LoadScene("Loading_Scene");
        //SceneManager.LoadScene("Game_Scene");
    }
}