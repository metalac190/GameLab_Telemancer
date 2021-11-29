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
            if(frameCount <= 15)
                currentScale = 0f;
            else if(frameCount <= 25)
                currentScale = 0.25f;
            else if(frameCount <= 35)
                currentScale = 0.5f;
            else if(frameCount <= 45)
                currentScale = 0.75f;
            else if(frameCount >= 55)
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
}