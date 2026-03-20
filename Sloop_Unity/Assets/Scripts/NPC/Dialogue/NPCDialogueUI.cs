using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sloop.NPC.Dialogue
{
    public class NPCDialogueUI : Sloop.UI.UIPanel
    {
        [Header("UI Refs")]
        [SerializeField] private TMP_Text titleText;
        [SerializeField] private TMP_Text bodyText;

        [Header("Main Buttons")]
        [SerializeField] private Button barkButton;
        [SerializeField] private Button interactButton;
        [SerializeField] private Button closeButton;

        [Header("Sub Choices")]
        [SerializeField] private GameObject subChoicesRoot;
        [SerializeField] private Button choiceAButton;
        [SerializeField] private TMP_Text choiceAText;
        [SerializeField] private Button choiceBButton;
        [SerializeField] private TMP_Text choiceBText;

        private Sloop.NPC.NPCController currentNPC;

        protected override void Awake()
        {
            base.Awake();
            Hide();
        }

        public void OpenForNPC(Sloop.NPC.NPCController npc)
        {
            currentNPC = npc;

            Sloop.UI.UIManager.Instance.OpenPanel(this);
            subChoicesRoot.SetActive(false);
            titleText.text = npc.GetDisplayName();
            bodyText.text = "...";
            SetupButtons();
        }

        private void SetupButtons()
        {
            barkButton.onClick.RemoveAllListeners();
            interactButton.onClick.RemoveAllListeners();
            closeButton.onClick.RemoveAllListeners();

            barkButton.onClick.AddListener(() => currentNPC.UI_BarkPressed(this));
            interactButton.onClick.AddListener(() => currentNPC.UI_InteractPressed(this));
            closeButton.onClick.AddListener(() => CloseDialogue());
        }

        public void CloseDialogue()
        {
            Sloop.UI.UIManager.Instance.CloseTopPanel();
        }

        public void SetLine(string text)
        {
            bodyText.text = text;
        }

        public void ShowChoices(string aText, System.Action aAction, string bText, System.Action bAction)
        {
            subChoicesRoot.SetActive(true);

            choiceAText.text = aText;
            choiceBText.text = bText;

            choiceAButton.onClick.RemoveAllListeners();
            choiceBButton.onClick.RemoveAllListeners();
            
            choiceAButton.onClick.AddListener(() => aAction?.Invoke());
            choiceBButton.onClick.AddListener(() => bAction?.Invoke());
        }

        public void HideChoices()
        {
            subChoicesRoot.SetActive(false);
        }

    }
}