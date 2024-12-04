using UnityEngine;
using Photon.Pun;


public class RoomManager : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject player;

    [Space]
    [SerializeField] private Transform spawnPoint;

    
    void Start()
    {
        Debug.Log("Connecting...");  

        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        Debug.Log("Connected to Server!");

        PhotonNetwork.JoinLobby(); 
    }


    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        Debug.Log("We're in the lobby!");

        PhotonNetwork.JoinOrCreateRoom("Test", null, null);
    }


    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        Debug.Log("We're connected and in a room!");

        GameObject _player = PhotonNetwork.Instantiate(player.name, spawnPoint.position, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
    }

}
