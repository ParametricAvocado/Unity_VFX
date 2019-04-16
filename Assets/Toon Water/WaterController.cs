using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class WaterController : MonoBehaviour {
    float time = 0;

    private void OnEnable()
    {
        time = 0;
        Shader.SetGlobalFloat("_WaterTime", 0);
    }

    void LateUpdate () {
        time += Time.deltaTime;
        Shader.SetGlobalFloat("_WaterTime", time);
    }
}
