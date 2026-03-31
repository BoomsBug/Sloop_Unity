using UnityEngine;
using UnityEngine.EventSystems;

namespace Sloop.UI
{
    public class GameButton : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
    {
        [SerializeField] private RectTransform target;
        [SerializeField] private float hoverScale = 1.05f;
        private Vector3 baseScale;

        private void Awake()
        {
            if (target==null) target = transform as RectTransform;

            baseScale = target.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            target.localScale = baseScale*hoverScale;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            target.localScale = baseScale;
        }
    }
}

