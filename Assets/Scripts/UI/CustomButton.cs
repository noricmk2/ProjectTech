using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler
{
    #region Inspector
    [SerializeField]
    private bool _interactable = true;
    [SerializeField]
    private Image _buttonImage;
    [SerializeField]
    private float _longClickTime = 1.0f;
    [SerializeField]
    private UnityEvent _onClickEvent;
    [SerializeField]
    private UnityEvent _longClickEvent;
    [SerializeField]
    private UnityEvent _onDownEvent;
    [SerializeField]
    private UnityEvent _onUpEvent;
    [SerializeField]
    private float _alphaThreshold = 0;

    #endregion
    [System.NonSerialized]
    public bool IsColorHilight;

    private bool _isLongTouch;
    private bool _LongTouchStart;
    private bool _isDown;
    private float _curTime;
    private IEnumerator _countEnumerator;

    private void Awake()
    {
        if (_buttonImage == null)
            _buttonImage = gameObject.GetComponent<Image>();
        if (_buttonImage != null)
            _buttonImage.alphaHitTestMinimumThreshold = _alphaThreshold;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_interactable)
        {
            if (_onClickEvent != null && !_LongTouchStart)
            {
                _onClickEvent.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _LongTouchStart = false;
        if (_interactable)
        {
            if (_onDownEvent != null)
                _onDownEvent.Invoke();

            if (_countEnumerator == null)
            {
                _countEnumerator = PointerDownCount_C();
                StartCoroutine(_countEnumerator);
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _curTime = 0;
        _isLongTouch = false;
        if (_countEnumerator != null)
        {
            StopCoroutine(_countEnumerator);
            _countEnumerator = null;
        }

        if (_interactable)
        {
            if (_onUpEvent != null)
                _onUpEvent.Invoke();
        }
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _curTime = 0;
        _isLongTouch = false;
        //SetIsDown(false, false);
        if (_countEnumerator != null)
        {
            StopCoroutine(_countEnumerator);
            _countEnumerator = null;
        }
    }

    IEnumerator PointerDownCount_C()
    {
        while (!_isLongTouch && _curTime < _longClickTime)
        {
            _curTime += Time.deltaTime;
            yield return null;
        }
        _LongTouchStart = true;
        _isLongTouch = true;
        _curTime = 0;
    }

    private void Update()
    {
        if (_isDown)
        {
            if (_longClickEvent != null && _isLongTouch)
                _longClickEvent.Invoke();
        }
    }

    public void SetInteract(bool enable)
    {
        _interactable = enable;
    }

    public void SetButtonImage(string imageName)
    {
        if (_buttonImage != null)
        {
            _buttonImage.sprite = AddressableManager.Instance.LoadSpriteSync(imageName);
        }
    }
}
