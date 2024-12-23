using Photon.Pun;
using UnityEngine;

public class PlayerPhotonSoundManager : MonoBehaviour
{

    public AudioSource footstepSourse;
    public AudioClip footstepSFX;

    public AudioSource gunShootSourse;
    public AudioClip[] allGunShootSFX;



    public void PlayFootstepSFX()
    {
        GetComponent<PhotonView>().RPC("PlayFootstepSFX_RPC", RpcTarget.All);
    }


    [PunRPC]
    public void PlayFootstepSFX_RPC()
    {
        footstepSourse.clip = footstepSFX;

        // Pitch and volume
        footstepSourse.pitch = UnityEngine.Random.Range(.7f, 1.2f);
        footstepSourse.volume = UnityEngine.Random.Range(.1f, .22f);

        footstepSourse.Play();
    }


    public void PlayRunningFootstepSFX()
    {
        if (Weapon.isRunning)
        {
            GetComponent<PhotonView>().RPC("PlayRunningFootstepSFX_RPC", RpcTarget.All);
        }
    }


    [PunRPC]
    public void PlayRunningFootstepSFX_RPC()
    {
        footstepSourse.clip = footstepSFX;

        // Pitch and volume
        footstepSourse.pitch = UnityEngine.Random.Range(.7f, 1.2f);
        footstepSourse.volume = UnityEngine.Random.Range(.2f, .35f);

        footstepSourse.Play();
    }



    public void PlayShootSFX(int index)
    {
        GetComponent<PhotonView>().RPC("PlayShotSFX_RPC", RpcTarget.All, index);
    }


    [PunRPC]
    public void PlayShotSFX_RPC(int index)
    {
        gunShootSourse.clip = allGunShootSFX[index];

        // Pitch and volume
        gunShootSourse.pitch = UnityEngine.Random.Range(1f, 1.2f);
        gunShootSourse.volume = UnityEngine.Random.Range(.55f, .75f);

        gunShootSourse.Play();
    }

}
