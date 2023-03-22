using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPanel : BasePanel
{
    [SerializeField] private Button playAgainButton;
    private void Awake() {
        playAgainButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    public override void Show()
    {
        base.Show();
    }
}
