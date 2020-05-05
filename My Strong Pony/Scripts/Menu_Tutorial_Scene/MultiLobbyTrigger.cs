using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiLobbyTrigger : MonoBehaviour
{
    public GameObject gameManager;

    private void OnTriggerEnter(Collider other)
    {
        gameManager.GetComponent<GameManager_Tutorial>().Move_MultiPlay_Scene();
    }
}
