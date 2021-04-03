namespace MFPS.Weapons
{
    public class Pistol : BaseWeapon, IWeapon
    {
        void Awake()
        {
            weaponType = this;
        }
    }
}