using UnityEngine;

namespace KarenKrill.UI.Views
{
    using Abstractions;
    using UnityEngine.EventSystems;

    [RequireComponent(typeof(CanvasGroup))]
    public abstract class ViewBehaviour : MonoBehaviour, IView
    {
        public bool Interactable { get => _canvasGroup.interactable; set => _canvasGroup.interactable = value; }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            SetFocus(true);
        }
        public virtual void Close()
        {
            SetFocus(false);
            gameObject.SetActive(false);
        }
        public virtual void SetFocus(bool isFocused)
        {
            if (EventSystem.current != null)
            {
                GameObject sellectedObject = isFocused ? DefaultFocusObject : null;
                EventSystem.current.SetSelectedGameObject(sellectedObject);
            }
        }

        [SerializeField]
        protected GameObject DefaultFocusObject;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }
    }
}