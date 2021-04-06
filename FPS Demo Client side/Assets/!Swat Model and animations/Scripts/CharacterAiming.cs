using UnityEngine;

public class CharacterAiming
{
    Transform transform;

    public CharacterAiming(Transform transform)
    {
        this.transform = transform;
    }

    public void SetRotation(float amount)
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x - amount,
            transform.eulerAngles.y, transform.eulerAngles.z);
    }

    public float GetAngle() => CheckAngle(transform.eulerAngles.x);

    public float CheckAngle(float value)
    {
        float angle = value - 180f;

        if (angle > 0)
            return angle - 180f;

        return angle + 180f;
    }
}
