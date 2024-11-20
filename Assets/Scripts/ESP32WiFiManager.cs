using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ESP32WiFiManager : MonoBehaviour
{
    private string esp32Url = "http://10.72.137.113"; // Replace with your ESP32 IP

    void Start()
    {
        // Test the connection by sending a GET request
        StartCoroutine(SendGetRequest());
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
            SendDataToESP32("BRUH");
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SendDataToESP32("ON");
        if(Input.GetKeyDown(KeyCode.Alpha2))
            SendDataToESP32("OFF");
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
            Debug.Log("Response: " + request.downloadHandler.text);
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
            Debug.Log("POST Response: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("POST request failed: " + request.error);
        }
    }
}
