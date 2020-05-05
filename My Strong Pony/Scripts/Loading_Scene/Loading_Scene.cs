using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace Y
{
    public class Loading_Scene : MonoBehaviour
    {
        public static string stringNextScene;

        Image imageProgressBar;
        [SerializeField] TMP_Text tmpTextLoadingTitle;

        private void Start()
        {
            imageProgressBar = transform.Find("Canvas/Panel/Image_Progress_Bar").GetComponent<Image>();
            tmpTextLoadingTitle = transform.Find("Canvas/Panel/TMP_Text_Loading_Title").GetComponent<TMP_Text>();

            tmpTextLoadingTitle.text = $"{stringNextScene} Loading...";
            StartCoroutine(LoadScene());
        }

        public static void LoadScene(string sceneName)
        {
            stringNextScene = sceneName;
            SceneManager.LoadScene("Loading_Scene");
        }

        IEnumerator LoadScene()
        {
            yield return null;

            AsyncOperation op = SceneManager.LoadSceneAsync(stringNextScene);
            op.allowSceneActivation = false;

            float timer = 0.0f;
            while (!op.isDone)
            {
                yield return null;
                
                timer += Time.deltaTime;

                if (op.progress >= 0.9f)
                {
                    imageProgressBar.fillAmount = Mathf.Lerp(imageProgressBar.fillAmount, 1f, timer);

                    if (imageProgressBar.fillAmount == 1.0f)
                    {
                        op.allowSceneActivation = true;
                    }
                }
                else
                {
                    imageProgressBar.fillAmount = Mathf.Lerp(imageProgressBar.fillAmount, op.progress, timer);
                    if (imageProgressBar.fillAmount >= op.progress)
                    {
                        timer = 0f;
                    }
                }
            }
        }

    }
}