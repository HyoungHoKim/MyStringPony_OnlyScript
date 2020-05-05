using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    private const string gameVersion = "1.0";
    private string playerName;
    private string roomName;

    public byte maxPlayers = 2;

    void Awake()
    {
        // 자동으로 마스터의 Scene을 싱크해줘서 각자 신을 로드할 필요가 없음 
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        // 플레이어 명 랜덤 생성 
        playerName = PlayerPrefs.GetString("PLAYER_NAME", "Player_" + Random.Range(0, 100).ToString("000"));
        roomName = "Room_" + Random.Range(0, 101).ToString("100");

    }

    #region PUN_CALLBACK

    public void ConnectPhotonServer()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already Connected");
        }
        else
        {
            PhotonNetwork.GameVersion = gameVersion;
            PhotonNetwork.NickName = playerName;
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    //PUN에 접속했을 때 콜백
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected To Master");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        
        Debug.Log($"Join Failure {returnCode} / {message}");

        RoomOptions ro = new RoomOptions();
        ro.MaxPlayers = maxPlayers; // 접속 가능 플레이어 수 
        ro.IsOpen = true;           // 접속 가능 여부
        ro.IsVisible = true;        // 룸에 접속했을 때 목록에 표시여부

        PhotonNetwork.CreateRoom("MyRoom", ro);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Create and Joined Room !!!");

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel("MultiPlayer");
        }
    }

    #endregion

}
