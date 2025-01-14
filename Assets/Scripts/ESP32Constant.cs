using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ESP32Constant : MonoBehaviour
{
    TcpClient client;
    NetworkStream stream;
    string ipAddress = "192.168.68.118"; // Replace with your ESP32's IP address
    int port = 80;
    private AccelerometerData accelerometerData;

    public float timeSinceLastCommunication = 0;
    private bool connecting = false;

    void Start()
    {
        accelerometerData = GetComponent<AccelerometerData>();
        ConnectToArduino();
    }

    private string buffer = "";
    void Update()
    {
        timeSinceLastCommunication += Time.deltaTime;
        // if(timeSinceLastCommunication >= 1){
        //     ConnectToArduino();
        //     return;
        // }

        if (stream != null && stream.CanWrite)
        {
            string message = "Connected to Unity";
            byte[] data = Encoding.ASCII.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }

        if (stream != null && stream.DataAvailable)
        {
            byte[] bytes = new byte[1024];
            int bytesRead = stream.Read(bytes, 0, bytes.Length);
            buffer += Encoding.ASCII.GetString(bytes, 0, bytesRead);

            while (buffer.Contains("\n"))
            {
                int newlineIndex = buffer.IndexOf('\n');
                string message = buffer.Substring(0, newlineIndex).Trim(); // Extract one message
                buffer = buffer.Substring(newlineIndex + 1); // Remove processed message from the buffer

                timeSinceLastCommunication = 0;
                ParseData(message);
            }
        }
    }

    void ParseData(string data)
    {
        string[] parts = data.Split(new char[] { '\\', '/' });

        if (parts.Length == 3)
        {
            float value1 = float.Parse(parts[0]);
            float value2 = float.Parse(parts[1]);
            float value3 = float.Parse(parts[2]);

            accelerometerData.NewData(new Accelerations(value1, value2, value3));
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

    void ConnectToArduino(){
        print("Connecting to Arduino");
        if(connecting)
            return;

        connecting = true;
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            connecting = false;
            Debug.Log("Connected to ESP32");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
        }
    }
    

    void OnApplicationQuit()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
