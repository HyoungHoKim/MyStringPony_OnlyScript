using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Hit_Test_Scene_Fade : Base_Fade
{
    protected override void Excute_Action()
    {
        SceneManager.LoadScene("Loading_Scene");
    }
}