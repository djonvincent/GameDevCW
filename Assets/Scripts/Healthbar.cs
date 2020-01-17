using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar: MonoBehaviour
{
    public Transform fill;
    [Range(0,1)]
    [SerializeField]
    private float _health = 1f;
    private float health_old = 1f;
    private float health_last_set = 0f;
    public float health {
        get {
            return _health;
        }

        set {
            if (value == _health) {
                return;
            }
            health_old = _health;
            health_last_set = Time.time;
            _health = Mathf.Clamp(value, 0, 1);
        }
    }

    protected virtual void Update() {
        if (_health != fill.localScale.x) {
            fill.localScale = new Vector3(
                Mathf.Lerp(
                    health_old, _health, (Time.time - health_last_set)/0.5f
                ),
                1,
                1
            );
        }
    }
}
