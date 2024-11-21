using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        // Connect to WebSocket server
        ws = new WebSocket("ws://10.72.137.113");  // Use your ESP32's IP here

        // Add event handlers to log connection events
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket connection opened.");
        };

        ws.OnMessage += (sender, e) =>
        {
            Debug.Log("Message from server: " + e.Data);
        };

        ws.OnError += (sender, e) =>
        {
            Debug.LogError("WebSocket Error: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket connection closed.");
        };

        // Log before attempting to connect
        Debug.Log("Attempting to connect to WebSocket server...");

        // Attempt to connect to the WebSocket server
        ws.Connect();
    }

    void Update()
    {
        // Send a message if the WebSocket is open
        if (ws != null && ws.IsOpen)
        {
            // ws.Send("Hello from Unity!");
        }
    }

    void OnApplicationQuit()
    {
        // Close the WebSocket connection when the application quits
        if (ws != null && ws.IsOpen)
        {
            ws.Close();
        }
    }
}
