using System.Collections.Generic;
using TMPro;

public interface IConsole
{
    void Execute();
    void PrintToConsole(ref TextMeshProUGUI output, string prefix = "\n");
}
