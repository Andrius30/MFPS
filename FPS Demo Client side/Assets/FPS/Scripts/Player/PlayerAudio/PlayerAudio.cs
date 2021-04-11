using UnityEngine;

[CreateAssetMenu(menuName = "Player settings/Player audio settings")]
public class PlayerAudio : ScriptableObject
{
    PlayerManager playerManager;

    public AudioClip[] concreteClips;
    public AudioClip[] dirtClips;
    public AudioClip[] metalClips;
    public AudioClip[] woodClips;
    public AudioClip[] grassClips;

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
   
    void PlayFootstepAudio(AudioClip[] clips) => playerManager.playerSource.PlayOneShot(PlayRandomClip(clips));
    AudioClip PlayRandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];
}
