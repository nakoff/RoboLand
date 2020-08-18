using SimpleJSON;
using Core;

namespace Game.Model
{
    public class PlayerInfo
    {
        private JSONNode jData;

        public PlayerInfo(string id){
            var data = DB.GetObjects()[EntityType.PLAYER_INFO].AsObject;
            jData = data[id];
        }

        public string Id => jData["id"].Value;
        public double CTime => jData["ctime"].AsDouble;
        public string Name => jData["name"].Value;
    }  
}

