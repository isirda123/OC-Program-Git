using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine.XR;
//using AXNGames.Networking;

public class UdpManager : MonoBehaviour {

    // receiving Thread
    Thread receiveThread;

    // infos
    public string lastReceivedUDPPacket = "";
    public string allReceivedUDPPackets = ""; // clean up this from time to time!

    private int port = 2600;
    static IPEndPoint remoteEndPoint;

    static UdpClient client;

    public void Start()
    {
        //print(VideoScript.host);
        /*if (!VideoScript.host)
            gameObject.SetActive(false);*/
        init();
    }
    // init
    private void init()
    {
        port = 26001;
        remoteEndPoint = new IPEndPoint(IPAddress.Broadcast, port); // toute machine
        client = new UdpClient();
        receiveThread = new Thread(
            new ThreadStart(ReceiveData));

        receiveThread.IsBackground = true;
        receiveThread.Start();
    }
    // sendData
    public void sendString(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            client.Send(data, data.Length, remoteEndPoint);
        }
        catch (Exception err)
        {
            print(err.ToString());
        }
    }

    void OnDisable()
    {
        if (receiveThread != null)
            receiveThread.Abort();
        client.Close();
    }


    // receive thread
    public void ReceiveData()
    {

        client = new UdpClient(26001);
        while (true)
        {
            try
            {

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                if (text != "LaunchSlave Track1")
                if (text != "LaunchSlave Track2")
                if (text != "LaunchSlave Track3")
                if (text != "LaunchSlave Track4")
                if (text != "LaunchSlave Track5")
                    print(text);

                // latest UDPpacket
                lastReceivedUDPPacket = text;

                // ....
                allReceivedUDPPackets = allReceivedUDPPackets + text;

            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    // getLatestUDPPacket
    // cleans up the rest
    public string getLatestUDPPacket()
    {
        allReceivedUDPPackets = "";
        return lastReceivedUDPPacket;
    }
}

