using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ESP32Constant : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    string ipAddress = "10.36.226.24"; // Replace with your ESP32's IP address
    int port = 80;
    float timer = 0;
    private AccelerometerData accelerometerData;

    void Start()
    {
        accelerometerData = GetComponent<AccelerometerData>();
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("Connected to ESP32");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
        }
    }

    private string buffer = "";
    void Update()
    {
        // timer += Time.deltaTime;
        if (stream != null && stream.CanWrite && timer >= 0.1f)
        {
            // timer = 0;
            string message = "Hello, ESP32!";
            byte[] data = Encoding.ASCII.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
            // Debug.Log("Message sent to ESP32");
        }

        if (stream != null && stream.DataAvailable)
        {
            byte[] bytes = new byte[1024];
            int bytesRead = stream.Read(bytes, 0, bytes.Length);
            buffer += Encoding.ASCII.GetString(bytes, 0, bytesRead);

            // print(buffer);
            // Process complete messages
            while (buffer.Contains("\n"))
            {
                int newlineIndex = buffer.IndexOf('\n');
                string message = buffer.Substring(0, newlineIndex).Trim(); // Extract one message
                buffer = buffer.Substring(newlineIndex + 1); // Remove processed message from the buffer

                ParseData(message);
            }

            // Parse the received data
            // ParseData(data);
            
            // byte[] buffer = new byte[1024];
            // int bytesRead = stream.Read(buffer, 0, buffer.Length);
            // string receivedData = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            // if(!receivedData.Contains('/'))
            //     return;
            // print(receivedData);
            // char[] separator = {'\\', '/'};
            // string[] separatedString = receivedData.Split(separator, 3, System.StringSplitOptions.None);
            // float accelerometerX = 0;
            // float accelerometerY = 0;
            // float accelerometerZ = 0;
            // bool a = float.TryParse(separatedString[0], out accelerometerX);
            // bool b = float.TryParse(separatedString[1], out accelerometerY);
            // bool c = float.TryParse(separatedString[2], out accelerometerZ);
            // if(a && b && c)
            //     accelerometerData.NewData(new Accelerations(accelerometerX, accelerometerY, accelerometerZ));
            // // Debug.Log("Response from ESP32: " + receivedData);
        }
    }

    void ParseData(string data)
    {
        // Example: Split data based on delimiters
        string[] parts = data.Split(new char[] { '\\', '/' });

        if (parts.Length == 3)
        {
            float value1 = float.Parse(parts[0]);
            float value2 = float.Parse(parts[1]);
            float value3 = float.Parse(parts[2]);

            accelerometerData.NewData(new Accelerations(value1, value2, value3));
            // SendData($"{value1}, {value2}, {value3}");
            // Debug.Log($"Received values: {value1}, {value2}, {value3}");
        }
        else
        {
            Debug.LogWarning("Invalid data format: " + data);
        }
    }
    

    void SendData(string message)
    {
        if (stream != null && stream.CanWrite)
        {
            byte[] data = Encoding.ASCII.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }
    }
    

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
