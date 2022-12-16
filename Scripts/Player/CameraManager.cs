using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    InputManager inputManager;

    public Transform targetTransform; //object  camera follow my player
    public Transform cameraPivot; // camera use pivot (look 阿ㄆ and 盪) 
    public Transform cameraTransform;
    public LayerMask collisionLayers; //相機與 collide 碰撞
    private float defaultPosition;
    private Vector3 cameraFollowVelocity = Vector3.zero;

    private Vector3 cameraVectorPosition;

    public float cameraCollisionOffSet = 0.2f; //jump off of object
    public float minimumCollisionOffSet = 0.2f;
    public float cameraCollisionRadius = 2;
    public float cameraFollowSpeed = 0.2f;
    //可浮動
    public float cameraLookSpeed = 0.01f;
    public float cameraPivotSpeed = 0.01f;

    public float lookAngle; //Camera up and down
    public float pivotAngle;  //Camera left and right
    public float minimumPivotAngle = -35;
    public float maximumPivotAngle = 35;
  
    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        targetTransform = FindObjectOfType<PlayerManager>().transform;
        cameraTransform = Camera.main.transform;
        defaultPosition = cameraTransform.localPosition.z;
    }

    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        HandleCameraCollisions();

    }
    //跟隨目標
    private void FollowTarget()
    {   //呼叫者可以修改方法所傳回的值
        //更新位置
        Vector3 targetPosition = Vector3.SmoothDamp
            (transform.position, targetTransform.position, ref cameraFollowVelocity, cameraFollowSpeed);
        transform.position = targetPosition;

    }

    private void RotateCamera()
    {
        Vector3 rotation;
        Quaternion targetRotation;
        lookAngle = lookAngle + (inputManager.cameraInputX * cameraLookSpeed);
        pivotAngle = pivotAngle - (inputManager.cameraInputY * cameraPivotSpeed);
        //樞紐不超過值範圍
        pivotAngle = Mathf.Clamp(pivotAngle, minimumPivotAngle, maximumPivotAngle);

        rotation = Vector3.zero;
        rotation.y = lookAngle;
        targetRotation = Quaternion.Euler(rotation);
        //朝著旋轉方向旋轉
        transform.rotation = targetRotation;
        rotation = Vector3.zero;
        //樞軸
        rotation.x = pivotAngle;
        targetRotation = Quaternion.Euler(rotation);
        //局部旋轉
        cameraPivot.localRotation = targetRotation;

    }

    private void HandleCameraCollisions()
    {
        float targetPosition = defaultPosition;
        RaycastHit hit;
        Vector3 direction = cameraTransform.position - cameraPivot.position;
        direction.Normalize();

        if (Physics.SphereCast
            (cameraPivot.transform.position, cameraCollisionRadius, direction, out hit, Mathf.Abs(targetPosition), collisionLayers))
        {
            float distance = Vector3.Distance(cameraPivot.position, hit.point);
            targetPosition = -(distance - cameraCollisionOffSet);
        }
        if (Mathf.Abs(targetPosition) < minimumCollisionOffSet)
        {
            targetPosition = targetPosition - minimumCollisionOffSet;
        }

        cameraVectorPosition.z = Mathf.Lerp(cameraTransform.localPosition.z, targetPosition, 0.2f);
        cameraTransform.localPosition = cameraVectorPosition;
    }
    
}
