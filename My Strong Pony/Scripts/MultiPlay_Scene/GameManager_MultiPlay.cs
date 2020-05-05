using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Y;

public class GameManager_MultiPlay : MonoBehaviourPunCallbacks
{
    public Transform[] spawnPoint;
    public int aiTeamNum = 1;
    public float offineModeReadyTime = 5;

    private Room room;
    private GameObject player;
    private GameObject aiTeam;


    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            player = PhotonNetwork.Instantiate("PlayerAvatar", spawnPoint[0].position, spawnPoint[0].rotation, 0, null);
            for (int i = 1; i <= aiTeamNum; i++)
            aiTeam = PhotonNetwork.Instantiate("AIRedTeam", spawnPoint[0].position + new Vector3(0, 0, i), spawnPoint[0].rotation, 0, null);
        }
        else
        {
            player = PhotonNetwork.Instantiate("PlayerAvatar", spawnPoint[1].position, spawnPoint[1].rotation, 0, null);
            for (int i = 1; i <= aiTeamNum; i++)
            aiTeam = PhotonNetwork.Instantiate("AIBlueTeam", spawnPoint[1].position + new Vector3(0, 0, -i), spawnPoint[1].rotation, 0, null);
        }

        player.GetComponent<Y.Move_Test_Scene>().isMove = false;
        foreach (HorseMoveLists ai in aiTeam.GetComponentsInChildren<HorseMoveLists>())
        {
            ai.enabled = false;
        }

        Debug.Log(PhotonNetwork.OfflineMode);

        if (PhotonNetwork.OfflineMode)
        {
            StartCoroutine("OfflineStart");
        }

        Debug.Log(PhotonNetwork.PlayerList.Length);
        Debug.Log(PhotonNetwork.CurrentRoom.MaxPlayers);
    }

    IEnumerator OfflineStart()
    {
        yield return new WaitForSeconds(offineModeReadyTime);
        Debug.Log("Game Start!!");
        player.GetComponent<Y.Move_Test_Scene>().isMove = true;
        foreach (HorseMoveLists ai in aiTeam.GetComponentsInChildren<HorseMoveLists>())
        {
            ai.enabled = true;
        }
        
    }

    private void Update()
    {

        //if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        //{
        //    Debug.Log("Game Start!!");

        //    photonView.RPC("ReadyToStart", RpcTarget.All);
        //}
    }

    [PunRPC]
    void ReadyToStart()
    {
        Debug.Log("check readyTostart");
        player.GetComponent<Y.Move_Test_Scene>().isMove = photonView.IsMine;
        foreach (HorseMoveLists ai in aiTeam.GetComponentsInChildren<HorseMoveLists>())
        {
            ai.enabled = photonView.IsMine;
        }
    }

    public void OnExitGameButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player={newPlayer.NickName} is entered !!!");

        if (PhotonNetwork.PlayerList.Length == PhotonNetwork.CurrentRoom.MaxPlayers)
        {
            Debug.Log("Game Start!!");

            Debug.Log($"Player={newPlayer.NickName}'s ReadyToStart !!!");

            photonView.RPC("ReadyToStart", RpcTarget.All);
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player={otherPlayer.NickName} is exit !!!");
    }
}
