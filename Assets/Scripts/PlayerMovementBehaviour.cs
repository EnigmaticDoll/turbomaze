using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementBehaviour : MonoBehaviour
{
    [SerializeField] private float speedJog;
    [SerializeField] private float speedSprint;
    [SerializeField] private float jumpInitialVerticalSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float sprintTimeMax;
    [SerializeField] private float sprintTimeRechargeCoefficient;

    private float verticalSpeed;
    private float sprintTimeLeft;
    private bool isWaitingSprintFullRecharge;

    SprintGaugeUI sprintUI;
    CharacterController controller;
    Animator animator;

    void Start()
    {
        GameObject uiHandler = GameObject.Find("UI Handler");
        if (null == uiHandler) Debug.LogWarning("uiHandler not found.");
        if (null != uiHandler) sprintUI = uiHandler.GetComponent<SprintGaugeUI>();
        if (null == sprintUI) Debug.LogWarning("sprintUI not found.");
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        verticalSpeed = 0;
        sprintTimeLeft = sprintTimeMax;
        isWaitingSprintFullRecharge = false;
    }

    // Update is called once per frame
    void Update()
    {
        float upDown = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        float leftRight = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        bool isSprintPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpPressed = Input.GetKey(KeyCode.Space);

        bool isGrounded = 0 == transform.position.y; // ignore controller.isGrounded, and just use y coordinate value.


        GameManager.Instance.GetMainCameraVector(out Vector3 cameraForward, out Vector3 cameraRight);
        Vector2 cameraPlaneForward = new Vector2(cameraForward.x, cameraForward.z).normalized;
        Vector2 cameraPlaneRight = new Vector2(cameraRight.x, cameraRight.z).normalized;
        Vector2 horizontalDirection = cameraPlaneForward * upDown + cameraPlaneRight * leftRight;


        if (sprintTimeLeft == 0f) isWaitingSprintFullRecharge = true;
        if (sprintTimeMax == sprintTimeLeft) isWaitingSprintFullRecharge = false;
        bool isSprinting = isSprintPressed && !isWaitingSprintFullRecharge && isGrounded && !isJumpPressed && (0 != horizontalDirection.magnitude);
        sprintTimeLeft = Mathf.Clamp(sprintTimeLeft + (isSprinting ? -Time.deltaTime : sprintTimeRechargeCoefficient * Time.deltaTime), 0f, sprintTimeMax);
        Vector2 horizontalVelocity = horizontalDirection * (isSprinting ? speedSprint : speedJog);


        if (isGrounded)
        {
            verticalSpeed = isJumpPressed ? jumpInitialVerticalSpeed : 0f;
        }
        else
        {
            verticalSpeed -= gravity * Time.deltaTime;
            verticalSpeed = Mathf.Max(verticalSpeed, -transform.position.y / Time.deltaTime);
        }

        if (0 != horizontalDirection.magnitude) transform.rotation = Quaternion.LookRotation(new Vector3(horizontalDirection.x, 0, horizontalDirection.y));
        controller.Move(new Vector3(horizontalVelocity.x, verticalSpeed, horizontalVelocity.y) * Time.deltaTime);

        int speedstep = (0 == horizontalDirection.magnitude) ? 0 : isSprinting ? 2 : 1;
        animator.SetInteger("speedstep", speedstep);
        animator.SetBool("isGrounded", isGrounded);

        if (null != sprintUI) sprintUI.SetSprintGauge(sprintTimeLeft, sprintTimeMax, isWaitingSprintFullRecharge);
        GameManager.Instance.SetCamerasPosition(new Vector2(transform.position.x, transform.position.z));
    }
}
