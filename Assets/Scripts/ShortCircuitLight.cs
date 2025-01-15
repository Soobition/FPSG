using Photon.Pun;
using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ShortCircuitLight : MonoBehaviour
{
    public GameObject light;



    private void Start()
    {
        StartCoroutine(StartShortCircuit());
    }


    IEnumerator StartShortCircuit()
    {
        while (true)
        {
            GetComponent<PhotonView>().RPC("ShortCircuit", RpcTarget.All);
            yield return new WaitForSeconds(21f);   // Adjust the interval between repetitions
        }
    }


    [PunRPC]
    IEnumerator ShortCircuit()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 1; j <= 5; j++)
            {
                yield return new WaitForSeconds(.1f);
                light.SetActive(true);

                yield return new WaitForSeconds(.1f);
                light.SetActive(false);
            }


            if (i == 2)
            {
                light.SetActive(true);
                yield return new WaitForSeconds(1);
                light.SetActive(false);
                yield return new WaitForSeconds(1);
                light.SetActive(true);
            }
            else if (i == 4)
            {
                light.SetActive(false);
                yield return new WaitForSeconds(2);
                light.SetActive(true);
                yield return new WaitForSeconds(1);
                light.SetActive(false);
            }
            else if (i >= 6)
            {
                light.SetActive(true);
                yield return new WaitForSeconds(4);
                light.SetActive(false);
            }
        }
    }
}
