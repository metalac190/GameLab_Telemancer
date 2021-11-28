using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class DynamicRes : MonoBehaviour
{
    public float updateRate = 1.0f;
    private float currentScale = 1.0f;
    private float deltaTimeChanged = 0.0f;
    private float frameCount = 0;

    public float SetDynamicResolutionScale()
    {
        deltaTimeChanged += Time.deltaTime;

        if (deltaTimeChanged >= updateRate)
        {
            // Do not update framerate when game is paused
            if (Time.timeScale == 1)
            {
                frameCount = (1 / Time.deltaTime);
            }

            // Lerp between min and max scale defined in HDRP settings
            // (Current Min is 50%)
            // currentScale = Mathf.InverseLerp(20, 60, frameCount);
            if(frameCount <= 20)
                currentScale = 0f;
            else if(frameCount <= 30)
                currentScale = 0.25f;
            else if(frameCount <= 40)
                currentScale = 0.5f;
            else if(frameCount <= 50)
                currentScale = 0.75f;
            else if(frameCount == 1f)
                currentScale = 1f;

            deltaTimeChanged = 0.0f;
        }
        return currentScale;
    }

    void Start()
    {
        // Binds the dynamic resolution policy defined above.
        DynamicResolutionHandler.SetDynamicResScaler(SetDynamicResolutionScale, DynamicResScalePolicyType.ReturnsMinMaxLerpFactor);
    }

    private IEnumerator GetFPS()
    {
        while (true)
        {
            if (Time.timeScale == 1)
            {
                yield return new WaitForSeconds(0.1f);
                frameCount = (1 / Time.deltaTime);
                Debug.Log(frameCount);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
}