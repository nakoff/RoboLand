using Core;
using Network;
using SimpleJSON;

namespace Game
{
    public class HandlerInstall : BaseHandler
    {
        public override string Run(JSONNode responseData)
        {
            var userId = (ushort)responseData["user_id"].AsInt;
            var token = responseData["token"].Value;
            var address = responseData["address"].Value;
            var port = responseData["port"].AsInt;

            if (userId < 1){
                return "wrong user_id";
            }

            if (token == ""){
                return "wrong token";
            }

            ClientTCP.CurToken = token;
            ClientTCP.UserId = userId;

            var err = ClientTCP.ConnectToGame(address, port);
            if (err != null){
                return err;
            }

            return null;
        }
    }
}

