using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerSetup : MonoBehaviour
{

    public Movement movement;


    public GameObject camera;


    public string nickname;


    public TextMeshPro nicknameText;


    public Transform tpWeaponHolder;


    [Header("Aim")]
    public GameObject akAim;
    public GameObject pistolAim;
    public GameObject shotgunAim;
    public GameObject launcherAim;



    public static bool isAK, isPistol, isShotgun, isLauncher;



    private void Update()
    {
        if (isAK)
        {
            akAim.SetActive(true);
            pistolAim.SetActive(false);
            shotgunAim.SetActive(false);
            launcherAim.SetActive(false);
        }
        else if (isPistol)
        {
            akAim.SetActive(false);
            pistolAim.SetActive(true);
            shotgunAim.SetActive(false);
            launcherAim.SetActive(false);
        }
        else if (isShotgun)
        {
            akAim.SetActive(false);
            pistolAim.SetActive(false);
            shotgunAim.SetActive(true);
            launcherAim.SetActive(false);
        }
        else
        {
            akAim.SetActive(false);
            pistolAim.SetActive(false);
            shotgunAim.SetActive(false);
            launcherAim.SetActive(true);
        }
    }




    public void IsLocalPlayer()
    {
        tpWeaponHolder.gameObject.SetActive(false);


        movement.enabled = true;

        camera.SetActive(true);
    }



    [PunRPC]
    public void SetTPWeapon(int _weaponIndex)
    {
        foreach (Transform _weapon in tpWeaponHolder)
        {
            _weapon.gameObject.SetActive(false);
        }

        tpWeaponHolder.GetChild(_weaponIndex).gameObject.SetActive(true);
    }



    /*[PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;

        nicknameText.text = nickname;
    }*/


    [PunRPC]
    public void SetNickname(string _name, PhotonMessageInfo info)
    {
        nickname = _name;
        // Check if the player is the local player
        if (info.Sender != PhotonNetwork.LocalPlayer)
        {
            nicknameText.text = nickname;
        }
        else { nicknameText.text = ""; } // Or you can set it to null or any other placeholder

    }

}
