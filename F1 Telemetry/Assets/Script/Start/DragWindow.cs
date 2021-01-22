using UnityEngine;
using UnityEngine.EventSystems;

public class DragWindow : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] float _dragAlpha;
    [SerializeField] RectTransform _dragRectTransform;
    [SerializeField] CanvasGroup _dragCanvasGroup;

    Canvas _canvas;
    RectTransform _canvasRectTransform;

    float _startAlpha;

    void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _canvasRectTransform = _canvas.GetComponent<RectTransform>();
        _startAlpha = _dragCanvasGroup.alpha;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragCanvasGroup.alpha = _dragAlpha;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _dragCanvasGroup.alpha = _startAlpha;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 newPosition = _dragRectTransform.anchoredPosition + eventData.delta / _canvas.scaleFactor;
            float maxX = (_canvasRectTransform.rect.width / 2) - (_dragRectTransform.rect.width / 2 * _dragRectTransform.localScale.x);
            float maxY = (_canvasRectTransform.rect.height / 2) - (_dragRectTransform.rect.height / 2 * _dragRectTransform.localScale.y);

            newPosition.x = Mathf.Clamp(newPosition.x, -maxX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, -maxY, maxY);

            _dragRectTransform.anchoredPosition = newPosition;
        }    
    }
}
