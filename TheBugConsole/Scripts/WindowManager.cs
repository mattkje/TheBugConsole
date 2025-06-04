using TMPro;
using UnityEngine;

namespace ThebugConsole.Scripts
{
    public class WindowManager : MonoBehaviour
    {
        public GameObject consolePrefab; // Reference to the console prefab
        public GameObject chatPrefab; // Reference to the chat prefab
        public KeyCode keyToToggleChat = KeyCode.T; // Key to toggle the chat
        public KeyCode keyToCloseConsole = KeyCode.K;

        private GameObject _consoleCanvas;
        private TMP_InputField _consoleInputField;
        private GameObject _chatCanvas;
        private TMP_InputField _chatInputField;

        private void Start()
        {
            InitiateConsoles();
            DontDestroyOnLoad(gameObject);
        }

        private void InitiateConsoles()
        {
            _consoleCanvas = Instantiate(consolePrefab);
            _chatCanvas = Instantiate(chatPrefab);

            _consoleInputField = _consoleCanvas.GetComponentInChildren<TMP_InputField>();
            _chatInputField = _chatCanvas.GetComponentInChildren<TMP_InputField>();

            _consoleInputField.onValidateInput += ValidateInput;
            _chatInputField.onValidateInput += ValidateInput;
        }

        private void Update()
        {
            if (_chatCanvas == null || _consoleCanvas == null) InitiateConsoles();

            if (Input.GetKeyDown(keyToToggleChat) && !_chatCanvas.activeSelf && !_consoleCanvas.activeSelf)
            {
                ToggleChat();
            }

            if (Input.GetKeyDown(keyToCloseConsole) && !_consoleCanvas.activeSelf && !_chatCanvas.activeSelf)
            {
                ToggleConsole();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (_consoleCanvas.activeSelf)
                {
                    _consoleCanvas.SetActive(false);
                    _consoleInputField.DeactivateInputField();
                }

                if (_chatCanvas.activeSelf)
                {
                    _chatCanvas.SetActive(false);
                    _chatInputField.DeactivateInputField();
                }
            }
        }

        private void ToggleConsole()
        {
            if (_consoleCanvas)
            {
                _consoleCanvas.SetActive(!_consoleCanvas.activeSelf);
                if (_consoleCanvas.activeSelf)
                {
                    _consoleInputField.ActivateInputField();
                }
            }
        }

        private void ToggleChat()
        {
            if (_chatCanvas)
            {
                _chatCanvas.SetActive(!_chatCanvas.activeSelf);
                if (_chatCanvas.activeSelf)
                {
                    _chatInputField.ActivateInputField();
                }
            }
        }

        // Custom validation function for allowed characters
        private char ValidateInput(string text, int charIndex, char addedChar)
        {
            string allowedChars =
                "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789+-_/*!?() &%$#\".:,;";
            if (allowedChars.IndexOf(addedChar) != -1)
            {
                return addedChar;
            }

            return '\0';
        }
    }
}