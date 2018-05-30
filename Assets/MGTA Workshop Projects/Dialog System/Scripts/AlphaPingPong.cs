using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlphaPingPong : MonoBehaviour {

    public UnityEngine.UI.Text target;

    [Range(.1f, 2)]
    public float duration = 1;

    float alpha = 0;

    public float timer = 0;

    [Range(0, 1)]
    public float minimumAlpha = .5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        LerpAlpha();
	}

    void LerpAlpha()
    {
        timer += Time.deltaTime;
        if (timer >= 2) timer = 0;
        float lerp = Mathf.PingPong(timer, duration) / duration;
        alpha = Mathf.Lerp(minimumAlpha, 1, lerp);
        target.color = new Color(target.color.r, target.color.g, target.color.b, alpha);
    }
}
