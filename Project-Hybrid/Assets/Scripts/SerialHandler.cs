using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialHandler : MonoBehaviour
{
    [SerializeField] bool usePS4 = false;
    [SerializeField] float accStrenghtScalar = 0;

    public static float rotX, rotY, rotZ;
    public static float pitch, roll, yaw;
    public static float accX, accY, accZ;

    float ps4RotXOffset, ps4RotYOffset, ps4RotZOffset;

    SerialPort stream = null;

    bool isInitialized = false;

    public static bool doReset;
    public static bool usesPS4;

    void Start()
    {
        DontDestroyOnLoad(this);

        usesPS4 = usePS4;
        if (usePS4) return;
        isInitialized = false;
        StartThread();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) ReInit();

        if (usePS4)
        {
            accX = GyroInputTester.accX * accStrenghtScalar;
            accY = GyroInputTester.accY * accStrenghtScalar;
            accZ = GyroInputTester.accZ * accStrenghtScalar;
            rotX = GyroInputTester.rotX - ps4RotXOffset;
            rotY = GyroInputTester.rotY - ps4RotYOffset;
            rotZ = GyroInputTester.rotZ - ps4RotZOffset;

            return;
        }

        if (!isInitialized)
        {
            string temp = ReadFromESP();
            if (temp == null) return;
            if (temp.Equals("Initialize")) isInitialized = true;
            return;
        }

        string[] command = SplitOutput();
        if (command == null) return;
        if (command.Length < 9) return;

        rotX = float.Parse(command[0]);
        rotY = float.Parse(command[1]);
        rotZ = float.Parse(command[2]);

        pitch = float.Parse(command[3]);
        roll = float.Parse(command[4]);
        yaw = float.Parse(command[5]);

        accX = float.Parse(command[6]);
        accY = -float.Parse(command[7]);
        accZ = -float.Parse(command[8]);

        if (doReset)
        {
            doReset = false;

            ps4RotXOffset = GyroInputTester.rotX;
            ps4RotYOffset = GyroInputTester.rotY;
            ps4RotZOffset = GyroInputTester.rotZ;
        }
    }

    string[] SplitOutput()
    {
        string[] output = ReadFromESP()?.Split(char.Parse(","));

        if (output == null) return null;

        if (output.Length < 9) SplitOutput();
        return output;
    }

    public void ReInit()
    {
        if (!usePS4)
        {
            SendToESP("reInit");
            isInitialized = false;
        }
        else
        {
            ps4RotXOffset = GyroInputTester.rotX;
            ps4RotYOffset = GyroInputTester.rotY;
            ps4RotZOffset = GyroInputTester.rotZ;
        }
    }

    Thread thread;

    Queue outputQueue;
    Queue inputQueue;

    public void StartThread()
    {
        outputQueue = Queue.Synchronized(new Queue());
        inputQueue = Queue.Synchronized(new Queue());

        thread = new Thread(ThreadLoop);
        thread.Start();
    }

    public void ThreadLoop()
    {
        stream = new SerialPort("COM16", 115200);
        stream.ReadTimeout = 3000;
        stream.Open();

        // Looping
        while (IsLooping())
        {
            // Send to Arduino
            if (outputQueue.Count != 0)
            {
                string command = (string)outputQueue.Dequeue();
                WriteToESP(command);
            }
            // Read from Arduino
            string result = ReadFromESP(3000);
            if (result != null)
                inputQueue.Enqueue(result);
        }

        stream.Close();
    }

    public void SendToESP(string command)
    {
        outputQueue.Enqueue(command);
    }

    public void WriteToESP(string message)
    {
        stream.WriteLine(message);
        stream.BaseStream.Flush();
    }

    public string ReadFromESP()
    {
        if (inputQueue.Count == 0) return null;
        return (string)inputQueue.Dequeue();
    }

    public string ReadFromESP(int timeout = 0)
    {
        stream.ReadTimeout = timeout;
        try
        {
            string output = stream.ReadLine();
            stream.BaseStream.Flush();
            return output;
        }
        catch (TimeoutException e)
        {
            return null;
        }
    }

    private bool looping = true;
    public void StopThread()
    {
        lock (this)
        {
            looping = false;
        }

    }

    public bool IsLooping()
    {
        lock (this)
        {
            return looping;
        }
    }

    private void OnApplicationQuit()
    {
        CloseStreams();
    }

    public void CloseStreams()
    {
       stream.Close();
    }
}