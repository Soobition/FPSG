using UnityEngine;

public class PlayerSetup : MonoBehaviour
{

    public Movement movement;

    public GameObject camera;


    public void IsLocalPlayer()
    {
        movement.enabled = true;

        camera.SetActive(true);
    }

}
