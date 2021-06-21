using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MovableRectTransform : MonoBehaviour, IDragHandler 
{
    [SerializeField]
    private float _leftLimit = -2.7f;

    [SerializeField]
    private float _rightLimit = 7.7f;

    private RectTransform _rectTransform;
    private Camera _mainCamera;

    private void Awake() 
    {
        _rectTransform = GetComponent<RectTransform>();
        _mainCamera = Camera.main;
    }
    
    public void OnDrag(PointerEventData eventData) 
    {
        Vector3 mousePosition = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, mousePosition, _mainCamera, out Vector2 resultPosition);
        bool isInsideLeftLimit = transform.TransformPoint(new Vector2(resultPosition.x, 0)).x >= _leftLimit;
        bool isInsideRightLimit = transform.TransformPoint(new Vector2(resultPosition.x, 0)).x <= _rightLimit;

        if (isInsideLeftLimit && isInsideRightLimit) 
        {
            transform.position = transform.TransformPoint(new Vector2(resultPosition.x, 0));
        }       
    }
}
