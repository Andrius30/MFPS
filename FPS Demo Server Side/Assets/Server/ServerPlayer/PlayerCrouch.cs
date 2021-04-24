
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
                SetCharacterController(0.8f, player.crouchCenter);
            else
                SetCharacterController(1.48f, -0.22f);
        }

        void SetCharacterController(float height, float center)
        {
            player.characterController.height = height;
            player.characterController.center = new Vector3(0, center, 0);
        }

    }
}
