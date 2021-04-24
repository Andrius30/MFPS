using System;
using System.IO;
using UnityEngine;

public class ScreenShot : MonoBehaviour
{
    [SerializeField] KeyCode ssInput; // screenShot input
    [SerializeField] string filePath = "/ScreenShots";
    [SerializeField] string fileName = "TEST ";
    [SerializeField] string extention = ".PNG";

    //void Awake() => 

    void Update()
    {
        if (GetInput())
        {
            TakeScreenShot();
        }
    }

    void TakeScreenShot() => ScreenCapture.CaptureScreenshot(Path.Combine(filePath, fileName + DateTime.Now.ToString("MMMM-dd-yyyy-hh-mm-ss") + extention));
    bool GetInput() => Input.GetKeyDown(ssInput);
}
