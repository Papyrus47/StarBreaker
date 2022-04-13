﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.GameContent;

namespace StarBreaker.NPCs.StarBreakerEXGunNPC
{
    public class FocusFlamethrower : EXGunNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("聚焦喷火器");
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new()
            {
                Hide = true
            };//隐藏
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);//让它绘制
        }
        public override void GunAI()
        {

        }
    }
}