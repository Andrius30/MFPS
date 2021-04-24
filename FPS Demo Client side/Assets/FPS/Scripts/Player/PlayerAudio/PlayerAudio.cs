using UnityEngine;

[CreateAssetMenu(menuName = "Player settings/Player audio settings")]
public class PlayerAudio : ScriptableObject
{
    PlayerManager playerManager;

    [Header("Foot step sounds")]
    public AudioClip[] concreteClips;
    public AudioClip[] dirtClips;
    public AudioClip[] metalClips;
    public AudioClip[] woodClips;
    public AudioClip[] grassClips;
    [Space(10)]
    [Header("Jump/Land sounds")]
    public AudioClip[] jumpClips;
    public AudioClip[] landClips;
    [Space(10)]
    [Header("HeadShot sound")]
    public AudioClip[] headShotClips;

    public void Init(PlayerManager playerManager) => this.playerManager = playerManager;
    public void Play()
    {
        switch (playerManager.currentSurface)
        {
            case SurfaceTypes.Concrete:
                PlayFootstepAudio(concreteClips);
                break;
            case SurfaceTypes.Dirt:
                PlayFootstepAudio(dirtClips);
                break;
            case SurfaceTypes.Metal:
                PlayFootstepAudio(metalClips);
                break;
            case SurfaceTypes.Wood:
                PlayFootstepAudio(woodClips);
                break;
            case SurfaceTypes.Grass:
                PlayFootstepAudio(grassClips);
                break;
        }
    }
    public void PlayJumpAudio()
    {
        if (playerManager.playerSource)
            playerManager.playerSource.PlayOneShot(PlayRandomClip(jumpClips));
    }
    public void PlayLandAudio()
    {
        if (playerManager.playerSource)
            playerManager.playerSource.PlayOneShot(PlayRandomClip(landClips));
    }
    public void PlayHeadShot()
    {
        playerManager.playerSource.PlayOneShot(PlayRandomClip(headShotClips));
    }

    void PlayFootstepAudio(AudioClip[] clips) => playerManager.playerSource.PlayOneShot(PlayRandomClip(clips));
    AudioClip PlayRandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];

}
