using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{

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


        if (previousSelectedWeapon != selectedWeapon)
        {
            Weapon.isHolsterUp = true;

            Weapon.isHolsterDown = false;


            SelectWeapon();
        }
    }



    private void SelectWeapon()
    {
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
