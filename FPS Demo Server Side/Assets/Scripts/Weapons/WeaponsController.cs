using MFPS.ServerCharacters;
using System.Collections.Generic;

namespace MFPS.Weapons.Controllers
{
    public class WeaponsController
    {
        public List<BaseWeapon> weapons;

        public int currentWeaponIndex { get; set; }
        BaseWeapon currentWepon;
        Player player;

        public WeaponsController(Player player) => this.player = player;

        public void ChangeWeapon(Player player)
        {
            if (player.inputs[2] > 0) // up
            {
                currentWeaponIndex++;
                if (currentWeaponIndex > GetWeaponsLength() - 1)
                {
                    currentWeaponIndex = 0;
                }
                SetWeapon(player, currentWeaponIndex);
            }
            if (player.inputs[2] < 0) // down
            {
                currentWeaponIndex--;
                if (currentWeaponIndex < 0)
                {
                    currentWeaponIndex = GetWeaponsLength() - 1;
                }
                SetWeapon(player, currentWeaponIndex);
            }
        }
        public void SetWeapon(Player player, int index)
        {
            DisableAllWeapons();
            currentWepon = GetAllWeapons()[index];
            currentWepon.gameObject.SetActive(true);
            PacketsToSend.PlayerChangedWeapon(player, currentWepon);
        }
        public IWeapon GetCurrentWeaponType() => currentWepon.weaponType;
        public BaseWeapon GetCurrentWeapon() => currentWepon;
        public float GetCoolDown() => currentWepon.coolDown;

        void DisableAllWeapons()
        {
            foreach (BaseWeapon weapon in GetAllWeapons())
            {
                weapon.gameObject.SetActive(false);
            }
        }
        BaseWeapon[] GetAllWeapons() => player.weaponsParent.GetComponentsInChildren<BaseWeapon>(true);
        int GetWeaponsLength() => GetAllWeapons().Length;
    }
}