using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;


public class ESP32WiFiManager : MonoBehaviour
{
    public Accelerations currentAccelerometer;
    private string esp32Url = "http://10.72.165.219"; // Replace with your ESP32 IP
    // private string esp32Url = "http://192.168.68.117"; // Replace with your ESP32 IP

    // public Velocities averageVelocities;
    // public List<Velocities> velocitiesList;
    public int maxListSize = 15;

    [Range(1, 10)]
    public int requestDelay = 1;
    private float requestCooldown;
    private AccelerometerData accelerometerData;

    void Start()
    {
        // Test the connection by sending a GET request
        StartCoroutine(SendGetRequest());
        accelerometerData = GetComponent<AccelerometerData>();
        currentAccelerometer = new Accelerations();
        // averageVelocities = new Velocities();
        requestCooldown = 0;
    }

    void Update()
    {
        requestCooldown += Time.deltaTime;
        if(requestCooldown >= (float)requestDelay/10){
            SendDataToESP32("Accelerometer");
            requestCooldown = 0;
        }
    }

    public void SendDataToESP32(string data)
    {
        StartCoroutine(SendPostRequest(data));
    }

    IEnumerator SendGetRequest()
    {
        UnityWebRequest request = UnityWebRequest.Get(esp32Url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Debug.Log("Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("GET request failed: " + request.error);
        }
    }

    IEnumerator SendPostRequest(string data)
    {
        WWWForm form = new WWWForm();
        form.AddField("data", data);

        UnityWebRequest request = UnityWebRequest.Post(esp32Url + "/data", form);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            if(data == "Accelerometer"){
                string receivedData = request.downloadHandler.text;
                char[] separator = {'\\', '/'};
                string[] separatedString = receivedData.Split(separator, 3, System.StringSplitOptions.None);
                float accelerometerX = float.Parse(separatedString[0]);
                float accelerometerY = float.Parse(separatedString[1]);
                float accelerometerZ = float.Parse(separatedString[2]);
                currentAccelerometer.NewValues(accelerometerX, accelerometerY, accelerometerZ);
                accelerometerData.NewData(new Accelerations(accelerometerX, accelerometerY, accelerometerZ));
            }
            // Debug.Log("POST Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("POST request failed: " + request.error);
        }
    }
}
