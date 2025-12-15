using UnityEngine;

// simple looping animation with a spriteRenderer
public class SimpleAnimation : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Sprite[] frames;
    public float framesPerSecond;

    float timelastUpdated;
    int currentFrame = 0;

    void Start()
    {
        spriteRenderer.sprite = frames[0];
        timelastUpdated = Time.time;
    }

    void Update()
    {
        if ((Time.time - timelastUpdated) >= (1f/framesPerSecond))
        {
            timelastUpdated = Time.time;
            spriteRenderer.sprite = frames[currentFrame++];
            if (currentFrame >= frames.Length)
                currentFrame = 0;
        }
    }
}
