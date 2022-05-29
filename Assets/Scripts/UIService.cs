using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class UIService : MonoBehaviour
{
    public static UIService Instance { get; private set; }

    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private Image _coinImage;
    [SerializeField] private RectTransform[] _moveCoins;

    [SerializeField] private float _moveSpeedCoins = 50f;
    [SerializeField] private Vector3 _coinPunchScale = Vector3.one / 5;
    [SerializeField] private float _coinPunchDuration = 0.25f;
 
    private void Awake()
    {
        Instance = this;
    }

    public void AddScore(Vector2 coinStartPosition)
    {
        for (int i = 0; i < _moveCoins.Length; i++)
        {
            if (_moveCoins[i].gameObject.activeInHierarchy == false)
            {
                _moveCoins[i].position = coinStartPosition;
                _moveCoins[i].gameObject.SetActive(true);
                _moveCoins[i].transform.DOLocalMove(Vector3.zero, _moveSpeedCoins).SetSpeedBased()
                    .OnComplete(() => 
                    {
                        _moneyText.text = (int.Parse(_moneyText.text) + 1).ToString();
                        _moveCoins[i].gameObject.SetActive(false);
                        _coinImage.transform.DORewind();
                        _coinImage.transform.DOPunchScale(_coinPunchScale, _coinPunchDuration);
                    });
                break;
            }
        }
    }
}
