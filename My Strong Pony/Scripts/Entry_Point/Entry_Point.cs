using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class Entry_Point : MonoBehaviour
{
    IEnumerator Start()
    {
        Application.targetFrameRate = 90;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        yield return new UnityEngine.WaitForSeconds(0.01f);

        SceneManager.LoadScene("Menu_Scene");
    }
}