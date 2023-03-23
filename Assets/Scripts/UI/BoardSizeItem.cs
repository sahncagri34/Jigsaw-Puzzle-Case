using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardSizeItem : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Text sizeText;

    [HideInInspector] public int boardSize;

    public event Action<BoardSizeItem> OnSelected;
    public event Action<BoardSizeItem> OnDestroyed;

    private void Awake() => selectButton.onClick.AddListener(OnButtonClicked);
    private void OnDestroy() => OnDestroyed?.Invoke(this);
    private void OnButtonClicked() => OnSelected?.Invoke(this);

    public void Set(int boardSize)
    {
        this.boardSize = boardSize;
        sizeText.text = boardSize.ToString();
    }
    public void SetIfSelected(bool isSelected)
    {
        var scaleFactor = isSelected == true ? 1.3f : 1f;
        transform.localScale = Vector3.one*scaleFactor;
    }
}
