using UnityEngine;

namespace MFPS.ServerCharacters
{
    class PlayerMove
    {
        Player player;
        bool isCrouching;

        public PlayerMove(Player player) => this.player = player;

        /// <summary>Calculates the player's desired movement direction and moves him.</summary>
        /// <param name="_inputDirection"></param>
        public void Move(Vector2 _inputDirection)
        {
            Vector3 _moveDirection = player.transform.right * _inputDirection.x + player.transform.forward * _inputDirection.y;

            if (!player.otherInputs[1])
            {
                player.moveSpeed = player.runSpeed;
                isCrouching = false;
            }
            else
            {
                player.moveSpeed = player.crouchSpeed;
                isCrouching = true;
            }

            _moveDirection *= player.moveSpeed;

            if (player.characterController.isGrounded)
            {
                player.velocityY = 0;
                Jump(player.otherInputs[0]);
                Crouch();
            }

            player.velocityY += player.gravity;
            _moveDirection.y = player.velocityY;
            player.characterController.Move(_moveDirection);

            PacketsToSend.PlayerPosition(player);
            PacketsToSend.PlayerRotation(player);
        }

        // REFACTOR later if I decide to go further with this demo =======
        void Jump(bool input)
        {
            if (input)
                player.velocityY = player.jumpSpeed;
        }
        void Crouch()
        {
            if (isCrouching)
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
