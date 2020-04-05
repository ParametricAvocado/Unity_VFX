using UnityEngine;

public class Turnaround : MonoBehaviour
{
	[SerializeField] private float m_TurnaroundSpeed = 60f;

	private void LateUpdate() {
		transform.Rotate(Vector3.up, m_TurnaroundSpeed * Time.deltaTime, Space.Self);
	}
}
