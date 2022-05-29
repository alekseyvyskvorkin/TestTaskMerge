using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    public static Interactable CurrentInteractable { get; set; }

    [SerializeField] private LayerMask _layerMask;

    private Camera _camera;

    private Touch _touch;
    private RaycastHit _hit;
    private Ray _ray;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            _touch = Input.GetTouch(0);
            _ray = _camera.ScreenPointToRay(_touch.position);

            OnBeganTouch();
            OnHoldTouch();
            OnCancelTouch();
        }
    }

    private void OnBeganTouch()
    {
        if (_touch.phase == TouchPhase.Began && Physics.Raycast(_ray, out _hit))
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return;
            }
            if (_hit.collider != null && _hit.collider.TryGetComponent<Interactable>(out var interactable))
            {
                CurrentInteractable = interactable;
                CurrentInteractable.OnClick();
            }
        }
    }

    private void OnHoldTouch()
    {
        if (_touch.phase == TouchPhase.Moved || _touch.phase == TouchPhase.Stationary)
        {
            if (Physics.Raycast(_ray, out _hit, 1000f, _layerMask.value) && CurrentInteractable != null)
            {
                CurrentInteractable.OnMove(_hit.point);
            }
        }
    }

    private void OnCancelTouch()
    {
        if (_touch.phase == TouchPhase.Ended || _touch.phase == TouchPhase.Canceled)
        {
            if (CurrentInteractable != null)
            {
                CurrentInteractable.OnPointerUp();
            }
        }
    }
}
