using Godot;
using System;
using Core;
using Network;

public class GameMain : Node
{

    // NETWORK
    [Export] private int headerSize = 8;
	[Export] private string  installToken = "8P149bYwHB";
    [Export] private string authIP = "192.168.56.11";
	[Export] private int authPort = 10000;



    public override void _Ready(){
        HandlerInit();
        
        ClientTCP.Init(installToken, headerSize);
        var err = ClientTCP.AuthOnServer(authIP, authPort);
        if (err != null){
            GD.PrintErr("Auth Failed: ", err);
        }
    }

    public override void _Process(float delta){
        ClientTCP.Update();
    }

    public override void _ExitTree(){
        ClientTCP.Disconnect();
    }


    private void HandlerInit(){
        HandlerManager.AddHandler(NET_CMD.INSTALL, new Game.HandlerInstall());
    }
}
