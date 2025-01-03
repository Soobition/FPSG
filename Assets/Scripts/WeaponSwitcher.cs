using Photon.Pun;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{

    public PhotonView playerSetupView;


    private int selectedWeapon = 0;


    void Start()
    {
       // SelectWeapon();
    }


    void Update()
    {
        int previousSelectedWeapon = selectedWeapon;


        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedWeapon = 0;
        }


        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedWeapon = 1;
        }


        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedWeapon = 2;
        }


        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedWeapon = 3;
        }


        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selectedWeapon >= transform.childCount - 1)
            {
                selectedWeapon = 0;
            }
            else { selectedWeapon += 1; }
        }


        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedWeapon <= 0)
            {
                selectedWeapon = transform.childCount - 1;
            }
            else { selectedWeapon -= 1; }
        }



        if (previousSelectedWeapon != selectedWeapon)
        {
            Weapon.isHolsterUp = true;

            Weapon.isHolsterDown = false;


            SelectWeapon();
        }
    }



    private void SelectWeapon()
    {
        playerSetupView.RPC("SetTPWeapon", RpcTarget.All, selectedWeapon);


        int i = 0;

        foreach (Transform _weapon in transform)
        {
            if (i == selectedWeapon)
            {
                _weapon.gameObject.SetActive(true);
            }
            else
            {
                _weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}
