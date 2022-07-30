namespace StarBreaker.Mounts
{
    public class StarMountsMount : ModMount
    {
        public override void SetStaticDefaults()
        {
            MountData.jumpHeight = 5; // 坐骑能跳多高.
            MountData.acceleration = 1f; // 坐骑加速的速度.
            MountData.jumpSpeed = 5f; // 按下跳跃按钮时玩家和坐骑向（负 y 速度）跳跃高度上升的速率.
            MountData.blockExtraJumps = false; // 决定你是否可以在坐骑中使用二段跳（如瓶中的云）.
            MountData.constantJump = true; // 允许您按住跳跃按钮.
            MountData.heightBoost = 0; // 底座与地面之间的高度
            MountData.fallDamage = 0f; // 坠落伤害倍数.
            MountData.runSpeed = 11f; //坐骑的速度
            MountData.dashSpeed = 8f; // 冲刺状态下坐骑移动的速度
            MountData.flightTimeMax = 0; // 坐骑可以处于飞行状态的帧数
            MountData.fatigueMax = 0;
            MountData.buff = ModContent.BuffType<Buffs.StarMountsBuff>();
            MountData.spawnDust = DustID.BlueTorch;

            MountData.standingFrameCount = 4;
            MountData.standingFrameDelay = 4;
            MountData.standingFrameStart = 0;
            MountData.runningFrameCount = 4;
            MountData.runningFrameDelay = 16;
            MountData.runningFrameStart = 0;
            MountData.flyingFrameCount = 4;
            MountData.flyingFrameDelay = 4;
            MountData.flyingFrameStart = 0;
            MountData.inAirFrameCount = 4;
            MountData.inAirFrameDelay = 4;
            MountData.inAirFrameStart = 0;
            MountData.idleFrameCount = 4;
            MountData.idleFrameDelay = 8;
            MountData.idleFrameStart = 0;
            MountData.idleFrameLoop = true;
            MountData.swimFrameCount = 4;
            MountData.swimFrameDelay = 4;
            MountData.swimFrameStart = 0;
            if (!Main.dedServ)
            {
                MountData.textureWidth = MountData.backTexture.Width();
                MountData.textureHeight = MountData.backTexture.Height();
            }
        }
        //public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
        //{
        //    Texture2D texture1 = ModContent.Request<Texture2D>("StarBreaker/Projs/Type/EnergyProj").Value;
        //    playerDrawData.Add(new(texture1, drawPosition - new Vector2(0, 20), null, drawColor, 0, drawOrigin,new Vector2(1,2), 0, 0));
        //    return Draw(playerDrawData, drawType, drawPlayer, ref texture, ref glowTexture, ref drawPosition, ref frame, ref drawColor, ref glowColor, ref rotation, ref spriteEffects, ref drawOrigin, ref drawScale, shadow);
        //}
    }
}
