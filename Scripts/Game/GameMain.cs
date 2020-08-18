using Godot;
using Core;
using Network;
using Dbg = Debug.DbgUtilites;

namespace Game
{
    public class GameMain : Node
    {

        // NETWORK
        [Export] private int headerSize = 8;
        [Export] private string  installToken = "8P149bYwHB";
        [Export] private string authIP = "192.168.56.11";
        [Export] private int authPort = 10000;

        private EditorInterface rdbViewer;

        public override void _Ready(){

            // Runtime DB Init
            var err = DB.Init();
            if (err != null){
                Dbg.LogErr("DB Init: ", err);
                return;
            }
            Dbg.LogSucc("DB Init: Success!");

            // Game Handlers Init
            err = HandlerInit();
            if (err != null){
                Dbg.LogErr("Handler Init: ", err);
                return;
            }
            Dbg.LogSucc("Handlers Init: Success!");

            // Entity Manager Init
            err = EntityManager.Init();
            if (err != null){
                Dbg.LogErr("Entity Manager Init: ", err);
                return;
            }
            Dbg.LogSucc("Entity Manager Init: Success!");
            
            // TCP Client
            ClientTCP.Init(installToken, headerSize);
            err = ClientTCP.AuthOnServer(authIP, authPort);
            if (err != null){
                Dbg.LogErr("Auth Failed: ", err);
                return;
            }
            Dbg.LogSucc("Auth Success");
        }

        public override void _Process(float delta){
            ClientTCP.Update();
        }

        public override void _ExitTree(){
            ClientTCP.Disconnect();
            EntityManager.Delete();
        }


        private string HandlerInit(){
            try{
                var err = HandlerManager.AddHandler(NET_CMD.INSTALL, new Game.HandlerInstall());
                if (err != null) return err;

                err = HandlerManager.AddHandler(NET_CMD.SYNC, new Game.HandlerSync());
                if (err != null) return err;
            }
            catch{
                return "unknown error";
            }

            return null;
        }
    }
}
