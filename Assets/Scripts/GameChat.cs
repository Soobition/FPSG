using UnityEngine;
using TMPro;
using WebSocketSharp;
using Photon.Pun;
using Photon.Realtime;

public class GameChat : MonoBehaviour
{

    public TextMeshProUGUI chatText;

    public TMP_InputField inputField;



    private bool isInputFieldToggled;



    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Y) && !isInputFieldToggled)
        {
            isInputFieldToggled = true;
            inputField.Select();
            inputField.ActivateInputField();

            TogglePlayerComponents(false);

            Debug.Log("Toggled on");
        }


        if (Input.GetKeyDown(KeyCode.Escape) && isInputFieldToggled)
        {
            isInputFieldToggled = false;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            TogglePlayerComponents(true);

            Debug.Log("Toggled off");
        }



        // Sending a Message
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter) && isInputFieldToggled && !inputField.text.IsNullOrEmpty())
        {
            // Sending a message

            string messageToSend = $"{PhotonNetwork.LocalPlayer.NickName}: {inputField.text}";

            GetComponent<PhotonView>().RPC("SendChatMessage", RpcTarget.All, messageToSend);

            inputField.text = "";

            isInputFieldToggled = false;

            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            Debug.Log("Message sent");


            // Toggled on
            isInputFieldToggled = true;
            inputField.Select();
            inputField.ActivateInputField();

            Debug.Log("Toggled on");
        }
    }


    private void TogglePlayerComponents(bool state)
    {
        GameObject player = GameObject.Find("Player(Clone)");

        if (player != null)
        {
            player.GetComponent<Movement>().enabled = state;

            Weapon[] weapons = player.GetComponentsInChildren<Weapon>();
            foreach (Weapon Item in weapons)
            {
                Item.enabled = state;
            }
        }
        else
        {
            Debug.LogWarning("Player object not found");
        }

    }


        [PunRPC]
    public void SendChatMessage(string _message)
    {
        chatText.text = chatText.text + "\n" + _message;
    }
}
