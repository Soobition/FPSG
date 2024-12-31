using UnityEngine;

public class RoomItemButton : MonoBehaviour
{

    public string roomName;

    public int sceneIndex = 1;


    public void OnButtonPressed()
    {
        RoomList.instance.JoinRoomByName(roomName, sceneIndex);
    }

}
