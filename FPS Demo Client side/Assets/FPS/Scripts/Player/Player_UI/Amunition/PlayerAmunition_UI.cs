using System.Text;
using TMPro;
using UnityEngine;

public class PlayerAmunition_UI
{
    PlayerManager playerManager;
    TextMeshProUGUI maxbulletsTxt;
    TextMeshProUGUI bulletsLeftTxt;

    public PlayerAmunition_UI(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
        CreateAmmoUI();
    }

    void CreateAmmoUI()
    {
        if (playerManager.isLocalPlayer)
        {
            GameObject gm = MonoBehaviour.Instantiate(playerManager.ammunitionCanvas, playerManager.transform);
            maxbulletsTxt = UtilityMethods.onChildRequested?.Invoke(gm.transform, "Maxbullets").GetComponent<TextMeshProUGUI>();
            bulletsLeftTxt = UtilityMethods.onChildRequested?.Invoke(gm.transform, "BulletsLeftTxt").GetComponent<TextMeshProUGUI>();
        }
    }

    public void SetText(int maxBullets, int bulletsLeft)
    {
        if (playerManager.isLocalPlayer)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder builder_max = new StringBuilder();
            bulletsLeftTxt.text = builder.Append($"{bulletsLeft}/").ToString();
            maxbulletsTxt.text = builder_max.Append($"{maxBullets}").ToString();
        }
    }
    public void UpdateBulletsLeft(int left)
    {
            StringBuilder builder = new StringBuilder();
            bulletsLeftTxt.text = builder.Append($"{left}/").ToString();
    }
}
