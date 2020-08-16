using System;
using SimpleJSON;

namespace Core
{
    public static class DB
    {
        private static JSONNode data;
        private static string idxObj = "objects";


        public static event Action<string, int> objectAdded = delegate { };
        public static event Action<string, int> objectUpdated = delegate { };
        public static event Action<string, int> objectDeleted = delegate { };


        public static void Init(){
            data = JSON.Parse("{}").AsObject;
            data.Add(idxObj, JSON.Parse("{}").AsObject);
        }

        public static void ParseServerData(JSONNode data){
            // Add / Update
            var updatedObjects = data["upd"].AsObject;
            foreach (var kv in updatedObjects){
                var type = kv.Key;
                var dbObjects = data[idxObj][type].AsArray;

                var objects = kv.Value.AsArray;
                foreach (var _kv in objects){
                    var obj = _kv.Value;
                    var id = obj["id"].AsInt;

                    if (dbObjects[id] == null){
                        objectAdded(type, id);
                    }
                    else {
                        objectUpdated(type, id);
                    }

                    dbObjects[id] = obj;
                }
            }

            // Delete
            var deletedObjects = data["del"].AsObject;
            foreach (var kv in deletedObjects){
                var type = kv.Key;
                var dbObjects = data[idxObj][type].AsArray;

                var objectIds = kv.Value.AsArray;
                foreach (var id in objectIds.Value){
                    dbObjects[id] = null;
                    objectDeleted(type, id);
                }
            }
        }
    } 
}

