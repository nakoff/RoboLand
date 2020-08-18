using System;
using SimpleJSON;

namespace Core
{
    public static class DB
    {
        private static JSONNode _data;
        private static string idxObj = "objects";


        public static event Action<string, string> objectAdded = delegate { };
        public static event Action<string, string> objectUpdated = delegate { };
        public static event Action<string, string> objectDeleted = delegate { };


        public static string Init(){
            _data = JSON.Parse("{}").AsObject;
            _data[idxObj] = JSON.Parse("{}").AsObject;

            return null;
        }

        public static JSONNode GetObjects(){
            return _data[idxObj].AsObject;
        }

        public static void ParseServerData(JSONNode data){
            // Add / Update
            var updatedObjects = data["upd"].AsObject;
            foreach (var kv in updatedObjects){
                var type = kv.Key;
                var dbObjects = _data[idxObj][type].AsObject;

                var objects = kv.Value.AsArray;
                foreach (var _kv in objects){
                    var obj = _kv.Value;
                    var id = obj["id"].Value;

                    if (dbObjects[id] == null){
                        dbObjects[id] = obj;
                        objectAdded(type, id);
                    }
                    else {
                        dbObjects[id] = obj;
                        objectUpdated(type, id);
                    }
                }
            }

            // Delete
            var deletedObjects = data["del"].AsObject;
            foreach (var kv in deletedObjects){
                var type = kv.Key;
                var dbObjects = _data[idxObj][type].AsArray;

                var objectIds = kv.Value.AsArray;
                foreach (var _kv in objectIds){     //{del: type1:[id1, id2] }
                    string id = _kv.Value;
                    dbObjects[id] = null;
                    objectDeleted(type, id);
                }
            }
        }
    } 
}

