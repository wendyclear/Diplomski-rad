using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;

public class ConnectToServices : MonoBehaviourPunCallbacks //MonoBehaviourPunCallbacks gives access to helpful callbacks
{
    public static ConnectToServices Instance;

    private CanvasManager _canvasManager;
    [SerializeField] TMP_InputField _roomInputField;
    [SerializeField] GameObject _warningText;
    [SerializeField] Transform _roomListContent;
    [SerializeField] GameObject _roomListing;
    [SerializeField] Transform _playerListContent;
    [SerializeField] GameObject _playerListing;
    [SerializeField] private GameObject _startButton;


    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        _canvasManager = this.GetComponent<CanvasManager>();
    }
    void Start()
    {
        //connects to photon master server using the settings from PhotonServerSettings object (region, etc...)
        //Debug.Log("Connecting to master...");
        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString();
    }

    public override void OnConnectedToMaster()
    {
        //Debug.Log("Connected to master.");
        // base.OnConnectedToMaster();
        //to find, join or create rooms, the user/player needs to be in the lobby
        PhotonNetwork.JoinLobby();
        //we want to autoamtically sync scene chanegs when host switches
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        //base.OnJoinedLobby();
        //PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString();
        //Debug.Log("I joined lobby " + PhotonNetwork.NickName);
    }
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(_roomInputField.text))
        {
            if (!_warningText.activeInHierarchy) _warningText.SetActive(true);
            return;
        }
        //Debug.Log("Creating a room with name : " + _roomInputField.text);
        //when create room is called there are 2 callbacks that can be called - Successfully created room  (OnJoinedRoom) or failed to create a room (OnCreateRoomFailed)
        PhotonNetwork.CreateRoom(_roomInputField.text);
        _canvasManager.OpenLoadScreen();
    }

    public override void OnJoinedRoom()
    {
        // base.OnJoinedRoom();
        _canvasManager.EnterRoom();
        Player[] players = PhotonNetwork.PlayerList;
        foreach (Transform item in _playerListContent)
        {
            Destroy(item.gameObject);
        }

        GameObject player;
        for (int i = 0; i < players.Length; i++)
        {
            player = Instantiate(_playerListing, _playerListContent);
            player.GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        if (!PhotonNetwork.IsMasterClient) _startButton.SetActive(false);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        //base.OnCreateRoomFailed(returnCode, message);
        _canvasManager.ShowError(message);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        _canvasManager.OpenLoadScreen();
    }

    public override void OnLeftRoom()
    {
        //base.OnLeftRoom();
        _canvasManager.OpenTitleScreen();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform item in _roomListContent)
        {
            Destroy(item.gameObject);
        }
        //base.OnRoomListUpdate(roomList);
        //gives list of RoomInfo (class that stores information about rooms)
        GameObject listing;
        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList) continue;
            listing = Instantiate(_roomListing, _roomListContent);
            listing.GetComponent<ListItem>().SetUp(roomList[i]);
        }
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        _canvasManager.OpenLoadScreen();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //base.OnPlayerEnteredRoom(newPlayer);
        //when ANOTHER player enters
        GameObject player = Instantiate(_playerListing, _playerListContent);
        player.GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1); //1 as in order in build settings
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        //base.OnMasterClientSwitched(newMasterClient);
        if (PhotonNetwork.IsMasterClient) _startButton.SetActive(true);
    }
}
