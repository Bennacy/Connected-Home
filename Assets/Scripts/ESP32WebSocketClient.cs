// using UnityEngine;
// using NativeWebSocket;

// public class ESP32WebSocketClient : MonoBehaviour
// {
//     private WebSocket websocket;

//     async void Start()
//     {
//         // Start WebSocket server
//         websocket = new WebSocket("ws://192.168.1.100:8080"); // Replace with Arduino's WebSocket IP and port

//         websocket.OnOpen += () =>
//         {
//             Debug.Log("Connection open!");
//         };

//         websocket.OnError += (e) =>
//         {
//             Debug.Log($"Error: {e}");
//         };

//         websocket.OnClose += (e) =>
//         {
//             Debug.Log("Connection closed!");
//         };

//         websocket.OnMessage += (bytes) =>
//         {
//             // Decode message from Arduino
//             string message = System.Text.Encoding.UTF8.GetString(bytes);
//             Debug.Log($"Message from Arduino: {message}");
//         };

//         await websocket.Connect();
//     }

//     void Update()
//     {
//         #if !UNITY_WEBGL || UNITY_EDITOR
//         websocket?.DispatchMessageQueue();
//         #endif
//     }

//     async void OnApplicationQuit()
//     {
//         if (websocket != null)
//         {
//             await websocket.Close();
//         }
//     }
// }
