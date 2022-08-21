using System.Reflection;

namespace StarBreaker
{
    public static class StarBreakerAssetTexture
    {
        /// <summary>
        /// 加载所有贴图
        /// </summary>
        public static void LoadAll(AssetRepository asset)
        {
            FrostFistSky = asset.Request<Texture2D>("Backgronuds/FrostFistSky");
            Portal = asset.Request<Texture2D>("Backgronuds/Portal");
            LightB = asset.Request<Texture2D>("Backgronuds/LightB");
            StarSky = asset.Request<Texture2D>("Backgronuds/StarSky");
            GunAndKnifeChannelUI = asset.Request<Texture2D>("Items/Weapon/IceGunAndFireKnife/GunAndKnifeChannelUI");
            GunAndKnifeChannelUI_Line = asset.Request<Texture2D>("Items/Weapon/IceGunAndFireKnife/GunAndKnifeChannelUI_Line");
            LightStar = asset.Request<Texture2D>("Particle/LightStar");
            TheBall = asset.Request<Texture2D>("Particle/TheBall");

            MyExtras = new Asset<Texture2D>[9];
            for (int i = 0; i < MyExtras.Length; i++)
            {
                MyExtras[i] = asset.Request<Texture2D>("Images/MyExtra_" + i.ToString());
            }

        }
        /// <summary>
        /// 卸载所有贴图
        /// </summary>
        public static void UnLoadAll()
        {
            Type type = typeof(StarBreakerAssetTexture);//获取自己的type
            FieldInfo[] file = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);//反射得到所有字段
            for(int i = 0;i<file.Length;i++)
            {
                object obj = file[i].GetValue(null);
                if (obj != null)
                {
                    if(obj is Asset<Texture2D> asset)
                    {
                        asset.Dispose();
                        file[i].SetValue(null, null);
                    }
                    else if(obj is Asset<Texture2D>[] assets)
                    {
                        for(int j = 0;j<assets.Length;j++)
                        {
                            assets[j].Dispose();
                        }
                        file[i].SetValue(null, null);
                    }
                }
            }
        }
        public static Asset<Texture2D>[] MyExtras = new Asset<Texture2D>[9];
        public static Asset<Texture2D> FrostFistSky;
        public static Asset<Texture2D> Portal;
        public static Asset<Texture2D> LightB;
        public static Asset<Texture2D> StarSky;
        public static Asset<Texture2D> GunAndKnifeChannelUI;
        public static Asset<Texture2D> LightStar;
        public static Asset<Texture2D> GunAndKnifeChannelUI_Line;
        public static Asset<Texture2D> TheBall;
    }
}
