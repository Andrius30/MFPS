using System.Collections.Generic;
using TMPro;
using UnityEngine;
public struct Com
{
    Dictionary<string, string> commands;

    public void Init() => commands = new Dictionary<string, string>();
    public void AddCommand(string command, string description) => commands.Add(command, description);
    public Dictionary<string, string> GetAvailableCommands() => commands;
}
public class ConsoleController : MonoBehaviour, IConsole
{
    public static ConsoleController instance;

    public KeyCode consoleInput;
    public KeyCode consoleOpenCloseInput = KeyCode.F1;

    public TextMeshProUGUI outputText;
    public TMP_InputField userInput;
    public Animator anim;
    public Dictionary<Com, IConsole> consoles = new Dictionary<Com, IConsole>();

    bool isOpened = true;

    void Awake()
    {
        if (instance == null)
            instance = this;
        Com newCommands = new Com();
        newCommands.Init();
        newCommands.AddCommand("help", "Show available commands");

        consoles.Add(newCommands, this);
    }

    void Update()
    {
        if (Input.GetKeyDown(consoleInput))
        {
            ExecuteCommand(userInput.text);
            ClearInput();
        }
        if (Input.GetKeyDown(consoleOpenCloseInput))
            OpenClose();
    }

    public void ExecuteCommand(string input)
    {
        if (input.Equals(string.Empty)) return;
        foreach (var console in consoles)
        {
            foreach (var com in console.Key.GetAvailableCommands())
            {
                Debug.Log($"com {com.Key } input {input }");
                if (!com.Key.Equals(input)) continue;
                if (com.Key.Equals(input))
                {
                    console.Value.Execute();
                    console.Value.PrintToConsole(ref outputText);
                    KeepFocused();
                    return;
                }
            }
        }
        outputText.text += $"\nCommand { input } was not found! Type help to show available commands :red;".Interpolate();
        KeepFocused();
    }

    public void Execute() { }
    public void PrintToConsole(ref TextMeshProUGUI output, string prefix = "\n")
    {
        foreach (var console in consoles)
        {
            foreach (var com in console.Key.GetAvailableCommands())
            {
                output.text += $"{ prefix } { com.Key } -> { com.Value } :yellow;".Interpolate();
            }
        }
    }
    void ClearInput() => userInput.text = string.Empty;
    void OpenClose()
    {
        isOpened = !isOpened;
        if (isOpened)
            anim.SetTrigger("close");
        else
        {
            anim.SetTrigger("open");
            KeepFocused();
        }
    }
    void KeepFocused() => userInput.ActivateInputField();
}
