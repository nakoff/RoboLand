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
	
	public static class ClientTCP
	{
		public static string GameIP {get; private set;}
		public static int GamePort {get; private set;}
		public static string CurToken {get; set;}
		public static uint UserId {get; set;}

		private static ushort seq = 1;
		private static int hdrSize;
		private static StreamPeerTCP socket;
		private static List<ClientMessage> msgQueue = new List<Network.ClientMessage>();


		public static void Init(string authToken, int headerSize){
			CurToken = authToken;
			hdrSize = headerSize;
		}

		public static void Update(){

			if (socket == null || socket.IsConnectedToHost() == false){
				return;
			}
			
			Recv();
		}

		public static void SendMsg(NET_CMD cmd, JSONNode data){
			data = CreateRequest(cmd, seq, UserId, CurToken, data);
			Send(cmd, data);
		}

		public static string AuthOnServer(string ip, int port){
			socket = new StreamPeerTCP();
			var status = socket.ConnectToHost(ip, port);
			if (status != Error.Ok){
				socket = null;
				return status.ToString();
			}

			Send(NET_CMD.INSTALL, SimpleJSON.JSON.Parse("{}"));
			return null;
		}

		public static string ConnectToGame(string ip, int port){
			Disconnect();

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

		public static void Disconnect(){
			if (socket == null){
				return;
			}

			if (socket.IsConnectedToHost()){
				socket.DisconnectFromHost();
			}

			socket = null;
		}

		private static void Recv(){
			var ss = socket.GetAvailableBytes();
			if (ss > hdrSize){
				var response = new ServerMessage(socket);
				GD.Print("recv << cmd:",response.cmd," data:",response.data);

				var handler = Core.HandlerManager.GetHandler(response.cmd);
				if (handler != null){
					var err = handler.Run(response.data);
					if (err != null){
						GD.PrintErr("Handler error: ", err);
					}
				}
			}
		}

		private static void Send(NET_CMD cmd, JSONNode data){
			var jData = CreateRequest(cmd, seq, UserId, CurToken, data);
			var bData = jData.ToString().ToUTF8();
			
			var bufferSize = hdrSize + bData.Length;
			var buffer = new byte[bufferSize];

			uint offset = 0;
			offset += Utilites.PushIntBE(buffer, bData.Length, offset);
			offset += Utilites.PushUShortBE(buffer, (ushort)cmd, offset);
			offset += Utilites.PushUShortBE(buffer, seq, offset);
			offset += Utilites.PushByteLE(buffer, bData, offset);


			var status = socket.PutData(buffer);
			GD.Print("send >> status:",status, "  data: ",jData);
			seq++;
		}

		private static JSONNode CreateRequest(NET_CMD cmd, ushort seq, uint userId, string token, JSONNode data){
			if (data == null){
				data = SimpleJSON.JSON.Parse("{}");
			}

			data["user_id"] = userId;
			data["sig"] = GetSig((ushort)cmd, seq, userId, token);
			return data;
		}

		private static string GetSig(ushort cmd, ushort seq, uint userId, string token){
			var str = cmd.ToString() + seq.ToString() + userId.ToString() + token.ToString();
			return str.MD5Text();
		}
	}
}

