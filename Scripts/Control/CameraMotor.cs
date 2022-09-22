using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    public Transform playerTargetPosition;
    private Vector3 cameraStartPosition = new Vector3(0, 7.0f, -7.0f);
    private Vector3 cameraStartRotation = new Vector3(15.0f, 0, 0);

    private Vector3 playerPositionOffset = new Vector3(0, 7.0f, -7.0f);
    private Vector3 playerRotationOffset = new Vector3(15.0f, 0, 0);
    public bool IsPlayerMoving { set; get; }

    // Start is called before the first frame update
    void Start()
    {
        //transform.position = playerTargetPosition.position + playerPositionOffset;
        transform.position = cameraStartPosition;
        transform.rotation = Quaternion.Euler(cameraStartRotation);
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (!IsPlayerMoving)
            return;

        Vector3 desiredPosition = playerTargetPosition.position + playerPositionOffset;
        //desiredPosition.x = 0;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * 4.0f);
        //transform.rotation = Quaternion.Lerp(transform.rotation, playerRotationOffset, Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(playerRotationOffset), 1.0f);
    }
}
