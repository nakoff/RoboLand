using Core;


namespace Game
{
    public static class EntityManager
    {
        public static string Init(){
            DB.objectAdded += OnObjectAdded;
            
            return null;
        }


        public static void Delete(){
            DB.objectAdded -= OnObjectAdded;
        }


        private static void OnObjectAdded(string type, string id)
        {
            switch (type)
            {
                case EntityType.PLAYER_INFO:
                    CreatePlayer(id);
                    break;
            }
        }

        private static void CreatePlayer(string id){
            var playerInfoModel = new Model.PlayerInfo(id);
            Debug.DbgUtilites.Log("Create Player: id=", playerInfoModel.Id," name=", playerInfoModel.Name);
        }
    }
}
