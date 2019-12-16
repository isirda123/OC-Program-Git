using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Text;

/// <summary>
/// Permet d'insérer de nouveau process de jeu
/// </summary>
[System.Serializable]
public struct structGame
{
    public string game;
    public string[] processtokill;
}

public class ScriptOC : MonoBehaviour
{
    /// <summary>
    /// Permet d'insérer de nouveau process de jeu
    /// </summary>
    [SerializeField]
    public List<structGame> listOfProcess = new List<structGame>();
    List<string> allGames = new List<string>();
    [SerializeField]
    List<string> allApp = new List<string>();
    Process gameLaunch;

    public UdpManager Sender;
    public string TextToSend;
    public string HostOrSlave;

    bool gameCheckAlreadyLaunch = false;
    bool processCheckAlreadyLaunch = false;

    void Start()
    {
        foreach (structGame game in listOfProcess)
        {
            allGames.Add(game.game);
            findapp(game);
        }
        if (File.Exists("Slave01"))
            HostOrSlave = "Slave01";
        else if (File.Exists("Slave02"))
            HostOrSlave = "Slave02";
        else if (File.Exists("Host"))
            HostOrSlave = "Host";
    }
    void findapp(structGame game)
    {
        int j;

        foreach (string ptk in game.processtokill)
        {
            j = 0;
            if (ptk != game.processtokill[0])
            {
                foreach (string app in allApp)
                {
                    if (ptk == app)
                        j = 1;
                }
                if (j == 0)
                    allApp.Add(ptk);
            }
        }
    }
    void Update()
    {
        string whichnolimits;
        string tracktolaunch = "";
        if (Checkallgame() == "nolimits2app")
        {
            whichnolimits = Process.GetProcessesByName("nolimits2app")[0].MainModule.FileName;
            print(whichnolimits);
            whichnolimits = whichnolimits + "/../Arguments";
            StreamReader nolimitrack = new StreamReader(whichnolimits);
            string line = nolimitrack.ReadLine();

            while (!string.IsNullOrEmpty(line))
            {
                if (line.Contains("Track"))
                {
                    tracktolaunch = line;
                    break ;
                }
            }
            if (HostOrSlave == "Host")
                Sender.SendMessage("LaunchSlave " + tracktolaunch);
        }
        print (gameLaunch);
        if (Input.GetKeyDown(KeyCode.A))
        {
            Sender.sendString(TextToSend);
        }
    }

    void Breakchecker()
    {

    }

    //Checkprocessbyname
    string Checkallgame()
    {
        string gamefind = "";
        int i;
        i = 0;
        foreach (structGame game in listOfProcess)
        {
            foreach (Process process in Process.GetProcessesByName(game.processtokill[0]))
            {
                gameLaunch = process;
                i = 1;
                gamefind = game.processtokill[0];
                if (gameCheckAlreadyLaunch == false)
                {
                    gameCheckAlreadyLaunch = true;
                    StartCoroutine(WaitingBeforeKill(game));
                }
            }
        }
        if (i == 0)
        {
            print("checkApp");
            gameLaunch = null;
            checkapp();
        }
        return (gamefind);
    }

    //Checkappbyname
    void checkapp()
    {
        foreach (string name in allApp)
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                if (process.ProcessName == name)
                    if (processCheckAlreadyLaunch == false)
                    {
                        processCheckAlreadyLaunch = true;
                        StartCoroutine(WaitingBeforeKill(process, name));

                    }
            }
        }
    }

    IEnumerator WaitingBeforeKill(structGame game)
    {
        yield return new WaitForSeconds(7f);

        foreach (string name in game.processtokill)
        {
            if (possiblekill(name) == 0)
            {
                killall(game.processtokill);
                
            }
        }
        gameCheckAlreadyLaunch = false;
    }

    // All Function
    IEnumerator WaitingBeforeKill(Process processMaybeKill, string name)
    {
        yield return new WaitForSeconds(7f);
        print("7s passed : " + name);
        if (gameLaunch == null)
        {
            print("it was nul");
            if (possiblekill(name) == 1)
            {
                if (processMaybeKill.ProcessName == name)
                    processMaybeKill.Kill();
            }
        }
            processCheckAlreadyLaunch = false;
    }

    int checkappbyname(List<string> allApp, string exception)
    {

        int i = 0;

        foreach (string name in allApp)
        {
            foreach (var process in Process.GetProcessesByName(name))
            {
                if (process.ProcessName == name)
                    i += 1;
            }
        }
        if (i == allApp.Count)
            return (1);
        return (0);
    }

    void killall(string[] processtokill)
    {
        
        foreach (string killable in processtokill)
        {
            foreach (Process proc in Process.GetProcessesByName(killable))
                proc.Kill();
        }
    }

    int possiblekill(string name)
    {
        foreach (var process in Process.GetProcessesByName(name))
        {
            if (process.ProcessName == name)
                return (1);
        }
        return (0);
    }
}