using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class EmmersionLights : MonoBehaviour
{
    [SerializeField] bool use = false;
    SerialPort stream = null;

    float timer;

    void Start()
    {
        if (!use) return;
        StartThread();
    }

    public void Update()
    {
        if (!use) return;
        string yax = ((int) ((SerialHandler.rotY / 360f) * 195f)).ToString();
        if (timer > 0.2f)
        {
            SendToESP("<Value, " + yax + ", 0.0>");
            timer = 0;
        }
        string read = ReadFromESP();
        if (read != null) Debug.Log(read);
        timer += Time.deltaTime;
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
        stream = new SerialPort("COM11", 115200);
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
