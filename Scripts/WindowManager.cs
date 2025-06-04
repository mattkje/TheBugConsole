using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.Tools.ThebugConsole.Scripts
{
    public class WindowManager : MonoBehaviour
    {
        public GameObject consolePrefab;  // Reference to the console prefab
        public GameObject chatPrefab;  // Reference to the chat prefab
        public KeyCode keyToToggleChat = KeyCode.T; // Key to toggle the chat
        public KeyCode keyToCloseConsole = KeyCode.K; 
        public GameObject pauseMenu;// Key to close the console
        public GameObject disconnectScreen;

        private GameObject _consoleCanvas;
        private TMP_InputField _consoleInputField;
        private GameObject _chatCanvas;
        private TMP_InputField _chatInputField;

        private void Start()
        {
            InitiateConsoles();
            DontDestroyOnLoad(gameObject);
            pauseMenu.SetActive(false);
            disconnectScreen.SetActive(false);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void InitiateConsoles()
        {
            _consoleCanvas = Instantiate(consolePrefab);
            _chatCanvas = Instantiate(chatPrefab);

            // Get references to the input fields
            _consoleInputField = _consoleCanvas.GetComponentInChildren<TMP_InputField>();
            _chatInputField = _chatCanvas.GetComponentInChildren<TMP_InputField>();

            // Subscribe to the onValidateInput event to restrict input
            _consoleInputField.onValidateInput += ValidateInput;
            _chatInputField.onValidateInput += ValidateInput;
        }

        private void Update()
        {
            if (_chatCanvas == null || _consoleCanvas == null) InitiateConsoles();
            
            // Check if the "T" key is pressed and the input field is not focused
            if (Input.GetKeyDown(keyToToggleChat) && !_chatCanvas.activeSelf && !_consoleCanvas.activeSelf)
            {
                ToggleChat();
            }

            // Check if the "K" key is pressed and the input field is not focused
            if (Input.GetKeyDown(keyToCloseConsole) && !_consoleCanvas.activeSelf && !_chatCanvas.activeSelf)
            {
                ToggleConsole();
            }

            // Handle the "Escape" key to close the console, but prevent it from being entered in the input field
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                
                if (_consoleCanvas.activeSelf)
                {
                    _consoleCanvas.SetActive(false);
                    _consoleInputField.DeactivateInputField();  // Deactivate the input field when closing
                }
                if (_chatCanvas.activeSelf)
                {
                    _chatCanvas.SetActive(false);
                    _chatInputField.DeactivateInputField();  // Deactivate the input field when closing
                }
            }
            
            if (!NetworkManager.Singleton.IsHost && !NetworkManager.Singleton.IsServer &&
                !NetworkManager.Singleton.IsClient) return;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseMenuLocked();
            }

            if (SceneManager.GetActiveScene().name == "MenuScene" && disconnectScreen.activeSelf)
            {
                disconnectScreen.SetActive(false);
            }
            
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ToggleConsole()
        {
            if (_consoleCanvas)
            {
                _consoleCanvas.SetActive(!_consoleCanvas.activeSelf);
                if (_consoleCanvas.activeSelf)
                {
                    _consoleInputField.ActivateInputField();  // Focus the input field when opening
                }
            }
        }

        private void ToggleChat()
        {
            if (NetworkManager.Singleton.ConnectedClients.Count < 1) return;
            if (_chatCanvas)
            {
                _chatCanvas.SetActive(!_chatCanvas.activeSelf);
                if (_chatCanvas.activeSelf)
                {
                    _chatInputField.ActivateInputField();  // Focus the input field when opening
                }
            }
            
        }
        
        public void TogglePauseMenu()
        {
            if (pauseMenu.activeSelf)
            {
                // close pause menu
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.SetActive(false);
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.SetActive(true);
            }
        }
        
        public void TogglePauseMenuLocked()
        {
            if (pauseMenu.activeSelf)
            {
                // close pause menu
                Cursor.lockState = CursorLockMode.Locked;
                pauseMenu.SetActive(false);
            }
            else
            {
                // open pause menu
                Cursor.lockState = CursorLockMode.None;
                pauseMenu.SetActive(true);
            }
        }

        // Custom validation function for allowed characters
        private char ValidateInput(string text, int charIndex, char addedChar)
        {
            // Define allowed characters
            string allowedChars = "abcdefghijklmnopqrstuvwxyzæøåABCDEFGHIJKLMNOPQRSTUVWXYZÆØÅ0123456789+-_/*!?() &%$#\".:,;";

            // Allow the character only if it's in the allowedChars string
            if (allowedChars.IndexOf(addedChar) != -1)
            {
                return addedChar;
            }

            // If the character is not allowed, return '\0' to ignore it
            return '\0';
        }

        public void ToggleDisconnectScreen()
        {
            disconnectScreen.SetActive(!disconnectScreen.activeSelf);
            Cursor.lockState = CursorLockMode.None;
            if (pauseMenu.activeSelf)
            {
                pauseMenu.SetActive(false);
            }
        }
    }
}