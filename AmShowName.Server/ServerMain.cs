using System;
using CitizenFX.Core;
using AmQBCore.Player;
using AmQBCore.Server;

namespace AmShowName.Server
{
    public class ServerMain : ServerScript
    {
        private const string GetPlayerFullNameKey = "server:GetPlayerFullName";

        private QBCore QBCore;

        public ServerMain() 
        {
            QBCore = new QBCore(Exports);
        }

        [EventHandler(GetPlayerFullNameKey)]
        private void GetPlayerFullName([FromSource] Player src, int targetId,  NetworkCallbackDelegate cb) 
        {
            
            QBPlayer p = QBCore.Functions.GetPlayer(targetId);
            if (p == null)
            {
                cb("");
                return;
            }
            QBPlayerData data = p.PlayerData;
            if (data == null)
            {
                cb("");
                return;
            }
            string dest = $"{data.CharInfo.FirstName}{data.CharInfo.LastName}";
            cb(dest);
        }
    }
}
