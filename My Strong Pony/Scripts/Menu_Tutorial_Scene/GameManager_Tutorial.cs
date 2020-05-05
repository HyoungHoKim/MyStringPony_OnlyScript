using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager_Tutorial : Base_Laser_Pointer
{
    public GameObject photonManager;

    public void Move_Game_Scene()
    {
        PhotonNetwork.OfflineMode = true;
        photonManager.GetComponent<PhotonManager>().ConnectPhotonServer();
    }

    public void Move_MultiPlay_Scene()
    {
        photonManager.GetComponent<PhotonManager>().ConnectPhotonServer();
    }

    public void Application_Quit()
    {
        Application.Quit();
    }
}