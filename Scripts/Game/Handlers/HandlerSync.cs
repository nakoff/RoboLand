using Core;
using Network;
using SimpleJSON;

namespace Game
{
    public class HandlerSync : BaseHandler
    {
        public override string Run(JSONNode responseData)
        {
            var time = responseData["time"].AsDouble;

            DB.ParseServerData(responseData);

            Debug.DbgUtilites.Log("Data => ", DB._data);

            return null;
        }
    }
}

