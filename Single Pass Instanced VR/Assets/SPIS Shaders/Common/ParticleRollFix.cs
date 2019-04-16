using UnityEngine;

public class ParticleRollFix : MonoBehaviour
{
    public enum BillboardMode
    {
        Default,
        View,
        Facing
    }

    [SerializeField]
    private Camera m_Camera = null;

    [SerializeField]
    private BillboardMode m_BillboardMode = BillboardMode.View;

    private ParticleSystem m_ParticleSystem;

    private ParticleSystem.Particle[] particleBuffer;

    private void Awake()
    {
        if (!m_Camera)
        {
            if (Camera.main)
            {
                m_Camera = Camera.main;
            }
            else
            {
                Debug.LogError("Camera not assigned. Default Main Camera not found either. This script will be ignored.", this);
                enabled = false;
                return;
            }
        }

        m_ParticleSystem = GetComponent<ParticleSystem>();

        if (!m_ParticleSystem)
        {
            Debug.LogError("Particle System not found. This script will be ignored.", this);
            enabled = false;
            return;
        }

        particleBuffer = new ParticleSystem.Particle[m_ParticleSystem.main.maxParticles];
    }



    private void AlignParticles()
    {
        int aliveParticles = m_ParticleSystem.GetParticles(particleBuffer);

        switch (m_BillboardMode)
        {
            case BillboardMode.View:
                Vector3 cameraRotation = m_Camera.transform.rotation.eulerAngles;
                cameraRotation.x = -cameraRotation.x;
                cameraRotation.y = -cameraRotation.y;

                for (int i = 0; i < aliveParticles; i++)
                {
                    cameraRotation.z = particleBuffer[i].rotation3D.z;
                    particleBuffer[i].rotation3D = cameraRotation;
                }

                break;
            case BillboardMode.Facing:
                var simulationSpace = m_ParticleSystem.main.simulationSpace;
                Vector3 cameraPosition = m_Camera.transform.position;
                Vector3 particlePosition;
                for (int i = 0; i < aliveParticles; i++)
                {
                    particlePosition = TransformParticlePositionToWorldSpace(particleBuffer[i].position, m_ParticleSystem);

                    Vector3 facingRotation = Quaternion.LookRotation(particlePosition - m_Camera.transform.position).eulerAngles;
                    facingRotation.x = -facingRotation.x;
                    facingRotation.y = -facingRotation.y;
                    particleBuffer[i].rotation3D = facingRotation;
                }
                break;
        }
        m_ParticleSystem.SetParticles(particleBuffer, aliveParticles);
    }

    private void LateUpdate()
    {
        if (m_BillboardMode != BillboardMode.Default)
        {
            AlignParticles();
        }
    }

    private static Vector3 TransformParticlePositionToWorldSpace(Vector3 position, ParticleSystem particleSystem)
    {
        if (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Local)
        {
            return particleSystem.transform.TransformPoint(position);
        }
        else if (particleSystem.main.simulationSpace == ParticleSystemSimulationSpace.Custom)
        {
            return particleSystem.main.customSimulationSpace.TransformPoint(position);
        }
        return position;
    }
}
