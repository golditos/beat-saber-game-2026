using UnityEngine;

public class AnimPlay : MonoBehaviour
{
    public AnimationClip clip;
    public Animation anim;
    public string clipName = "Walk";
    
    private void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animation>();
    }

    private void Start()
    {
        if (anim == null || clip == null) return;
        if (anim.GetClip(clipName) == null)
            anim.AddClip(clip, clipName);
        anim.Stop();
        anim.Play(clipName);
    }

    public void PlayClip()
    {
        if (anim != null && anim.GetClip(clipName) != null)
            anim.Play(clipName);
    }

    public void StopClip()
    {
        anim?.Stop();
    }
    
    public bool IsPlaying => anim != null && anim.isPlaying;
}
