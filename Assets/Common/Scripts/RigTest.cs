using UnityEngine;

public class RigTest : MonoBehaviour
{
    private float speed = 0;
    private float angle = 0;

    

    private void Update()
    {
        Animator anim = GetComponent<Animator>();


        Vector3 inputAxis = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        Vector3 worldInput = Vector3.ProjectOnPlane(Camera.main.transform.TransformDirection(inputAxis), transform.up);
        Vector3 localInput = transform.InverseTransformDirection(worldInput);
        Debug.Log(localInput);
        angle = Mathf.Lerp(angle, Mathf.Rad2Deg * Mathf.Atan2(localInput.x, localInput.z), Time.deltaTime*5);
        speed = Mathf.Lerp(speed, inputAxis.magnitude * (Input.GetKey(KeyCode.LeftShift) ? 1.2f : 4.5f), Time.deltaTime*5f);
        anim.SetFloat("Speed", speed);
        anim.SetFloat("Angle", angle);
    }
}
