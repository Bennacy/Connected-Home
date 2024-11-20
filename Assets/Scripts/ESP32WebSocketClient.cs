using UnityEngine;
using WebSocketSharp;

public class WebSocketClient : MonoBehaviour
{
    private WebSocket ws;

    void Start()
    {
        // Connect to WebSocket server
        ws = new WebSocket("ws://10.72.137.113");  // Use your ESP32's IP here

        // Event handlers for WebSocket events
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

        // Connect to the WebSocket server
        ws.Connect();
    }

    void OnApplicationQuit()
    {
        // Close the WebSocket connection when the application quits
        if (ws != null && ws.IsOpen)
        {
            ws.Close();
        }
    }

    void Update()
    {
        // Send a message every frame or based on events
        if (ws != null && ws.IsOpen)
        {
            ws.Send("Hello from Unity!");
        }
    }
}
