using MFPS.ServerCharacters;
using System.Collections.Generic;
using UnityEngine;

namespace MFPS.Weapons.Controllers
{
    public class WeaponsController
    {
        public List<BaseWeapon> weapons;

        public int currentWeaponIndex { get; set; }
        BaseWeapon currentWepon;
        BaseWeapon oldWep;
        Player player;

        public WeaponsController(Player player, List<BaseWeapon> allWeapons)
        {
            this.player = player;
            weapons = new List<BaseWeapon>(allWeapons);
            for (int i = 0; i < weapons.Count; i++)
            {
                weapons[i].id = i;
            }
        }

        public void ChangeWeapon(Player player)
        {
            if (player.inputs[2] > 0) // up
            {
                currentWeaponIndex++;
                if (currentWeaponIndex > weapons.Count - 1)
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
                    currentWeaponIndex = weapons.Count - 1;
                }
                SetWeapon(player, currentWeaponIndex);
            }
        }
        void SetWeapon(Player player, int index)
        {
            if (oldWep != null)
            {
                MonoBehaviour.Destroy(oldWep.gameObject);
            }
            GameObject gm = MonoBehaviour.Instantiate(weapons[index].gameObject, player.transform);
            gm.transform.localPosition = Vector3.zero;
            currentWepon = gm.GetComponent<BaseWeapon>();
            oldWep = currentWepon;
            PacketsToSend.PlayerChangedWeapon(player, currentWepon);
        }
        public IWeapon GetCurrentWeaponType() => currentWepon.weaponType;
        public BaseWeapon GetCurrentWeapon() => currentWepon;
        public float GetCoolDown() => currentWepon.coolDown;
    }
}