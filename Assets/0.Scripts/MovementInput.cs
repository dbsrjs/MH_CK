using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterController ��� �̵� �� �ִϸ��̼� ���� ó���� ���� ��ũ��Ʈ.
/// Animator���� ���� �Ķ���͵��� �ʿ��ϴ�:
/// - Blend (float): �̵� �ӵ��� ���� �ִϸ��̼� ��ȯ
/// - InputX, InputZ (float): Blend Tree���� ���⺰ �ִϸ��̼� ó��
/// - onMove (bool): �̵� �� ����
/// - isJump, isRunJump (trigger): ���� �ִϸ��̼�
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{

    public float Velocity; // ���� �̵� �ӵ�

    [Header("�Է� ���� ����")]
    public float InputX; // ���� �Է�
    public float InputZ; // ���� �Է�
    public Vector3 desiredMoveDirection; // ī�޶� �������� ���� ���� �̵� ����
    private bool blockRotationPlayer; // �÷��̾� ȸ�� ���� ����
    private float desiredRotationSpeed = 0.1f; // �÷��̾ ȸ���ϴ� �ӵ�

    [Header("������Ʈ ����")]
    public Animator anim; // �ִϸ�����
    public Camera cam; // ī�޶� ����
    public CharacterController controller; // ĳ���� ��Ʈ�ѷ�
    private float Speed; // ���� �Է� ������ ũ�� (����)
    private float allowPlayerRotation = 0.1f; // �ּ� �Է� ũ�� (�̰ź��� ������ ���� ó��)
    private bool isGrounded; // �ٴڿ� ����ִ��� ����

    [Header("�߷� ó��")]
    private float verticalVel; // ���� �ӵ� (�߷� �ݿ�)
    private Vector3 moveVector; // ���� �̵� ���� (y�� ����)

    private bool isMove = false; // �̵� ������ ����
    private float jumpPower = 6f;

    void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // ���� �Է�
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        InputMagnitude();

        // �߷� ó��
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVel < 0)
            verticalVel = -2f;
        else
            verticalVel += -20f * Time.deltaTime;

        // ���� �̵� ���� ����
        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector * Time.deltaTime);
    }

    /// <summary>
    /// �÷��̾� �̵� �� ȸ�� ó��
    /// </summary>
    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        // ī�޶� �������� �����¿� ���� ���
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // �Է°��� ī�޶� ���⿡ �°� ��ȯ
        desiredMoveDirection = forward * InputZ + right * InputX;

        // ȸ�� ��� �� ĳ���Ͱ� �̵� ������ �ٶ󺸵��� ȸ��
        if (!blockRotationPlayer)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(desiredMoveDirection),
                desiredRotationSpeed
            );

            // �̵�
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        }
    }

    /// <summary>
    /// Ư�� ��ġ�� �ٶ󺸵��� ȸ��
    /// </summary>
    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    /// <summary>
    /// ī�޶� �ٶ󺸴� �������� ȸ��
    /// </summary>
    public void RotateToCamera(Transform t)
    {
        var forward = cam.transform.forward;
        forward.y = 0f;
        forward.Normalize();

        desiredMoveDirection = forward;

        t.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }

    /// <summary>
    /// �Է°��� ���� �ִϸ��̼� ���� �� �̵� ����
    /// </summary>
    void InputMagnitude()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(InputX, InputZ);
        isMove = moveInput.magnitude != 0;
        Speed = moveInput.sqrMagnitude; // �Է� ���� (������)
        if (isMove) print("true");
        anim.SetBool("onMove", isMove); // �̵� �ִϸ��̼� ó���� bool
        // �̵� ���̸� Blend �Ķ���Ϳ� �� �ְ� ���� �̵� ����
        if (Speed > allowPlayerRotation)
        {
            PlayerMoveAndRotation();
        }
    }

    /// <summary>
    /// ���� �ִϸ��̼� Ʈ���� ����
    /// ���� ���� ���� ó���� �����Ǿ� ���� ����
    /// </summary>
    private void Jump()
    {
        verticalVel = jumpPower; // ���� �ӵ��� ������ ����

        if (anim.GetBool("onMove"))
            anim.SetTrigger("isRunJump");
        else
            anim.SetTrigger("isJump");
    }
}