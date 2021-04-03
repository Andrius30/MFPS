namespace MFPS.Weapons
{
    public class AK47 : BaseWeapon, IWeapon
    {
        void Awake()
        {
            weaponType = this;
        }
    }
}