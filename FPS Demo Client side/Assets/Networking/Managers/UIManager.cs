using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject startMenu;
    public TMP_InputField userNameField;
    public Button startButton;

    void Awake()
    {
        if (instance == null) instance = this;
        startButton.onClick.AddListener(ConnectToServer);
    }

    void ConnectToServer()
    {
        startMenu.SetActive(false);
        userNameField.interactable = false;
        Client.instance.ConnectToServer();
    }
}
