using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void OnClick() { }
    public virtual void OnMove(Vector3 position) { }
    public virtual void OnPointerUp() { }
}
