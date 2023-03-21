using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager>
{
    [SerializeField] private List<BasePanel> panels;

    private Stack<BasePanel> panelStack;

    private void Start() => Initilize();

    private void Initilize()
    {
        panelStack = new Stack<BasePanel>();
        PushPanel(UIPanelType.StartPanel);
    }

    public BasePanel PushPanel(UIPanelType panelType)
    {
        if (panelStack.Count > 0)
        {
            BasePanel currentPanel = panelStack.Peek();
            currentPanel.Hide();
        }

        BasePanel panelToPush = GetPanel(panelType);
        panelToPush.Show();

        panelStack.Push(panelToPush);

        return panelToPush;
    }
    public void PopPanel()
    {
        if (panelStack.Count <= 1) return;

        BasePanel panelToPop = panelStack.Pop();
        BasePanel currentPanel = panelStack.Peek();

        panelToPop.Hide();
        currentPanel.Show();
    }


    public BasePanel GetPanel(UIPanelType panelType)
    {
        var panel = panels.Find(x => x.uiPanelType == panelType);
        return panel;
    }
}
public enum UIPanelType
{
    None,
    StartPanel,
    LevelSelectionPanel,
    GamePanel,
    WinPanel
}
