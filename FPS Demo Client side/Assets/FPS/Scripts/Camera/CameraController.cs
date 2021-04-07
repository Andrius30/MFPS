using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 100f;

    [SerializeField] Transform playerBody;
    [SerializeField] Transform aimingPivot;
    [SerializeField] Animator anim;
    [SerializeField] float minAngle = -30;
    [SerializeField] float maxAngle = 45;

    CharacterAiming aiming;

    void Awake()
    {
        aiming = new CharacterAiming(aimingPivot);
    }

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);

        // TODO: update local animation
        aiming.SetRotation(mouseY);
        anim.SetFloat("aimAngle", aiming.GetAngle());
        float t = aiming.GetAngle();
        t = Mathf.Clamp(t, minAngle, maxAngle);
        aimingPivot.localRotation = Quaternion.Euler(t, 0, 0);

        //TODO: send packet with aiming pivot to inform other clients about my current aiming angle

    }
    void FixedUpdate() => SendToServerUpdateAimAnimation();
    void SendToServerUpdateAimAnimation()
    {
        PacketsToSend.PlayerAimingAnim(aiming.GetAngle(), aimingPivot.localRotation);
    }
}
