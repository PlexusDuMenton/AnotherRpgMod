using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.IO;
using AnotherRpgMod.RPGModule.Entities;

namespace AnotherRpgMod.Utils
{
    class MPDebug
    {
        static public void Log(Mod mod, object message)
        {
            Log(mod, (string)message);
        }


        static public void Log(Mod mod, string message)
        {
            if (Main.netMode == 2)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)Message.Log);
                packet.Write(message);
                packet.Send();
            }
        }
    }
}
