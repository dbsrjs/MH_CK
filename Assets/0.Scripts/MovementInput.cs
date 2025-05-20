using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CharacterController 기반 이동 및 애니메이션 블렌딩 처리를 위한 스크립트.
/// Animator에는 다음 파라미터들이 필요하다:
/// - Blend (float): 이동 속도에 따라 애니메이션 전환
/// - InputX, InputZ (float): Blend Tree에서 방향별 애니메이션 처리
/// - onMove (bool): 이동 중 여부
/// - isJump, isRunJump (trigger): 점프 애니메이션
/// </summary>
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{

    public float Velocity; // 실제 이동 속도

    [Header("입력 관련 변수")]
    public float InputX; // 수평 입력
    public float InputZ; // 수직 입력
    public Vector3 desiredMoveDirection; // 카메라 기준으로 계산된 최종 이동 방향
    private bool blockRotationPlayer; // 플레이어 회전 고정 여부
    private float desiredRotationSpeed = 0.1f; // 플레이어가 회전하는 속도

    [Header("컴포넌트 참조")]
    public Animator anim; // 애니메이터
    public Camera cam; // 카메라 참조
    public CharacterController controller; // 캐릭터 컨트롤러
    private float Speed; // 현재 입력 벡터의 크기 (제곱)
    private float allowPlayerRotation = 0.1f; // 최소 입력 크기 (이거보다 작으면 정지 처리)
    private bool isGrounded; // 바닥에 닿아있는지 여부

    [Header("중력 처리")]
    private float verticalVel; // 수직 속도 (중력 반영)
    private Vector3 moveVector; // 최종 이동 벡터 (y값 포함)

    private bool isMove = false; // 이동 중인지 여부
    private float jumpPower = 6f;

    void Start()
    {
        cam = Camera.main;
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 점프 입력
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            Jump();

        InputMagnitude();

        // 중력 처리
        isGrounded = controller.isGrounded;
        if (isGrounded && verticalVel < 0)
            verticalVel = -2f;
        else
            verticalVel += -20f * Time.deltaTime;

        // 수직 이동 벡터 적용
        moveVector = new Vector3(0, verticalVel, 0);
        controller.Move(moveVector * Time.deltaTime);
    }

    /// <summary>
    /// 플레이어 이동 및 회전 처리
    /// </summary>
    void PlayerMoveAndRotation()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        // 카메라 기준으로 전후좌우 벡터 계산
        var forward = cam.transform.forward;
        var right = cam.transform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // 입력값을 카메라 방향에 맞게 변환
        desiredMoveDirection = forward * InputZ + right * InputX;

        // 회전 허용 시 캐릭터가 이동 방향을 바라보도록 회전
        if (!blockRotationPlayer)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(desiredMoveDirection),
                desiredRotationSpeed
            );

            // 이동
            controller.Move(desiredMoveDirection * Time.deltaTime * Velocity);
        }
    }

    /// <summary>
    /// 특정 위치를 바라보도록 회전
    /// </summary>
    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    /// <summary>
    /// 카메라가 바라보는 방향으로 회전
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
    /// 입력값에 따른 애니메이션 세팅 및 이동 실행
    /// </summary>
    void InputMagnitude()
    {
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(InputX, InputZ);
        isMove = moveInput.magnitude != 0;
        Speed = moveInput.sqrMagnitude; // 입력 강도 (제곱값)
        if (isMove) print("true");
        anim.SetBool("onMove", isMove); // 이동 애니메이션 처리용 bool
        // 이동 중이면 Blend 파라미터에 값 주고 실제 이동 실행
        if (Speed > allowPlayerRotation)
        {
            PlayerMoveAndRotation();
        }
    }

    /// <summary>
    /// 점프 애니메이션 트리거 실행
    /// 실제 점프 물리 처리는 구현되어 있지 않음
    /// </summary>
    private void Jump()
    {
        verticalVel = jumpPower; // 수직 속도에 점프력 적용

        if (anim.GetBool("onMove"))
            anim.SetTrigger("isRunJump");
        else
            anim.SetTrigger("isJump");
    }
}