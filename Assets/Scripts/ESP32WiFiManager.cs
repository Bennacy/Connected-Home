using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ESP32WiFiManager : MonoBehaviour
{
    public string accelerometerData = "";
    public float accelerometerX = 0;
    public float accelerometerY = 0;
    public float accelerometerZ = 0;
    private string esp32Url = "http://10.72.137.113"; // Replace with your ESP32 IP

    void Start()
    {
        // Test the connection by sending a GET request
        StartCoroutine(SendGetRequest());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            SendDataToESP32("Accelerometer");
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SendDataToESP32("ON");
        if(Input.GetKeyDown(KeyCode.Alpha2))
            SendDataToESP32("OFF");

    }

    void FixedUpdate()
    {
        SendDataToESP32("Accelerometer");
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
                accelerometerData = request.downloadHandler.text;
                char[] separator = {'\\', '/'};
                string[] separatedString = accelerometerData.Split(separator, 3, System.StringSplitOptions.None);
                accelerometerX = float.Parse(separatedString[0]);
                accelerometerY = float.Parse(separatedString[1]);
                accelerometerZ = float.Parse(separatedString[2]);
            }
            Debug.Log("POST Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("POST request failed: " + request.error);
        }
    }
}
