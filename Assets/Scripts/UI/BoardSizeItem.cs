using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardSizeItem : MonoBehaviour
{
    [SerializeField] private Image selectedImage;
    [SerializeField] private Button selectButton;
    [SerializeField] private TextMeshProUGUI sizeText;

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
        selectedImage.gameObject.SetActive(isSelected);
    }
}
