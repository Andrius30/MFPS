using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour, IConsole
{
    [SerializeField] float mouseSensitivity = 100f;

    [SerializeField] Transform playerBody;
    [SerializeField] Transform aimingPivot;
    [SerializeField] Animator anim;
    [SerializeField] float minAngle = -30;
    [SerializeField] float maxAngle = 45;

    float xRotation = 0;
    CharacterAiming aiming;
    Com com;

    public void Execute() // TESTING
    {
        Debug.Log("Camera controller something happened");
    }

    public void PrintToConsole(ref TextMeshProUGUI output, string prefix)
    {
        output.text += $"{ prefix } Camera has been disabled. :{Color.green };\n".Interpolate();
    }

    void Awake()
    {
        aiming = new CharacterAiming(aimingPivot);
        com = new Com();
        com.Init();
        com.AddCommand("cmr","(TEST) Disables camera.");
        ConsoleController.instance.consoles.Add(com, this); // TESTING
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerBody.Rotate(Vector3.up * mouseX);

        //  transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // TODO: update local animation
        aiming.SetRotation(mouseY);
        anim.SetFloat("aimAngle", aiming.GetAngle());
        float t = aiming.GetAngle();
        t = Mathf.Clamp(t, minAngle, maxAngle);
        aimingPivot.localRotation = Quaternion.Euler(t, 0, 0);

        //TODO: send packet with aiming pivot to infor other clients about my current aiming angle

    }
}
