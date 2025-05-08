using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Component")]
    [SerializeField]
    private Transform playerBody;
    [SerializeField]
    private Transform cameraArm;
    [SerializeField]
    private Rigidbody rb;

    private Animator animator;


    private float speed = 5f;
    private bool isMove = false;

    private void Awake()
    {
        animator = playerBody.GetComponent<Animator>();
    }

    void Update()
    {
        LookAround();
        Move();

        if(Input.GetKeyDown(KeyCode.Space))
            Jump();
    }

    private void Move()
    {
        Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        isMove = moveInput.magnitude != 0;
        animator.SetBool("onMove", isMove);

        if (isMove)
        {
            Vector3 lookForward = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
            Vector3 lookRight = new Vector3(cameraArm.right.x, 0f, cameraArm.right.z).normalized;
            Vector3 moveDir = lookForward * moveInput.y + lookRight * moveInput.x;

            playerBody.forward = moveDir;
            transform.position += moveDir * Time.deltaTime * 5f;
        }

        Debug.DrawRay(cameraArm.position, new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized, Color.red);
    }

    private void LookAround()
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        //각도 제한
        float x = camAngle.x - mouseDelta.y;
        if (x < 180f)
            x = Mathf.Clamp(x, -1f, 70f);
        else
            x = Mathf.Clamp(x, 335f, 361f);

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);
    }

    private void Jump()
    {
        if(animator.GetBool("onMove"))
            animator.SetTrigger("isRunJump");

        else
            animator.SetTrigger("isJump");

        rb.AddForce(Vector3.up * 4f, ForceMode.Impulse);
    }
}
