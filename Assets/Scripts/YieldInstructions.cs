using UnityEngine;

public class PausableWaitForSeconds : CustomYieldInstruction
{
    private float waited;
    private float duration;
    private GameManager GM;

    public PausableWaitForSeconds(float _duration) {
        duration = _duration;
        waited = 0f;
        GM = GameManager.instance;
    }

    public override bool keepWaiting {
        get {
            if (GM != null && !GM.paused) {
                waited += Time.deltaTime;
            }
            if (waited >= duration) {
                return false;
            }
            return true;
        }
    }
}
