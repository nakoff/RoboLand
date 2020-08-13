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
	
	public class ClientTCP : Control
	{
		private const string address = "192.168.56.11";
		private const int port = 10000;
		private const int headerSize = 8;

		private const string  installToken = "8P149bYwHB";
		private string curToken;
		private uint userId;
		private ushort seq;
		private const ushort cmdInstall = 1;
		
		private StreamPeerTCP socket;
		private List<ClientMessage> msgQueue = new List<Network.ClientMessage>();
		

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
			data = CreateRequest(cmd, seq, userId, curToken, data);
			Send(cmd, data);
		}

		private void Auth(){
			socket = new StreamPeerTCP();
			var status = socket.ConnectToHost(address, port);
			GD.Print("Connect to server: ", status);

			var timer2 = new Timer(){OneShot = true, WaitTime = 1};
			timer2.Connect("timeout", this, nameof(_Send));
			AddChild(timer2);
			timer2.Start();
		}

		private void Recv(){
			var ss = socket.GetAvailableBytes();
			if (ss > headerSize){
				var _size = Utilites.Byte2IntBE((byte[])socket.GetData(4)[1]);
				var _cmd = Utilites.Byte2UShortBE((byte[])socket.GetData(2)[1]);
				var _seq = Utilites.Byte2UShortBE((byte[])socket.GetData(2)[1]);
				var _data = (byte[])socket.GetData(_size)[1];
				var _msg = SimpleJSON.JSON.Parse(System.Text.Encoding.UTF8.GetString(_data, 0, _size));
			}
		}

		private void _Send(){
			curToken = installToken;
			Send(NET_CMD.INSTALL, SimpleJSON.JSON.Parse("{}"));
		}

		private void Send(NET_CMD cmd, JSONNode data){
			var jData = CreateRequest(cmd, seq, userId, installToken, data);
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

