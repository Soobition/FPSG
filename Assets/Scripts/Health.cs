using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{

    public int health;


    public bool isLocalPlayer;


    public RectTransform healthBar;


    public float originalHealthBarSize;


    [Header("UI")]
    public TextMeshProUGUI healthText;


    private bool hasDied;



    private void Start()
    {
        originalHealthBarSize = healthBar.sizeDelta.x;
    }


    [PunRPC]
    public void TakeDamage(int _damage)
    {
        if (hasDied)
            return;

        health -= _damage;

        healthBar.sizeDelta = new Vector2(originalHealthBarSize * health / 100f, healthBar.sizeDelta.y);

        healthText.text = health.ToString();

        if (health <= 0)
        {
            hasDied = true;

            if (isLocalPlayer)
            {
                RoomManager.instance.SpawnPlayer();

                RoomManager.instance.deaths++;
                RoomManager.instance.SetHashes();
            }

            Destroy(gameObject);
        }
    }

}
