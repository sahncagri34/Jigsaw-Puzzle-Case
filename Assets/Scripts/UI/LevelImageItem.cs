using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelImageItem : MonoBehaviour
{
    [SerializeField] private Image levelImage;
    [SerializeField] private Image selectedImage;
    [SerializeField] private Button selectButton;

    [HideInInspector] public Sprite LevelSprite;

    public event Action<LevelImageItem> OnSelected;
    public event Action<LevelImageItem> OnDestroyed;

    private void Awake()
    {
        selectButton.onClick.AddListener(OnButtonClicked);
    }
    private void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
    private void OnButtonClicked()
    {
        OnSelected?.Invoke(this);
    }
    
    public void Set(Sprite levelSprite)
    {
        LevelSprite = levelSprite;
        levelImage.sprite = levelSprite;
        selectedImage.sprite = levelSprite;
    }
    public void SetIfSelected(bool isSelected)
    {
        selectedImage.gameObject.SetActive(isSelected);
    }
}
