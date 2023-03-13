using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public enum ZoomType { FOV, ZoomForward, ZoomLowerY}
public class CameraSystem : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera _virtualCamera;
    private Vector3 _followOffset;
    
    [Header("Zoom Type")]
    public ZoomType zoomType = ZoomType.ZoomForward;

    [Header("FOV Options")]
    [SerializeField] private float _targetFoV = 50f;
    [SerializeField] private float _minFoV = 10f;
    [SerializeField] private float _maxFoV = 50f;
    [SerializeField] private float _zoomSpeed = 10f;

    [Header("ZoomForward Options")]
    [SerializeField] private float _followOffsetMin = 20f;
    [SerializeField] private float _followOffsetMax = 200;

    [Header("ZoomLowerY Options")]
    [SerializeField] private float _followOffsetYMin= 10f;
    [SerializeField] private float _followOffsetYMax = 50f;

    [Tooltip("Active only on ZoomForward and ZoomLowerY")]
    [SerializeField] private float _zoomAmount = 3f;

    [Header("Base Cam Options")]
    [SerializeField] private float _moveSpeed = 120f;
    [SerializeField] private float _rotateSpeed = 450f;

    [Header("DragPan")]
    [SerializeField] private bool _useDragPan = false;
    [SerializeField] private float _dragPanSpeed = 2f;
    private bool _dragPanActive = true;
    private Vector2 _lastMousePos;

    [Header("EdgeScrolling")]
    [SerializeField] private bool _useEdgeScrolling = false;
    [SerializeField] private int _edgeScrollSize = 10;

    [SerializeField] private bool _isZoomActive;
    private void Awake()
    {
        _followOffset = _virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
    }
    private void Update()
    {
        CameraMovement();

        if (_useEdgeScrolling)
        {
            CameraMovementEdgeScrolling();
        }

        if (_useDragPan)
        {
            CameraMovementDragPan();
        }

        CameraRotation();

        if (_isZoomActive)
        {
            switch(zoomType)
            {
                case ZoomType.ZoomForward:CameraZoomMoveForward();
                    break;

                case ZoomType.ZoomLowerY:CameraZoomLowerY();
                    break;

                case ZoomType.FOV:CameraZoomFoV();
                    break;
            }
        }
    }
    

    private void CameraMovement()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            inputDir.z = -1f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            inputDir.z = +1f;
        }

        if (Input.GetKey(KeyCode.A))
        {
            inputDir.x = -1f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            inputDir.x = +1f;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void CameraRotation()
    {
        float rotateDir = 0f;

        if (Input.GetKey(KeyCode.E))
        {
            rotateDir = -1f;
        }

        if (Input.GetKey(KeyCode.Q))
        {
            rotateDir = +1f;
        }

        transform.eulerAngles += new Vector3(0, rotateDir * _rotateSpeed * Time.deltaTime, 0);
    }

    private void CameraMovementEdgeScrolling()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.mousePosition.x < _edgeScrollSize)
        {
            inputDir.x = -1;
        }

        if (Input.mousePosition.y < _edgeScrollSize)
        {
            inputDir.z = -1;
        }

        if (Input.mousePosition.x > Screen.width - _edgeScrollSize)
        {
            inputDir.x = +1;
        }

        if (Input.mousePosition.y > Screen.height - _edgeScrollSize)
        {
            inputDir.z = +1;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void CameraMovementDragPan()
    {
        Vector3 inputDir = Vector3.zero;

        if (Input.GetMouseButtonDown(1))
        {
            _dragPanActive = true;
            _lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1))
        {
            _dragPanActive = false;
        }

        if (_dragPanActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - _lastMousePos;

            inputDir.x = mouseMovementDelta.x * _dragPanSpeed;
            inputDir.z = mouseMovementDelta.y * _dragPanSpeed;

            _lastMousePos = Input.mousePosition;
        }

        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;

        transform.position += moveDir * _moveSpeed * Time.deltaTime;
    }

    private void CameraZoomFoV()
    {
        if ( Input.mouseScrollDelta.y > 0 )
        {
            _targetFoV -= 5;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            _targetFoV += 5;
        }

        _targetFoV = Mathf.Clamp(_targetFoV, _minFoV, _maxFoV);

        Mathf.Lerp(_virtualCamera.m_Lens.FieldOfView, _targetFoV, Time.deltaTime * _zoomSpeed);
        _virtualCamera.m_Lens.FieldOfView = _targetFoV;
    }

    private void CameraZoomMoveForward()
    {
        Vector3 zoomDir = _followOffset.normalized;
        

        if (Input.mouseScrollDelta.y > 0)
        {
            _followOffset -= zoomDir * _zoomAmount;

        }

        if (Input.mouseScrollDelta.y < 0)
        {
            _followOffset += zoomDir * _zoomAmount;

        }

        if(_followOffset.magnitude < _followOffsetMin)
        {
            _followOffset = zoomDir * _followOffsetMin;
        }

        if (_followOffset.magnitude > _followOffsetMax)
        {
            _followOffset = zoomDir * _followOffsetMax;
        }
        
        _virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(_virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, _followOffset, Time.deltaTime * _zoomSpeed);

    }

    private void CameraZoomLowerY()
    {
        

        if (Input.mouseScrollDelta.y > 0)
        {
            _followOffset.y -= _zoomAmount;

        }

        if (Input.mouseScrollDelta.y < 0)
        {
            _followOffset.y += _zoomAmount;

        }

        _followOffset.y = Mathf.Clamp(_followOffset.y, _followOffsetYMin, _followOffsetYMax);

        

        _virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset = Vector3.Lerp(_virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, _followOffset, Time.deltaTime * _zoomSpeed);

    }

    public bool GetIsZoomActive()
    {
        return _isZoomActive;
    }

    public void SetIsZoomActive(bool active)
    {
        _isZoomActive = active;
    }

    public CinemachineVirtualCamera GetVirtualCamera()
    {
        return _virtualCamera;
    }

    public Vector3 GetFollowOffset()
    {
        return _followOffset;
    }
}
