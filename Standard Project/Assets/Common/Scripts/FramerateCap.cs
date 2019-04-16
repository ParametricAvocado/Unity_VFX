using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FramerateCap : MonoBehaviour
{
    [SerializeField]
    private int m_TargetFramerate = -1;
    private int defaultVSync;
    private int defaultFramerate;

    void OnEnable()
    {
        if (m_TargetFramerate != -1)
        {
            defaultVSync = QualitySettings.vSyncCount;
            defaultFramerate = Application.targetFrameRate;

            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = m_TargetFramerate;
        }
    }

    private void OnDisable()
    {
        if (m_TargetFramerate != -1)
        {
            QualitySettings.vSyncCount = defaultVSync;
            Application.targetFrameRate = defaultFramerate;
        }
    }
}
