using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HealthbarPlayer: Healthbar
{
    private CanvasGroup canvasGroup;
    public bool show = false;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    protected override void Update() {
        base.Update();
        canvasGroup.alpha = Mathf.Clamp(
            canvasGroup.alpha + (show ? 0.8f : -0.5f) * Time.deltaTime,
            0f,
            1f
        );
    }
}
