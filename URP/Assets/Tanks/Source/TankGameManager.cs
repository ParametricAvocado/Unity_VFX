using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class TankGameManager : MonoBehaviour {
    [SerializeField] private GameObject[] m_PlayerPrefabs;
    [SerializeField] private Transform[] m_PlayerSpawns;
    [SerializeField] private CinemachineTargetGroup m_TargetGroup;
    
    
    void Start() {
        var p1 = PlayerInput.Instantiate(m_PlayerPrefabs[0], controlScheme :"Arrows", pairWithDevice: Keyboard.current);
        p1.transform.SetPositionAndRotation(m_PlayerSpawns[0].position, m_PlayerSpawns[0].rotation);
        var p2 = PlayerInput.Instantiate(m_PlayerPrefabs[1], controlScheme : "WASD", pairWithDevice: Keyboard.current);
        p2.transform.SetPositionAndRotation(m_PlayerSpawns[1].position, m_PlayerSpawns[1].rotation);
        
        m_TargetGroup.AddMember(p1.transform,1f,1f);
        m_TargetGroup.AddMember(p2.transform,1f,1f);
    }
}
