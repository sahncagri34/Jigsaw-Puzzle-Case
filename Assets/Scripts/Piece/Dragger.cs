using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Dragger : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private Vector3 offset;
    public bool canMove;
    public event Action<Vector3> OnReleasedObject;
    public event Action OnGrabbedObject;
 
    public void SetDraggerActive()
    {
        canMove = true;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canMove)
            return;

        offset = transform.position - Input.mousePosition;
        OnGrabbedObject?.Invoke();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
         if (!canMove)
            return;

        OnReleasedObject?.Invoke(Input.mousePosition + offset);
    }
    public void OnDrag(PointerEventData eventData)
    {
         if (!canMove)
            return;

        transform.position = Input.mousePosition + offset;
    }

    
}
