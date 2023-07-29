using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Items
{
    public class StarBreaker_GlobalItem : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.ModItem is IHeldProjItem;
        public override void HoldItem(Item item, Player player)
        {
            if (player.ownedProjectileCounts[item.shoot] <= 0)
            {
                Projectile.NewProjectile(player.GetSource_ItemUse(item), player.position, Vector2.Zero, item.shoot,player.GetWeaponDamage(item),player.GetWeaponKnockback(item),
                    player.whoAmI);
            }
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => false;
    }
}
