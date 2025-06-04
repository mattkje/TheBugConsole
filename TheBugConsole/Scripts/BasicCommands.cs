using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

/**
 * BasicCommands class to handle basic commands like help, quit, echo, showFPS, and ping
 */
public class BasicCommands : MonoBehaviour
{
    public TMP_Text consoleOutput;

    void Start()
    {
        Debug.Log("BasicCommands initialized");
    }

    /***-
     * Output a message to the console
     */
    void Output(string message)
    {
        consoleOutput.text += "\n" + message;
    }

    /**
     * Show the help message
     */
    [Command("help", "Show this help message")]
    void Help(string[] args)
    {
        Output("Available commands:");
        // Get most important methods
        var methods = GetType()
            .GetMethods(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        foreach (var method in methods)
        {
            var commandAttribute =
                (CommandAttribute)method.GetCustomAttributes(typeof(CommandAttribute), false).FirstOrDefault();
            if (commandAttribute != null)
            {
                Output(commandAttribute.Name + ": " + commandAttribute.Description);
            }
        }
    }

    /**
     * Quit the application
     */
    [Command("quit", "Quit the application")]
    void Quit(string[] args)
    {
        Application.Quit();
    }

    /**
     * Echo the input
     */
    [Command("echo", "Echo the input")]
    void Echo(string[] args)
    {
        if (args.Length == 0)
        {
            Debug.Log("Usage: echo <message>");
        }
        else
        {
            Debug.Log(string.Join(" ", args));
        }
    }

    /**
     * Show the current frames per second
     */
    [Command("showFPS", "Show the current frames per second")]
    void ShowFPS(string[] args)
    {
        Debug.Log("FPS: " + (1.0f / Time.deltaTime));
    }

    /**
     * Ping the server
     */
    [Command("ping", "Ping the server")]
    void Ping(string[] args)
    {
        // TODO: Implement ping command
        Debug.Log("Pong!");
    }

    /**
     * Clear the console output
     */
    [Command("clear", "Clear the console output")]
    void Clear(string[] args)
    {
        consoleOutput.text = "";
    }
}