using MFPS.ServerCharacters;
using System.Collections.Generic;

namespace MFPS.Weapons.Controllers
{
    public class WeaponsController
    {
        public List<BaseWeapon> weapons;

        BaseWeapon currentWeapon;
        Player player;

        public WeaponsController(Player player) => this.player = player;

        public void ChangeWeapon(int currentWeaponIndex) => SetWeapon(player, currentWeaponIndex);
        public void SetWeapon(Player player, int index)
        {
            DisableAllWeapons();
            currentWeapon = GetAllWeapons()[index];
            currentWeapon.SetWeaponState(WeaponState.DrawWeapon);
            currentWeapon.gameObject.SetActive(true);
            PacketsToSend.PlayerChangedWeapon(player, currentWeapon);
        }
        public IWeapon GetCurrentWeaponType() => currentWeapon.weaponType;
        public BaseWeapon GetCurrentWeapon() => currentWeapon;
        public float GetCoolDown() => currentWeapon.coolDown;
        public BaseWeapon[] GetAllWeapons() => player.weaponsParent.GetComponentsInChildren<BaseWeapon>(true);

        void DisableAllWeapons()
        {
            foreach (BaseWeapon weapon in GetAllWeapons())
            {
                weapon.gameObject.SetActive(false);
            }
        }
    }
}