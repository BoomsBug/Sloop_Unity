using UnityEngine;

namespace Sloop.UI
{
    public class UIPanel : MonoBehaviour
    {
        [SerializeField] protected GameObject root;
        protected virtual void Awake()
        {
            if (root==null) root=gameObject;
        }
        
        public virtual void Show()
        {
            root.SetActive(true);
            OnShow();
        }

        public virtual void Hide()
        {
            root.SetActive(false);
            OnHide();
        }

        protected virtual void OnShow(){}
        protected virtual void OnHide(){}
    }
}
    
