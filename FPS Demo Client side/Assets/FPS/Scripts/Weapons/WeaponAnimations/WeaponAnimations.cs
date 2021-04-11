
///<Summary> Because its not production type game I'm adding just basic self made generic animations </Summary>
public class WeaponAnimations
{
    ClientWeapon baseWeapon;

    public WeaponAnimations(ClientWeapon baseWeapon) => this.baseWeapon = baseWeapon;

    public void PlayShootingAnimation() => baseWeapon.weaponAnimator.SetTrigger("shoot");
    public void PlayReloadingAnimation() => baseWeapon.weaponAnimator.SetBool("isReloading", true);
    public void DrawAnimations() => baseWeapon.weaponAnimator.SetTrigger("drawout");
    public void MovingSidesAnimation(float value)
    {
        if (value == 0)
            Reset();
        if (value < 0)
            SetSidesMoveAnimation("isLeft", true);
        if (value > 0)
            SetSidesMoveAnimation("isRight", true);
    }

    void SetSidesMoveAnimation(string animName, bool isMoving) => baseWeapon.weaponAnimator.SetBool(animName, isMoving);
    void Reset()
    {
        SetSidesMoveAnimation("isLeft", false);
        SetSidesMoveAnimation("isRight", false);
    }
}
