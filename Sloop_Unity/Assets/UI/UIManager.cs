using System.Collections.Generic;
using UnityEngine;

namespace Sloop.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;
        private Stack<UIPanel> menuStack = new Stack<UIPanel>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void OpenPanel(UIPanel panel)
        {
            if (panel == null) return;

            if (menuStack.Count > 0)
                menuStack.Peek().Hide();
            menuStack.Push(panel);
            panel.Show();
        }

        public void CloseTopPanel()
       {
            if (menuStack.Count == 0)
                return;

            UIPanel top = menuStack.Pop();

            top.Hide();

            if (menuStack.Count > 0)
                menuStack.Peek().Show();
        }

        public void CloseAll()
        {
            while (menuStack.Count > 0)
            {
                menuStack.Pop().Hide();
            }
        }
    }
}