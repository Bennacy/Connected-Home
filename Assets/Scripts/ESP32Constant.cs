using System.Collections;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class ESP32Constant : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    // private string ipAddress = "192.168.68.118"; //! IP when testing at  home
    private string ipAddress = "10.36.228.171"; //! IP when testing at  IADE
    private int port = 80;
    private AccelerometerData accelerometerData;
    private string buffer = "";

    [SerializeField] private bool connected = false;

    [SerializeField]private float timeSinceLastCommunication = 0;

    [SerializeField]private GameObject successMessage;
    [SerializeField]private GameObject failMessage;
    [SerializeField]private GameObject disconnectMessage;

    void Start()
    {
        accelerometerData = GetComponent<AccelerometerData>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.C)){
            if(!connected)
                ConnectToArduino();
            else
                Disconnect();
        }

        if(!connected)
            return;

        if(timeSinceLastCommunication > 2){
            StartCoroutine(ShowScreenMessage(failMessage, 10));
        }

        timeSinceLastCommunication += Time.deltaTime;

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
                //! Extract one message
                string message = buffer.Substring(0, newlineIndex).Trim();
                //! Remove processed message from the buffer
                buffer = buffer.Substring(newlineIndex + 1);

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
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            connected = true;
            accelerometerData.liveTesting = true;
            Debug.Log("Connected to ESP32");
            StartCoroutine(ShowScreenMessage(successMessage));
        }
        catch (System.Exception e)
        {
            Debug.LogError("Connection error: " + e.Message);
            StartCoroutine(ShowScreenMessage(failMessage));
        }
    }

    void Disconnect(){
        connected = false;
        if (stream != null) stream.Close();
        if (client != null) client.Close();
        accelerometerData.liveTesting = false;
        StartCoroutine(ShowScreenMessage(disconnectMessage));
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    private IEnumerator ShowScreenMessage(GameObject message, int time = 2)
    {
        message.SetActive(true);
        yield return new WaitForSeconds(time);
        message.SetActive(false);
    }
}
