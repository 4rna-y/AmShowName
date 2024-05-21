using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using AmQBCore.Client;

using static CitizenFX.Core.Native.API;

namespace AmShowName.Client
{
    public class ClientMain : ClientScript
    {
        private int _delayer = 0;
        private const string GetPlayerFullNameKey = "server:GetPlayerFullName";

        private Dictionary<int, string> _fullNames;

        private QBCore QBCore { get; set; }
        public ClientMain()
        {
            QBCore = new QBCore(Exports);
            _fullNames = new Dictionary<int, string>();
        }

        [Tick]
        public Task OnTick()
        {
            UpdateData();
            Draw();

            return Task.FromResult(0);
        }

        private void UpdateData()
        {
            if (_delayer++ == 120)
            {
                _delayer = 0;

                foreach (Player player in this.Players)
                {
                    if (!_fullNames.ContainsKey(player.ServerId))
                    {
                        TriggerServerEvent(GetPlayerFullNameKey, player.ServerId, new Action<string>((name) =>
                        {
                            if (string.IsNullOrWhiteSpace(name)) return;
                            if (_fullNames.ContainsKey(player.ServerId)) return;
                            _fullNames.Add(player.ServerId, name);
                        }));

                        
                    }
                }
            }
        }

        private void Draw()
        {
            
            Vector3 pPos = Player.Local.Character.Position;
            foreach (Player player in this.Players) 
            {
                if (!_fullNames.ContainsKey(player.ServerId)) continue;
                Vector3 tPos = player.Character.Position;
                if (Vector3.Distance(pPos, tPos) < 5)
                {
                    DrawText3D(tPos.X, tPos.Y, tPos.Z + 1.1f, $"[{player.ServerId}] {_fullNames[player.ServerId]}");
                }
            }
        }

        private void DrawText3D(float x, float y, float z, string text)
        {
            float sx = 0f;
            float sy = 0f;
            if (World3dToScreen2d(x, y, z, ref sx, ref sy))
            {
                Vector3 camCoord = GetGameplayCamCoords();
                float dis = GetDistanceBetweenCoords(camCoord.X, camCoord.Y, camCoord.Z, x, y, z, true);
                float scale = 1 * (1 / dis) * (1 / GetGameplayCamFov()) * 100;

                SetTextScale(scale, scale);
                SetTextFont(0);
                SetTextProportional(true);
                SetTextColour(255, 255, 255, 255);
                SetTextDropshadow(0, 0, 0, 0, 255);
                SetTextDropShadow();
                SetTextEdge(4, 0, 0, 0, 255);
                SetTextOutline();
                SetTextEntry("STRING");
                SetTextCentre(true);
                AddTextComponentString(text);
                DrawText(sx, sy);
            }
        }
    }
}
