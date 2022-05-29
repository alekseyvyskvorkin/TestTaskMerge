using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CameraSettingsController : MonoBehaviour
{
    [SerializeField] private Tile _rightTile;

    private Camera _camera;

    private void Awake()
    {
        StartCoroutine(SetMaxScreenSize());
    }

    private IEnumerator SetMaxScreenSize()
    {
        _camera = Camera.main;
        float highestXViewportPoint = Camera.main.WorldToViewportPoint(new Vector3(0, _rightTile.transform.position.y, 0)).y;

        while (highestXViewportPoint > 0.8f)
        {
            highestXViewportPoint = Camera.main.WorldToViewportPoint(new Vector3(0, _rightTile.transform.position.y, 0)).y;
            _camera.orthographicSize += Time.deltaTime;
            yield return null;
        }

        _camera.DOOrthoSize(_camera.orthographicSize + 1f, 1f).SetSpeedBased();
    }
}
