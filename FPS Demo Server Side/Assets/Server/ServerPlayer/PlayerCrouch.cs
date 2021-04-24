
using UnityEngine;

namespace MFPS.ServerCharacters
{
    public class PlayerCrouch
    {
        Player player;

        public PlayerCrouch(Player player) => this.player = player;

        public void Crouch(PlayerState playerState)
        {
            if (playerState == PlayerState.Crouching)
                SetCharacterController(1, player.crouchCenter);
            else
                SetCharacterController(2, 0);
        }

        void SetCharacterController(float height, float center)
        {
            player.characterController.height = height;
            player.characterController.center = new Vector3(0, center, 0);
        }

    }
}
