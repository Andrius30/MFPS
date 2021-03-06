using UnityEngine;

public class PlayerAnimations
{
    PlayerManager playerManager;

    public PlayerAnimations(PlayerManager playerManager) => this.playerManager = playerManager;

    public void PlayMoveAnimation(float x, float z)
    {
        playerManager.characterAnimator.SetFloat("horizontal", x);
        playerManager.characterAnimator.SetFloat("vertical", z);
        if (playerManager.newWeapon != null)
            playerManager.newWeapon.weaponAnimations.MovingSidesAnimation(x);
    }
    public void PlayCrouchAnimation(bool isCrouching)
    {
        if (isCrouching)
        {
            CrouchAnimation(true, playerManager.weaponParentPositionOnCrouching);
        }
        else
        {
            CrouchAnimation(false, Vector3.zero);
        }
    }
    public void PlayWalkAnimation(bool isWalking)
    {
        if (isWalking)
            playerManager.characterAnimator.SetBool("isWalking", isWalking);
        else
            playerManager.characterAnimator.SetBool("isWalking", isWalking);
    }
    public void PlayAimingAnimation(float angle, Quaternion localRot)
    {
        playerManager.weaponsparent.localRotation = localRot;
        playerManager.characterAnimator.SetFloat("aimAngle", angle);
    }
    public void Jumping(float velocity) => playerManager.characterAnimator.SetFloat("jump", velocity);

    /// <summary>
    /// Play network animations
    /// </summary>
    /// <param name="state"></param>
    public void PlayAnimationsDependingOnPlayerState(int state)
    {
        switch (state)
        {
            case (int)PlayerState.Crouching:
                playerManager.playerState = PlayerState.Crouching;
                PlayCrouchAnimation(true);
                PlayWalkAnimation(false);
                break;
            case (int)PlayerState.Walking:
                playerManager.playerState = PlayerState.Walking;
                PlayWalkAnimation(true);
                PlayCrouchAnimation(false);
                break;
            case (int)PlayerState.Running:
                playerManager.playerState = PlayerState.Running;
                PlayCrouchAnimation(false);
                PlayWalkAnimation(false);
                break;
        }
    }
    
    void CrouchAnimation(bool isPlaying, Vector3 pos)
    {
        playerManager.characterAnimator.SetBool("isCrouching", isPlaying);
        playerManager.weaponsparent.localPosition = pos;
    }
}

