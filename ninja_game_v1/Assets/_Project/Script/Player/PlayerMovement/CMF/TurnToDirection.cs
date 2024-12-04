using CMF;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnToDirection : MonoBehaviour
{
    public Transform targetTransform;
    Transform tr;
    Transform parentTransform;

    public CharacterInput characterInput;

    //Setup;
    void Start()
    {
        tr = transform;
        parentTransform = transform.parent;

        if (targetTransform == null)
            Debug.LogWarning("No target transform has been assigned to this script.", this);
    }

    //Update;
    void LateUpdate()
    {

        if (!targetTransform)
            return;

        //Calculate up and forward direction;
        Vector3 _forwardDirection = Vector3.ProjectOnPlane(targetTransform.forward, parentTransform.up).normalized;
        Vector3 _upDirection = parentTransform.up;

        float horizontalInput =  characterInput.GetHorizontalMovementInput();
        float verticalInput = characterInput.GetVerticalMovementInput();
        Vector3 inputDir = targetTransform.forward *  verticalInput + targetTransform.right * horizontalInput;

        if (inputDir != Vector3.zero)
        {
            //Set rotation;
            tr.rotation = Quaternion.LookRotation(_forwardDirection, _upDirection);
        }
    }
}
