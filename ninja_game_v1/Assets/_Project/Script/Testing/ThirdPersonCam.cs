using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCam : MonoBehaviour
{
    [Header("Rotate speed")]
    public float rotationSpeed = 1.0f;

    [Header("References")]
    public Transform orientation;
    //public Transform combatLookAt;
    public Transform playerObj;

    Transform camTr;
    Transform parentTransform;

    private void Start()
    {
        camTr = transform;
        parentTransform = transform.parent;

        if (playerObj == null)
            Debug.LogWarning("No target transform has been assigned to this script.", this);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LateUpdate()
    {
        /*
        Vector3 dirToCombatLookAt = combatLookAt.position - new Vector3(transform.position.x, combatLookAt.position.y, transform.position.z);

        orientation.forward = dirToCombatLookAt.normalized;

        playerObj.forward = dirToCombatLookAt.normalized;
        */
        if (!playerObj)
            return;

        //Calculate up and forward direction;
        Vector3 _forwardDirection = Vector3.ProjectOnPlane(camTr.forward, parentTransform.up).normalized;
        Vector3 _upDirection = parentTransform.up;

        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDir = orientation.forward * verticalInput + orientation.right * horizontalInput;


        orientation.rotation = Quaternion.Lerp(orientation.rotation, Quaternion.LookRotation(_forwardDirection, _upDirection), rotationSpeed * Time.deltaTime);

        if (inputDir != Vector3.zero)
        {
            //Set rotation;
            //playerObj.rotation = Quaternion.LookRotation(_forwardDirection, _upDirection);
            //orientation.rotation = Quaternion.LookRotation(_forwardDirection, _upDirection);
            playerObj.rotation = Quaternion.Lerp(playerObj.rotation, Quaternion.LookRotation(_forwardDirection, _upDirection), rotationSpeed * Time.deltaTime);
            
        }
            
    }
}
