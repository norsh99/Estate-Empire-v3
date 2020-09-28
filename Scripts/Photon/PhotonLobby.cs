using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PhotonLobby : MonoBehaviourPunCallbacks
{

    public static PhotonLobby lobby;

    //public GameObject battleButton;
    //public GameObject cancelButton;


    private GameMaster gameMasterRef;
    private int idListing;


    private void Awake()
    {
        lobby = this; //Creates the singleton, live without the Main menu scene.
    }

    void Start()
    {
        gameMasterRef = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameMaster>();
        idListing = 0;

        PhotonNetwork.ConnectUsingSettings(); //Connects to master photon server.
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Player has connected to the Photon master server");
        PhotonNetwork.AutomaticallySyncScene = true;

        gameMasterRef.BringInChoosePlayersPanel();
    }

    bool canClickCreateNewRoom = true;
    public void OnClickCreateNewRoom() 
    {
        if (canClickCreateNewRoom)
        {
            CreateRoom();
            canClickCreateNewRoom = false;
            StartCoroutine(WaitToClick());
        }
    }

    IEnumerator WaitToClick()
    {
        yield return new WaitForSeconds(3);
        canClickCreateNewRoom = true;
    }

    public void OnClickJoinNewRoom()
    {
        //PhotonNetwork.JoinRandomRoom();
        if (gameMasterRef.codeInputField.text != "")
        {
            PhotonNetwork.JoinRoom("Room"+gameMasterRef.codeInputField.text);
        }
        else
        {
            gameMasterRef.JoinButtonSVGImage.color = new Color(1.0f, 0.3f, 0.3f, 1.0f);
            StartCoroutine(ResetJoinButtonColor());
        }
    }

    IEnumerator ResetJoinButtonColor()
    {
        yield return new WaitForSeconds(1.0f);
        gameMasterRef.JoinButtonSVGImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    }

    public void OnClickStartGame()
    {
        //TODO label each player with IDs
        if (PhotonNetwork.IsMasterClient)
        {
            gameMasterRef.JoinedRoomAsMasterSuccessfully();
            gameMasterRef.playerPhoton.StartGame();
        }
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to join the game but failed. There must be no open games available");
        gameMasterRef.JoinButtonSVGImage.color = new Color(1.0f, 0.3f, 0.3f, 1.0f);
        gameMasterRef.codeInputField.text = "";
        StartCoroutine(ResetJoinButtonColor());
        //CreateRoom(); Old Code
    }

    void CreateRoom()
    {
        Debug.Log("Trying to create a room");
        int randomRoomName = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = 4, PublishUserId = true };
        PhotonNetwork.CreateRoom("Room" + randomRoomName, roomOps);
        gameMasterRef.code.text = "Code: " + randomRoomName;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("We are now in the room");
        GameObject playerPhotonGameObject = PhotonNetwork.Instantiate("PlayerPhotonPrefab", transform.position, Quaternion.identity, 0);
        gameMasterRef.playerPhoton = playerPhotonGameObject.GetComponent<PlayerPhotonScript>();

        playerPhotonGameObject.GetComponent<PlayerPhotonScript>().id = idListing;

        gameMasterRef.playersInGame.text = "Players: " + PhotonNetwork.PlayerList.Length;

        if (PhotonNetwork.IsMasterClient)
        {
            gameMasterRef.choosePlayersPanel.SetActive(false);
            gameMasterRef.waitingToStartScreen.SetActive(true);
            gameMasterRef.startButton.SetActive(true);
        }
        else
        {
            gameMasterRef.choosePlayersPanel.SetActive(false);
            gameMasterRef.waitingToStartScreen.SetActive(true);
            gameMasterRef.startButton.SetActive(false);
        }
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Tried to create a new room but failed, there must already be a room with the same name");
        CreateRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        gameMasterRef.playersInGame.text = "Players: " + PhotonNetwork.PlayerList.Length;
    }

    public void OnCancelButonClicked() 
    {
        //cancelButton.SetActive(false);
        //battleButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    
    
    }







}
