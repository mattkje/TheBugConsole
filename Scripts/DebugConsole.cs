using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu.Tools.ThebugConsole.Scripts
{
    public class DebugConsole : MonoBehaviour
    {
        private static DebugConsole Instance { get; set; }
        public TMP_Text consoleOutput;
        public TMP_InputField consoleInput;
        public TMP_Text suggestionOutput;

        private readonly List<string> _commandHistory = new List<string>();
        private int _historyIndex = -1;
        private Dictionary<string, Action<string[]>> _commandHandlers;

        
        void Start()
        {
            consoleOutput.text = "";
            consoleInput.onEndEdit.AddListener(OnSubmitCommand);
    
            Application.logMessageReceived += HandleLog;
    
            WelcomeMessage();
            RegisterCommands();
        }

        private void OnDestroy()
        {
            // Unsubscribe from the event when the object is destroyed
            Application.logMessageReceived -= HandleLog;
        }

        private void HandleLog(string logString, string stackTrace, LogType type)
        {
            // Append the log message to the console output
            consoleOutput.text += $"\n[{type}] {logString}";
        }

        private void OnSubmitCommand(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                consoleOutput.text += "\n> " + input;
                _commandHistory.Add(input);
                _historyIndex = -1;

                ProcessCommand(input);

                consoleInput.text = "";
                consoleInput.ActivateInputField();
            }
        }

        private void ProcessCommand(string input)
        {
            string[] inputParts = input.Split(' ');
            string command = inputParts[0].ToLower();

            if (_commandHandlers.TryGetValue(command, out var handler))
            {
                handler(inputParts);
            }
            else
            {
                consoleOutput.text += "\nUnknown command: " + input;
            }
        }

        void RegisterCommands()
        {
            _commandHandlers = new Dictionary<string, Action<string[]>>();

            // Get all methods with the CommandAttribute in all loaded assemblies
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    foreach (var method in type.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
                    {
                        if (method.GetCustomAttribute(typeof(CommandAttribute)) is CommandAttribute attribute)
                        {
                            // Ensure the method has the correct parameter signature
                            var parameters = method.GetParameters();
                            if (parameters.Length != 1 || parameters[0].ParameterType != typeof(string[])) continue;
                            object instance = null;
                            if (typeof(MonoBehaviour).IsAssignableFrom(type))
                            {
                                // Find existing instance in the scene
                                instance = FindFirstObjectByType(type);
                                if (instance == null)
                                {
                                    // Ignoring if instance is not found. Not all commands are supposed to work everywhere anyway.
                                    continue;
                                }
                            }
                            else
                            {
                                instance = Activator.CreateInstance(type);
                            }

                            Action<string[]> action = (Action<string[]>)Delegate.CreateDelegate(typeof(Action<string[]>), method.IsStatic ? null : instance, method);
                            _commandHandlers.Add(attribute.Name.ToLower(), action);
                        }
                    }
                }
            }
        }
    
        void WelcomeMessage()
        {
            consoleOutput.text += "Welcome to the Thebug console!";
            consoleOutput.text += "\n\n";
            consoleOutput.text += "\nType 'help' to see available commands.";
        }
    

        void AutoCompleteCommand()
        {
            string currentInput = consoleInput.text;
            if (string.IsNullOrEmpty(currentInput))
            {
                suggestionOutput.text = "";
                return;
            }

            var matchingCommands = _commandHandlers.Keys
                .Where(cmd => cmd.StartsWith(currentInput.ToLower()))
                .ToList();

            if (matchingCommands.Count > 0)
            {
                suggestionOutput.text = string.Join("\n", matchingCommands);
            }
            else
            {
                suggestionOutput.text = "";
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (_commandHistory.Count > 0 && _historyIndex < _commandHistory.Count - 1)
                {
                    _historyIndex++;
                    consoleInput.text = _commandHistory[_commandHistory.Count - 1 - _historyIndex];
                    consoleInput.caretPosition = consoleInput.text.Length;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (_historyIndex > 0)
                {
                    _historyIndex--;
                    consoleInput.text = _commandHistory[_commandHistory.Count - 1 - _historyIndex];
                    consoleInput.caretPosition = consoleInput.text.Length;
                }
                else
                {
                    _historyIndex = -1;
                    consoleInput.text = "";
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                AutoCompleteCommand();
                var suggestions = suggestionOutput.text.Split('\n');
                if (suggestions.Length > 0)
                {
                    consoleInput.text = suggestions[0];
                    consoleInput.caretPosition = consoleInput.text.Length;
                }
            }
            else
            {
                AutoCompleteCommand();
            }
        }
    }
}