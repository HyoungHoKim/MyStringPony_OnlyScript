using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBackgroundMusic : MonoBehaviour
{
   /* public AudioClip[] backgroundMusic;
    private AudioSource _soundSource;
    void Start()
    {
        _soundSource = this.GetComponent<AudioSource>();
        _soundSource.clip = backgroundMusic[0];
        _soundSource.Play();

        StartCoroutine(loopBgm());
    }
    IEnumerator loopBgm()
    {
        while (true)
        {
            yield return null;
            if (!_soundSource.isPlaying)
            {
                _soundSource.clip = backgroundMusic[1];
                _soundSource.PlayOneShot(backgroundMusic[1]);
                _soundSource.loop = true;
            }
        }
    }*/
}
