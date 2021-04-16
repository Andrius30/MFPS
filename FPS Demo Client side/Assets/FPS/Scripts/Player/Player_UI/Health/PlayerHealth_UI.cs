using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth_UI
{
    PlayerManager playerManager;
    GameObject healthCanvas;
    Image healthFillImg;
    TextMeshProUGUI playerHealthTxt;

    public PlayerHealth_UI(PlayerManager playerManager)
    {
        this.playerManager = playerManager;
        healthCanvas = playerManager.healthCanvas;
        CreateHealthCanvas();
        ShowHealthAt_UI(playerManager.playerHealth.GetHealth());
    }

    public void ShowHealthAt_UI(float health)
    {
        if (playerManager.isLocalPlayer)
        {
            healthFillImg.fillAmount = health / playerManager.maxHealth;
            SetText();
        }
    }
    void CreateHealthCanvas()
    {
        if (playerManager.isLocalPlayer)
        {
            GameObject gm = MonoBehaviour.Instantiate(healthCanvas, playerManager.transform);
            healthFillImg = UtilityMethods.onChildRequested(gm.transform, "Player_Health_Fill").GetComponent<Image>();
            playerHealthTxt = UtilityMethods.onChildRequested(gm.transform, "Player_Health_Txt").GetComponent<TextMeshProUGUI>();
        }
    }
    void SetText()
    {
        StringBuilder builder = new StringBuilder();
        playerHealthTxt.text = builder.Append($"{(int)playerManager.playerHealth.GetHealthPercent()} %").ToString();
    }
}
