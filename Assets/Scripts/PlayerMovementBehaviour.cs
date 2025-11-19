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

    CharacterController controller;
    Animator animator;

    int itemLayer;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
        verticalSpeed = 0;
        sprintTimeLeft = sprintTimeMax;
        isWaitingSprintFullRecharge = false;
        itemLayer = LayerMask.NameToLayer("Item");
    }

    // Update is called once per frame
    void Update()
    {
        float upDown = (Input.GetKey(KeyCode.W) ? 1f : 0f) - (Input.GetKey(KeyCode.S) ? 1f : 0f);
        float leftRight = (Input.GetKey(KeyCode.D) ? 1f : 0f) - (Input.GetKey(KeyCode.A) ? 1f : 0f);
        bool isSprintPressed = Input.GetKey(KeyCode.LeftShift);
        bool isJumpPressed = Input.GetKey(KeyCode.Space);

        bool isGrounded = 0 == transform.position.y; // ignore controller.isGrounded, and just use y coordinate value.


        GameManager.Instance.GetCameraVector(out Vector3 cameraForward, out Vector3 cameraRight);
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

        GameManager.Instance.SetCamerasPosition(new Vector2(transform.position.x, transform.position.z));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (itemLayer == other.gameObject.layer)
        {
            GameObject prefab = GameManager.Instance.FindPrefabOfPooledGameObject(other.transform.parent.gameObject); // item collider is not at root!
            if (null != prefab && GameManager.Instance.itemDataMap.TryGetValue(prefab, out ItemData itemData) && null != itemData.readOnlyVfx)
            {
                GameObject vfx = Instantiate(itemData.readOnlyVfx, transform);
                if (null != vfx)
                {
                    ParticleSystem particleSystem = vfx.GetComponent<ParticleSystem>();
                    if (null != particleSystem)
                    {
                        var main = particleSystem.main;
                        main.startColor = Color.red;
                    }
                    Destroy(vfx, itemData.readOnlyVfxTime);
                }
            }
            GameManager.Instance.ReturnOrDestroyGameObject(other.gameObject);
        }
    }
}
