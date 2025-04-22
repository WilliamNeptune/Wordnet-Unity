using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
public class AddCustomWordPanel : MonoBehaviour
{
        public TMP_InputField inputField;
        private uint currentSynsetId;
        [SerializeField]
        private Button btnClose;
        [SerializeField]
        private Text promptText;
        void Start()
        {
            inputField.onEndEdit.AddListener(OnSubmitCustomWord);
            btnClose.onClick.AddListener(OnClickClose);
            Functions.setTextStrS(promptText, "ADD_CUSTOM_WORD_2", WordnetPanel.instance.getLemma());
            OpenKeyboard();
        }

        public void OnClickClose()
        {
            PanelManager.ClosePanelVertically (this.gameObject, null, 1000);
        }

        private void OnSubmitCustomWord(string text)
        {
            AddNewSynonym();
            OnClickClose();
        }

        public void init(uint synsetId)
        {
            currentSynsetId = synsetId;
        }
        private void AddNewSynonym()
        {
            if(inputField.text == "") return;
            ServerManager.instance.TryToAddCustomLemmaToSynset(inputField.text, currentSynsetId);
            WordnetPanel.instance.UpdateSynsetByIndex();
        }

        private void OpenKeyboard()
        {
            if (inputField != null)
            {
                // Activate the input field to focus it
                inputField.ActivateInputField();

                // Open the keyboard
                TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
            }
        }
}

