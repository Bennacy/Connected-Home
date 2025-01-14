using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Networking;
using System;


[System.Serializable]
public class Accelerations{
    public float X;
    public float Y;
    public float Z;

    public Accelerations(float x = 0, float y = 0, float z = 0){
        X = x;
        Y = y;
        Z = z;
    }
    public void NewValues(float x = 0, float y = 0, float z = 0){
        X = x;
        Y = y;
        Z = z;
    }
}


public class AccelerometerData : MonoBehaviour
{
    public bool liveRecording;
    public string flaskIP = "http://127.0.0.1:5000/";
    public Accelerations currentAccelerometer;
    public List<Accelerations> accelerationsList;
    public int maxListSize = 15;
    string trainFileName = "";
    string predictFileName = "";
    private PlayerController playerController;

    public float flaskInterval = 0.5f;
    private float flaskTimer = 0;
    public bool canPost = true;
    public float postInterval = 2;
    private float postCooldown;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        accelerationsList = new List<Accelerations>();
        trainFileName = Application.dataPath + "/train.csv";
        predictFileName = Application.dataPath + "/predict.csv";
        postCooldown = postInterval;
        flaskTimer = flaskInterval;
    }

    void Update()
    {
        flaskTimer += Time.deltaTime;
        postCooldown += Time.deltaTime;
        
        if(flaskTimer >= flaskInterval && postCooldown >= postInterval && liveRecording){
            canPost = true;
            flaskTimer = 0;
            StartCoroutine(FlaskPost());
        }
        
        if(Input.GetKeyDown(KeyCode.UpArrow)){
            StartCoroutine(WriteTrainCSV("UP"));
        }
        if(Input.GetKeyDown(KeyCode.DownArrow)){
            StartCoroutine(WriteTrainCSV("DOWN"));
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow)){
            StartCoroutine(WriteTrainCSV("LEFT"));
        }
        if(Input.GetKeyDown(KeyCode.RightArrow)){
            StartCoroutine(WriteTrainCSV("RIGHT"));
        }

        if(Input.GetKeyDown(KeyCode.N)){
            StartCoroutine(WriteTrainCSV("NONE"));
        }

        if(Input.GetKeyDown(KeyCode.Space)){
            StartCoroutine(WritePredictCSV());
        }

        if(Input.GetKeyDown(KeyCode.R)){
            ResetCSV(predictFileName); // Reset prediction file
        }
        if(Input.GetKeyDown(KeyCode.T)){
            ResetCSV(trainFileName); // Reset training file
        }
    }

    private void ReceiveCommand(string command){
        if(command.Contains("UP")){
            playerController.ToggleLights(true);
        }else if(command.Contains("DOWN")){
            playerController.ToggleLights(false);
        }else if(command.Contains("LEFT")){
            playerController.CycleLightColor(-1);
        }else if(command.Contains("RIGHT")){
            playerController.CycleLightColor(1);
        }
    }

    private void ResetCSV(string fName)
    {
        string firstLine = "";
        if(fName == trainFileName){
            for(int i = 1; i <= maxListSize; i++){
                firstLine +=  $"X{i}, Y{i}, Z{i}, ";
            }
            firstLine += "Direction";
        }else{
            for(int i = 1; i <= maxListSize; i++){
                if(i <= maxListSize-1)
                    firstLine +=  $"X{i}, Y{i}, Z{i}, ";
                else
                    firstLine +=  $"X{i}, Y{i}, Z{i}";
            }
        }
        TextWriter tw = new StreamWriter(fName, false);
        tw.WriteLine(firstLine);
        tw.Close();
        print($"{fName} reset!");
    }

    public void NewData(Accelerations newAccel){
        accelerationsList.Add(newAccel);
        if(accelerationsList.Count > maxListSize)
            accelerationsList.Remove(accelerationsList[0]);
    }

    public IEnumerator WriteTrainCSV(string command){
        yield return new WaitForSeconds(1);
        if(accelerationsList.Count > 0){
            TextWriter tw = new StreamWriter(trainFileName, true);
            string data = "";

            for(int i = 0; i < accelerationsList.Count; i++){
                data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}, ";
            }
            data += command;
            tw.WriteLine(data);
            tw.Close();
            print("Training data saved");
        }
    }

    public IEnumerator WritePredictCSV(){
        yield return new WaitForSeconds(1);
        if(accelerationsList.Count > 0){
            TextWriter tw = new StreamWriter(predictFileName, true);
            string data = "";

            for(int i = 0; i < accelerationsList.Count; i++){
                if(i < accelerationsList.Count-1)
                    data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}, ";
                else
                    data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}";
            }
            
            tw.WriteLine(data);
            tw.Close();
            StartCoroutine(FlaskPost());
        }
    }

    public IEnumerator FlaskPost(){

        string data = "";
        for(int i = 0; i < accelerationsList.Count; i++){
            if(i < accelerationsList.Count-1)
                data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}, ";
            else
                data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}";
        }

        string features = "{\"features\":[";
        features += data;
        features += "]}";
        using (UnityWebRequest webRequest = UnityWebRequest.Post(flaskIP+"predictLogisticRegression", features, "application/json")){
            yield return webRequest.SendWebRequest();
            if(webRequest.result == UnityWebRequest.Result.Success){
                string response = webRequest.downloadHandler.text;

                if(!response.Contains("NONE") && ValidTest()){
                    ReceiveCommand(response);
                    canPost = false;
                    postCooldown = 0;
                    // lastResponse = "";
                }
            }
            if(webRequest.result == UnityWebRequest.Result.ConnectionError){
                print("Error");
            }
        }
    }

    private bool ValidTest(){
        float highestValue = Mathf.NegativeInfinity;
        Accelerations accel = accelerationsList[accelerationsList.Count-4];

        if(Mathf.Abs(accel.X) > highestValue)
            highestValue = Mathf.Abs(accel.X);
        if(Mathf.Abs(accel.Y) > highestValue)
            highestValue = Mathf.Abs(accel.Y);
        if(Mathf.Abs(accel.Z) > highestValue)
            highestValue = Mathf.Abs(accel.Z);

        // print(highestValue);

        return highestValue > 13;
    }
}
