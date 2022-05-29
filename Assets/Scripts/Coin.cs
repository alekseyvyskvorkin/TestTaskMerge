using UnityEngine;

public class Coin : Interactable
{
    [SerializeField] private ParticleSystem _ps;
    [SerializeField] private float _destroyTime = 0.5f;

    public override void OnClick()
    {
        GetComponent<BoxCollider>().enabled = false;
        InputController.CurrentInteractable = null;
        UIService.Instance.AddScore(Input.GetTouch(0).position);
        _ps.Play();
        Destroy(gameObject, _destroyTime);
    }
}
