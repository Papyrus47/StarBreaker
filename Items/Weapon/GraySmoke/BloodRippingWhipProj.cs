using StarBreaker.Projs.Type;

namespace StarBreaker.Items.Weapon.GraySmoke
{
	public class BloodRippingWhipProj : BaseWhip_Channel
	{
		public override void SetStaticDefaults()
		{
			DisplayName.AddTranslation(7, "鲜血撕裂鞭");
			ProjectileID.Sets.IsAWhip[Type] = true;
		}
		public override void SetDefault()
		{
			Projectile.DefaultToWhip();
			ChannelWhip.SegmentsTime = 6;
			ChannelWhip.RangeMultiplier = 1 / 60f;
			ChannelWhip.MaxChargeTime = 250;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 0;
		}
        public override void ChannelAI()
        {
			base.ChannelAI();
        }
        public override void WhipDraw()
		{
			List<Vector2> list = new();
			Projectile.FillWhipControlPoints(Projectile, list);
			DrawLine(list);

			SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

			Main.instance.LoadProjectile(Type);
			Texture2D texture = TextureAssets.Projectile[Type].Value;

			Vector2 pos = list[0];

			for (int i = 0; i < list.Count - 1; i++)
			{
				Rectangle frame = new Rectangle(0, 6, 18, 18);
				Vector2 origin = new Vector2(8,14);
				float scale = 1;

				Vector2 element = list[i];
				Vector2 diff = list[i + 1] - element;
				float rotation = diff.ToRotation() - MathHelper.PiOver2; //旋转
				Color color = Lighting.GetColor(element.ToTileCoordinates());

				if (i == list.Count - 2)
				{
					frame.Y = 160;
					frame.Height = 40;
					flip = Projectile.spriteDirection > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

					Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
					float t = Timer / timeToFlyOut;
					scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
				}
				else if (i > 10)
				{
					frame.Y = 54;
					frame.Height = 18;
				}
				else if (i > 5)
				{
					frame.Y = 94;
					frame.Height = 18;
				}
				else if (i > 0)
				{
					frame.Y = 134;
					frame.Height = 18;
				}

				Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);

				pos += diff;
			}
		}
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			ref int blood = ref target.GetGlobalNPC<NPCs.StarGlobalNPC>().BloodyBleed;
			blood = 1570 + (int)ChargeTime;
			if(Projectile.numHits > 5)
            {
				Projectile.numHits = 0;
				for(int i =0;i<2;i++)
                {
					blood = 1570 + (int)ChargeTime;
					target.StrikeNPC(damage, knockback, Projectile.direction, crit);
				}
            }
			if(ChargeTime >= ChannelWhip.MaxChargeTime)
            {
				target.GetGlobalNPC<NPCs.StarGlobalNPC>().BloodRippingWhipTime = 3600;
            }
        }
    }
}
