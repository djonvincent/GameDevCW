using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class Healthbar: MonoBehaviour
{
    public GameObject fill;
    [Range(0,1)]
    [SerializeField]
    private float _health;
    public float health {
        get {
            return _health;
        }

        set {
            fill.transform.localScale = new Vector3(Mathf.Clamp(health,0,1), 1, 1);
            _health = value;
        }
    }
}
