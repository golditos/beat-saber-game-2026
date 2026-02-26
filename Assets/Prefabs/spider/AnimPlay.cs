using UnityEngine;

public class AnimPlay : MonoBehaviour
{

    public AnimationClip clip;
    public Animation anim;

    void Start()
    {
        if(anim.isPlaying) return;
        
        anim.AddClip(clip, "Walk");
        anim.Stop();
        anim.Play("Walk");

        print(anim.clip);
    }
}
