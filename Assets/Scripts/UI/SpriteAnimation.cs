using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class SpriteAnimation : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Image _targetImage;
    [SerializeField] private string _startName;
    [SerializeField] private int _startNum;
    [SerializeField] private int _endNum;
    [SerializeField] private float _changeTime;
    #endregion

    private float _deltaTime;
    private int _curIdx;

    private void Awake()
    {
        if (_targetImage == null)
            _targetImage = GetComponent<Image>();
        _curIdx = _startNum;
    }

    private void Update()
    {
        if (_deltaTime >= _changeTime)
        {
            _deltaTime = 0;
            ++_curIdx;
            if (_curIdx > _endNum)
                _curIdx = _startNum;
            _targetImage.sprite = AddressableManager.Instance.LoadSpriteSync(string.Concat(_startName, _curIdx));
        }
        else
        {
            _deltaTime += Time.deltaTime;
        }
    }
}
