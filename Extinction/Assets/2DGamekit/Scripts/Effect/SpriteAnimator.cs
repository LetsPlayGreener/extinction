using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAnimator : MonoBehaviour
{
    public List<Sprite> sprites;
    public List<Sprite> moving;
    public List<Sprite> animationOnce;
    private List<Sprite> readAnimation;
    private bool playingAnimationOnce = false;
    public float nbImagePerSeconds;
    private float fpsLoop;
    public float fpsMoving = -1;
    public float fpsAnimationOnce = -1;
    private float lastFrameChange = float.MinValue;
    private int selectedSprite = 0;
    private SpriteRenderer sprite;
    private bool initialOrientation;

    private Vector3 around;
    private Vector3 target;
    public float movingSpeed;
    public Vector2 range;

    private bool disableAfterAnimationOnce = false;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        around = transform.position;
        target = transform.position;
        initialOrientation = sprite.flipX;
        readAnimation = sprites;
        fpsLoop = nbImagePerSeconds;
        if (fpsMoving < 0)
            fpsMoving = nbImagePerSeconds;
        if (fpsAnimationOnce < 0)
            fpsAnimationOnce = nbImagePerSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playingAnimationOnce)
        {
            if (fpsLoop <= 0)
                fpsLoop = 0.0001f;
            if (Time.time - lastFrameChange > 1 / fpsLoop)
            {
                lastFrameChange = Time.time;
                selectedSprite++;
                if (selectedSprite >= readAnimation.Count)
                    selectedSprite = 0;
                sprite.sprite = readAnimation[selectedSprite];
            }
        }
        else
        {
            if (fpsAnimationOnce <= 0)
                fpsAnimationOnce = 0.0001f;
            if (Time.time - lastFrameChange > 1 / fpsAnimationOnce)
            {
                lastFrameChange = Time.time;
                selectedSprite++;
                if (selectedSprite < animationOnce.Count)
                    sprite.sprite = animationOnce[selectedSprite];
                else
                {
                    selectedSprite = 0;
                    sprite.sprite = readAnimation[selectedSprite];
                    lastFrameChange = Time.time;
                    playingAnimationOnce = false;
                    if (disableAfterAnimationOnce)
                        gameObject.SetActive(false);
                }
            }
        }

        if(range != Vector2.zero)
        {
            if (transform.position == target)
            {
                //find new target
                Vector2 random = new Vector2(Random.value * range.x * 2 - range.x, Random.value * range.y * 2 - range.y);
                target = around + new Vector3(random.x, random.y, 0);
                if (target.x > transform.position.x)
                    sprite.flipX = initialOrientation;
                else
                    sprite.flipX = !initialOrientation;
            }
            else
                transform.position = Vector3.MoveTowards(transform.position, target, movingSpeed * Time.deltaTime);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying)
            around = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(around, new Vector3(range.x * 2, range.y * 2, 0));
    }

    public void PlayAnimationOnce(bool disableAfter = false)
    {
        if(animationOnce != null && animationOnce.Count > 0)
        {
            selectedSprite = 0;
            sprite.sprite = animationOnce[selectedSprite];
            lastFrameChange = Time.time;
            playingAnimationOnce = true;
            disableAfterAnimationOnce = disableAfter;
        }
    }

    public void StartIdle()
    {
        readAnimation = sprites;
        fpsLoop = nbImagePerSeconds;
    }

    public void StartMoving()
    {
        readAnimation = moving;
        fpsLoop = fpsMoving;
    }
}
