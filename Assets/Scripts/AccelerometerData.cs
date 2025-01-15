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
    public bool liveTesting = false;
    [SerializeField] private string flaskIP = "http://127.0.0.1:5000/";
    //! Essentially works as a stack, once it has reached a specific size new values push out the oldest ones
    [SerializeField] private List<Accelerations> accelerationsList;
    [SerializeField] private int maxListSize = 10;
    private string trainFileName = "";
    private string predictFileName = "";
    private PlayerController playerController;

    [SerializeField] private float flaskInterval = 0.5f;
    private float flaskCooldown = 0;
    //! Allows the user to move their arm back to a more normal position before testing for new gestures
    [SerializeField] private float postInterval = 2;
    private float postCooldown;

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        accelerationsList = new List<Accelerations>();
        trainFileName = Application.dataPath + "/train.csv";
        predictFileName = Application.dataPath + "/predict.csv";
        postCooldown = postInterval;
        flaskCooldown = flaskInterval;
    }

    void Update()
    {
        if(!liveTesting)
            return;
        
        flaskCooldown += Time.deltaTime;
        postCooldown += Time.deltaTime;
        
        if(flaskCooldown >= flaskInterval && postCooldown >= postInterval){
            flaskCooldown = 0;
            StartCoroutine(FlaskPost());
        }

        //! Not needed anymore, here for reference
        //! Controls used while recording/testing data for the machine learning algorithm
        // if(Input.GetKeyDown(KeyCode.UpArrow)){
        //     StartCoroutine(WriteTrainCSV("UP"));
        // }
        // if(Input.GetKeyDown(KeyCode.DownArrow)){
        //     StartCoroutine(WriteTrainCSV("DOWN"));
        // }
        // if(Input.GetKeyDown(KeyCode.LeftArrow)){
        //     StartCoroutine(WriteTrainCSV("LEFT"));
        // }
        // if(Input.GetKeyDown(KeyCode.RightArrow)){
        //     StartCoroutine(WriteTrainCSV("RIGHT"));
        // }
        // if(Input.GetKeyDown(KeyCode.N)){
        //     StartCoroutine(WriteTrainCSV("NONE"));
        // }
        // if(Input.GetKeyDown(KeyCode.Space)){
        //     StartCoroutine(WritePredictCSV());
        // }

        // if(Input.GetKeyDown(KeyCode.R)){
        //     ResetCSV(predictFileName); // Reset prediction file
        // }
        // if(Input.GetKeyDown(KeyCode.T)){
        //     ResetCSV(trainFileName); // Reset training file
        // }
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
            string data = TranslateData();
            
            tw.WriteLine(data);
            tw.Close();
            StartCoroutine(FlaskPost());
        }
    }

    public IEnumerator FlaskPost(){
        string data = TranslateData();
        string features = "{\"features\":[";
        features += data;
        features += "]}";
        using (UnityWebRequest webRequest = UnityWebRequest.Post(flaskIP+"predictLogisticRegression", features, "application/json")){
            yield return webRequest.SendWebRequest();
            if(webRequest.result == UnityWebRequest.Result.Success){
                string response = webRequest.downloadHandler.text;

                if(!response.Contains("NONE") && ValidTest(response)){
                    ReceiveCommand(response);
                    postCooldown = 0;
                }
            }
            if(webRequest.result == UnityWebRequest.Result.ConnectionError){
                print("Error");
            }
        }
    }

    private string TranslateData(){
        string data = "";

        for(int i = 0; i < accelerationsList.Count; i++){
            if(i < accelerationsList.Count-1)
                data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}, ";
            else
                data +=  $"{accelerationsList[i].X}, {accelerationsList[i].Y}, {accelerationsList[i].Z}";
        }

        return data;
    }

    private bool ValidTest(string response){
        float highestValue = Mathf.NegativeInfinity;
        //! Not using the most recent value, as that could trigger a response before the full movement is completed
        Accelerations accel = accelerationsList[accelerationsList.Count-4];

        if(Mathf.Abs(accel.X) > highestValue)
            highestValue = Mathf.Abs(accel.X);
        if(Mathf.Abs(accel.Y) > highestValue)
            highestValue = Mathf.Abs(accel.Y);
        if(Mathf.Abs(accel.Z) > highestValue)
            highestValue = Mathf.Abs(accel.Z);

        //! Testing if the strongest force being applied exceeds a threshold, to help with sensitivity
        return highestValue > 13;
        // return highestValue > (response.Contains("LEFT") || response.Contains("RIGHT") ? 12 : 13);
    }
}
