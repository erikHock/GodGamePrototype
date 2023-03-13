#define DEBUGGING
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemGrabber : MonoBehaviour
{
    public ItemGrabState currentState;
    public Action<ItemGrabState> OnItemGrabStateChanged;

    private Camera _camera;
    private BoxCollider _boxCollider;
    private Vector3 _hitPos;
    private Vector3 _grabbingPos;
    private bool isRayPositionEnabled = true;

    [SerializeField] private float _maxTimeInAir = 3f;
    private float flyTimer;

    [SerializeField] float _grabbingHeight = 10f;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _camera = Camera.main;
        currentState = ItemGrabState.Grounded;
        flyTimer = _maxTimeInAir;
    }
    private void Update()
    {
        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (isRayPositionEnabled)
            {
                _hitPos = hit.point;
                _grabbingPos = new Vector3(hit.point.x, _grabbingHeight, hit.point.z);
            }
            
            if (Input.GetMouseButtonDown(0) && isRayPositionEnabled)
            {
                // Set item to grounded
                if (currentState == ItemGrabState.Grabbed || currentState == ItemGrabState.Fly)
                {
                    if (!hit.collider.GetComponent<ItemGrabber>())   //Bug in this line
                    currentState = ItemGrabState.Grounded;
                }
                // If its clicked grab item
                else if (currentState == ItemGrabState.Grounded && hit.collider == _boxCollider)
                {
                    currentState = ItemGrabState.Grabbed;
                }
            }
        }

        // Update in State function
        switch (currentState)
        {
            case ItemGrabState.Grabbed:
                break;

            case ItemGrabState.Grounded:
                break;

            case ItemGrabState.Fly:
                transform.position = Vector3.Lerp(transform.position, _grabbingPos, Time.deltaTime * 3f);
                
                //If fly time is long && Mouse is not moving change state
                if (!IsMouseMoving())
                {
                    flyTimer -= Time.deltaTime;
                    if (flyTimer < 0)
                    {
                        currentState = ItemGrabState.Grounded;
                        flyTimer = _maxTimeInAir;
                    }
                }
                

                break;
        }

        OnItemGrabStateChanged?.Invoke(currentState);

       
    }

    private void OnEnable()
    {
        OnItemGrabStateChanged += SetItemPosition;
    }
    
    // Lerp transform on ChangeState event
    private void SetItemPosition(ItemGrabState state)
    {
        switch (state)
        {
            case ItemGrabState.Grabbed:
                if (IsGrounded())
                {
                    StartCoroutine(SetItemToDestination(1f, _grabbingPos));
                    currentState = ItemGrabState.Fly;
                }
                break;

            case ItemGrabState.Fly:
                break;

            case ItemGrabState.Grounded:
                if (!IsGrounded())
                {
                    StartCoroutine(SetItemToDestination(1f, _hitPos));
                }
                break;
        }
        
    }

    private bool IsMouseMoving()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            return true;
        }
        return false;
    }

    private bool IsGrounded()
    {
        float offsetHeight = 0.1f;
        bool isHitting = Physics.Raycast(_boxCollider.bounds.center, Vector3.down, _boxCollider.bounds.extents.y + offsetHeight);

#if DEBUGGING
        if (isHitting)
        {
            Debug.DrawRay(_boxCollider.bounds.center, Vector3.down * (_boxCollider.bounds.extents.y + offsetHeight), Color.green, 1f);
        }
        else
        {
            Debug.DrawRay(_boxCollider.bounds.center, Vector3.down * (_boxCollider.bounds.extents.y + offsetHeight), Color.red, 1f);
        }
#endif
        return isHitting;
    }

    private IEnumerator SetItemToDestination(float duration, Vector3 destination)
    {
        isRayPositionEnabled = false;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(transform.position, destination, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = destination;
        isRayPositionEnabled = true;
        yield return null;
    }

    private bool IsAboveGround(float rayY, float minY)
    {
        bool y = rayY >= minY ? true : false;
        return y;
    }

}


public enum ItemGrabState
{
    Grabbed,
    Fly,
    Grounded
}
