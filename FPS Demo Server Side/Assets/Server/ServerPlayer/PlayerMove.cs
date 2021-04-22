using UnityEngine;

namespace MFPS.ServerCharacters
{
    public enum PlayerState
    {
        Idle,
        Running,
        Walking,
        Crouching
    }
    public class PlayerMove
    {
        Player player;
        PlayerState playerState;

        public PlayerMove(Player player) => this.player = player;

        /// <summary>Calculates the player's desired movement direction and moves him.</summary>
        /// <param name="_inputDirection"></param>
        public void Move(float[] inputs)
        {
            Vector2 _inputDirection = Vector2.zero;
            _inputDirection.x = inputs[0];
            _inputDirection.y = inputs[1];
            Vector3 _moveDirection = player.transform.right * _inputDirection.x + player.transform.forward * _inputDirection.y;

            if (player.otherInputs[1])
                SetPlayerStateAndSpeed(PlayerState.Crouching, player.crouchSpeed);
            else if (player.otherInputs[2])
                SetPlayerStateAndSpeed(PlayerState.Walking, player.walkSpeed);
            else
                SetPlayerStateAndSpeed(PlayerState.Running, player.runSpeed);

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
            PacketsToSend.PlayMovementAnimation(player, _inputDirection.x, _inputDirection.y);
        }

        void SetPlayerStateAndSpeed(PlayerState state, float speed)
        {
            player.moveSpeed = speed;
            playerState = state;
        }
        // REFACTOR later if I decide to go further with this demo =======
        void Jump(bool input)
        {
            if (input)
                player.velocityY = player.jumpSpeed;
        }
        void Crouch()
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
        public PlayerState GetPlayerState() => playerState;
    }
}
