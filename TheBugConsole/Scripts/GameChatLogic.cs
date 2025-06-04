using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class GameChatLogic : MonoBehaviour
{
    [Tooltip("Singleton instance of the GameChatLogic class.")]
    public static GameChatLogic Instance { get; private set; }

    [Tooltip("Text component to display chat messages.")]
    public TMP_Text chatOutput;

    [Tooltip("Input field for entering chat messages.")]
    public TMP_InputField chatInput;

    [Tooltip("Toggle to enable or disable chat filtering.")]
    public bool filterChat = true;

    [Tooltip("List of banned words for chat filtering.")]
    private List<string> bannedWords = new List<string>();

    [Tooltip("History of chat messages.")] private readonly List<string> chatHistory = new List<string>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadBannedWords();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        chatInput.onEndEdit.AddListener(OnSubmitMessage);
    }

    void OnDestroy()
    {
        chatInput.onEndEdit.RemoveListener(OnSubmitMessage);
    }

    void OnSubmitMessage(string message)
    {
        if (!string.IsNullOrEmpty(message))
        {
            if (filterChat)
            {
                message = FilterBadWords(message);
            }

            chatInput.text = "";
            chatInput.ActivateInputField();

            // In a template, just add the message locally
            string username = GetUsername();
            string formattedMessage = $"{username}: {message}";
            AddMessageToChat(formattedMessage);
        }
    }

    private string FilterBadWords(string message)
    {
        foreach (var word in bannedWords)
        {
            if (message.Contains(word))
            {
                message = message.Replace(word, new string('*', word.Length));
            }
        }

        return message;
    }

    public void AddMessageToChat(string message)
    {
        chatHistory.Add(message);
        chatOutput.text += "\n" + message;
    }

    string GetUsername()
    {
        // Placeholder username for template
        return "Player";
    }

    public void ClearChat()
    {
        chatHistory.Clear();
        chatOutput.text = "";
    }

    private void LoadBannedWords()
    {
        // Example of loading banned words from a text asset in Unity
        var textAsset = UnityEngine.Resources.Load<TextAsset>("banned_words");
        if (textAsset != null)
        {
            using (var reader = new StringReader(textAsset.text))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    bannedWords.Add(line.Trim());
                }
            }
        }
        else
        {
            Debug.LogError("Banned words file not found.");
        }
    }
}