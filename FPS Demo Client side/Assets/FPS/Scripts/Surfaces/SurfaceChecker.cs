using UnityEngine;

public class SurfaceChecker
{
    PlayerManager playerManager;

    public SurfaceChecker(PlayerManager playerManager) => this.playerManager = playerManager;

    public void CheckSurface()
    {
        if (playerManager.isDebugEnabled)
            Debug.DrawRay(playerManager.surfaceCheckPosition.position, -playerManager.surfaceCheckPosition.up * playerManager.checkDistance, Color.red);
        // ==============================================================================================================
        if (Physics.Raycast(playerManager.surfaceCheckPosition.position, -playerManager.surfaceCheckPosition.up, out RaycastHit hit, playerManager.checkDistance))
        {
            if (hit.transform != null)
            {
                Surface surface = hit.transform.GetComponent<Surface>();
                if (surface != null)
                    playerManager.currentSurface = surface.GetSurfaceType();
                else
                    playerManager.currentSurface = SurfaceTypes.NONE;

            }
        }
    }
}