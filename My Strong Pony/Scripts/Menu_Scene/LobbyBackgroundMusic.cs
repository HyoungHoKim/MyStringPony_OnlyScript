using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyBackgroundMusic : MonoBehaviour
{
    public AudioClip[] introMusic;
    private AudioSource _soundSource;
    void Start()
    {
        _soundSource = this.GetComponent<AudioSource>();
        StartCoroutine(playeList());
    }
    IEnumerator playeList()
    {
        _soundSource.clip = introMusic[0];
        _soundSource.Play();
        if (!_soundSource.isPlaying)
        {
            yield return null;
            StartCoroutine(loopBgm());
        }
    }

    IEnumerator loopBgm()
    {
        yield return new WaitForSeconds(10.0f);
        while (true)
        {
            if (!_soundSource.isPlaying)
            {
                _soundSource.clip = introMusic[1];
                _soundSource.PlayOneShot(introMusic[1]);
                _soundSource.loop = true;
            }
        }
    }

}
