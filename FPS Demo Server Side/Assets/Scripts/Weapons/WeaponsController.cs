using System.Collections.Generic;
using UnityEngine;

public class WeaponsController
{
    public List<BaseWeapon> weapons;

    public int currentWeaponIndex { get; set; }
    BaseWeapon currentWepon;
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

    public void ChangeWeapon()
    {
        if (player.inputs[2] > 0) // up
        {
            currentWeaponIndex++;
            if (currentWeaponIndex > weapons.Count - 1)
            {
                currentWeaponIndex = 0;
            }
            SetWeapon(currentWeaponIndex);
        }
        if (player.inputs[2] < 0) // down
        {
            currentWeaponIndex--;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weapons.Count - 1;
            }
            SetWeapon(currentWeaponIndex);
        }
    }
    void SetWeapon(int index)
    {
        currentWepon = weapons[index];
        PacketsToSend.PlayerChangedWeapon(player, currentWepon);
    }
    public BaseWeapon GetCurrentWeapon() => currentWepon;
    public float GetCoolDown() => currentWepon.coolDown;
}
