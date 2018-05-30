using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class QuestionBlock2D : HittableBlock2D {
    public bool addCoin = true;
    public Sprite postHitSprite;
    public int maxHits = 1;
    SpriteRenderer rend;
    [SerializeField] int timesHit;
    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    public override void HitFromBelow()
    {
        base.HitFromBelow();
        if (addCoin)
        {
            PlatformerCharacter.main.AddCoin(1);
        }
        timesHit++;
        if(timesHit >= maxHits)
        {
            if(postHitSprite) rend.sprite = postHitSprite;
        }
    }
}
