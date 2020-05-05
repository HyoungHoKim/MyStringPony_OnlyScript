using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Base_Fade : MonoBehaviour
{
    protected Image image_fade;

    protected void Awake()
    {
        image_fade = transform.Find("Image_Fade").GetComponent<Image>();
    }

    protected IEnumerator Fade()
    {
        WaitForSeconds wait_100 = new WaitForSeconds(0.1f);

        for (float f = 0.0f; f <= 1.0f; f += 0.1f)
        {
            Color c = image_fade.color;
            c.a = f;
            image_fade.color = c;
            yield return wait_100;
        }

        yield return wait_100;
        
        Excute_Action();
    }

    protected virtual void Excute_Action()
    {

    }

    public void Start_Fade()
    {
        StartCoroutine(Fade());
    }
}