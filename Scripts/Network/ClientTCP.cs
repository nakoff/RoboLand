using System.Collections.Generic;
using Godot;
using SimpleJSON;

namespace Network
{
	class ClientMessage 
	{
		public ushort cmd;
		public JSONNode data;
	}

	class ServerMessage 
	{
		public NET_CMD cmd;
		public ushort seq;
		public JSONNode data;

		public ServerMessage(StreamPeerTCP socket){
			var _size = Utilites.Byte2IntBE((byte[])socket.GetData(4)[1]);
			cmd = (NET_CMD)Utilites.Byte2UShortBE((byte[])socket.GetData(2)[1]);
			seq = Utilites.Byte2UShortBE((byte[])socket.GetData(2)[1]);
			var _data = (byte[])socket.GetData(_size)[1];
			data = SimpleJSON.JSON.Parse(System.Text.Encoding.UTF8.GetString(_data, 0, _size));
		}
	}
	
	public class ClientTCP : Control
	{
		private const string authIP = "192.168.56.11";
		private const int authPort = 10000;
		public string GameIP {get; private set;}
		public int GamePort {get; private set;}

		private const int headerSize = 8;
		private const string  installToken = "8P149bYwHB";
		private ushort seq = 1;

		public static string CurToken {get; set;}
		public static uint UserId {get; set;}
		
		private StreamPeerTCP socket;
		private List<ClientMessage> msgQueue = new List<Network.ClientMessage>();

		public static ClientTCP Instance {get; private set;}
		

		public override void _EnterTree(){
			Instance = this;

			Core.ResponseMap.SetHandler(NET_CMD.INSTALL, new Game.HandlerInstall());
		}

		public override void _Ready(){
			var timer = new Timer(){OneShot = true, WaitTime = 2};
			timer.Connect("timeout", this, nameof(Auth));
			AddChild(timer);
			timer.Start();
		}

		public override void _Process(float delta){

			if (socket == null || socket.IsConnectedToHost() == false){
				return;
			}
			
			Recv();
		}



		public void SendMsg(NET_CMD cmd, JSONNode data){
			data = CreateRequest(cmd, seq, UserId, CurToken, data);
			Send(cmd, data);
		}

		private void Auth(){
			socket = new StreamPeerTCP();
			var status = socket.ConnectToHost(authIP, authPort);
			GD.Print("Connect to server: ", status);

			CurToken = installToken;
			Send(NET_CMD.INSTALL, SimpleJSON.JSON.Parse("{}"));
		}

		public string ConnectToGame(string ip, int port){
			Disconnect();

			GD.Print("CONNECT TO GAME!");
			var status = socket.ConnectToHost(ip, port);
			if (status != Error.Ok){
				socket = null;
				return status.ToString();
			}

			GameIP = ip;
			GamePort = port;
			seq = 1;
			msgQueue.Clear();
			return null;
		}

		private void Disconnect(){
			if (socket == null){
				return;
			}

			if (socket.IsConnectedToHost()){
				socket.DisconnectFromHost();
			}

			socket = null;
		}

		private void Recv(){
			var ss = socket.GetAvailableBytes();
			if (ss > headerSize){
				var response = new ServerMessage(socket);
				GD.Print("recv << cmd:",response.cmd," data:",response.data);
				var handler = Core.ResponseMap.GetHandler(response.cmd);
				if (handler != null){
					handler.Run(response.data);
				}
			}
		}

		private void Send(NET_CMD cmd, JSONNode data){
			var jData = CreateRequest(cmd, seq, UserId, installToken, data);
			var bData = jData.ToString().ToUTF8();
			
			var bufferSize = headerSize + bData.Length;
			var buffer = new byte[bufferSize];

			uint offset = 0;
			offset += Utilites.PushIntBE(buffer, bData.Length, offset);
			offset += Utilites.PushUShortBE(buffer, (ushort)cmd, offset);
			offset += Utilites.PushUShortBE(buffer, seq, offset);
			offset += Utilites.PushByteLE(buffer, bData, offset);


			var status = socket.PutData(buffer);
			GD.Print("Client.Send> status:",status, "  data: ",jData);
			seq++;
		}

		private JSONNode CreateRequest(NET_CMD cmd, ushort seq, uint userId, string token, JSONNode data){
			if (data == null){
				data = SimpleJSON.JSON.Parse("{}");
			}

			data["user_id"] = userId;
			data["sig"] = GetSig((ushort)cmd, seq, userId, token);
			data["pf"] = "WIN";
			return data;
		}

		private string GetSig(ushort cmd, ushort seq, uint userId, string token){
			var str = cmd.ToString() + seq.ToString() + userId.ToString() + token.ToString();
			return str.MD5Text();
		}
	}
}

