
namespace MFPS.ServerCharacters
{
    public class PlayerJump
    {
        Player player;

        public PlayerJump(Player player) => this.player = player;

        public void Jump(bool input)
        {
            if (input)
            {
                player.velocityY = player.jumpSpeed;
                PacketsToSend.PlayerJumpVelocity(player, player.velocityY);
            }
        }

    }
}
