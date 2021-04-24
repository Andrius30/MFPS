using UnityEngine;

namespace MFPS.ServerCharacters
{
    public enum PlayerState
    {
        Idle,
        Running,
        Walking,
        Crouching,
    }
    public class PlayerMove
    {
        Player player;
        PlayerJump playerJump;
        PlayerCrouch playerCrouch;

        PlayerState playerState;

        public PlayerMove(Player player)
        {
            this.player = player;
            playerJump = new PlayerJump(player);
            playerCrouch = new PlayerCrouch(player);
        }

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
                playerJump.Jump(player.otherInputs[0]);
                playerCrouch.Crouch(playerState);
            }

            player.velocityY += player.gravity;
            _moveDirection.y = player.velocityY;
            player.characterController.Move(_moveDirection);

            PacketsToSend.PlayerPosition(player);
            PacketsToSend.PlayerRotation(player);
            PacketsToSend.PlayerJumpVelocity(player, player.velocityY);
            PacketsToSend.PlayMovementAnimation(player, _inputDirection.x, _inputDirection.y);
        }

        void SetPlayerStateAndSpeed(PlayerState state, float speed)
        {
            player.moveSpeed = speed;
            playerState = state;
        }
        public PlayerState GetPlayerState() => playerState;
    }
}
