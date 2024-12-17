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



    [PunRPC]
    public void SetNickname(string _name)
    {
        nickname = _name;

        nicknameText.text = nickname;
    }

}
