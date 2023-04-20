using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamShake : MonoBehaviour
{

    public static CamShake instance;

    private void Awake() => instance = this;


    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 origionalPos = transform.localPosition;

        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            float shakeNum = Random.Range(-1f, .2f) * magnitude;

            transform.localPosition = new Vector3(shakeNum, shakeNum, origionalPos.z);

            elapsed += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        transform.localPosition = origionalPos;
    }
}
