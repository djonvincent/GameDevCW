using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class HealthbarPlayer: Healthbar
{
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    public float maxHealth = 1f;
    public bool show = false;

    void Awake() {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    protected override void Update() {
        base.Update();
        rectTransform.sizeDelta = new Vector2(
            Mathf.Clamp(
                rectTransform.sizeDelta.x + 200f * Time.deltaTime,
                0f,
                maxHealth * 700f
            ),
            1
        );
        canvasGroup.alpha = Mathf.Clamp(
            canvasGroup.alpha + (show ? 0.8f : -0.5f) * Time.deltaTime,
            0f,
            1f
        );
    }
}
