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
        PushPanel<StartPanel>();
    }

    public T PushPanel<T>() where T : BasePanel
    {
        if (panelStack.Count > 0)
        {
            BasePanel currentPanel = panelStack.Peek();
            currentPanel.Hide();
        }

        T panelToPush = GetPanel<T>();
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
    public T GetPanel<T>() where T : BasePanel
    {
        foreach (var panel in panels)
        {
            if (panel is T)
            {
                return (T)panel;
            }
        }
        return null;
    }




}

