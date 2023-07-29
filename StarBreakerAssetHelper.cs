using ReLogic.Content.Sources;
using System.Reflection;

namespace StarBreaker
{
    public static class StarBreakerAssetHelper
    {
        public const string Texture2DPath = "Effects/Images/";
        public const string EffectsPath = "Effects/XNBFiles/";
        public static Asset<Texture2D>[] SB_Extra;
        public static Asset<Texture2D> StarsPierceProjColor;
        public static Asset<Texture2D> StarsPierceProjColor1;
        public static Asset<Texture2D> StarOriginStaffColor;
        public static Asset<Texture2D> AppraiseTex;
        public static Asset<Texture2D> Vertigo;
        public static Asset<Effect> StarsPierceProjEffect;
        public static Asset<Effect> StarsPierce_ContinuityEffect;
        public static Asset<Effect> SwingEffect;
        public static void Load(AssetRepository asset, IContentSource contentSource)
        {
            Type type = typeof(StarBreakerAssetHelper);
            var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
            List<Asset<Texture2D>> list = new();
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (typeof(Asset<Texture2D>).IsAssignableFrom(field.FieldType))
                {
                    field.SetValue(null, asset.Request<Texture2D>(Texture2DPath + field.Name));
                }
                else if (typeof(Asset<Texture2D>[]).IsAssignableFrom(field.FieldType))
                {
                    list.Clear();
                    int j = 0;
                    while (true)
                    {
                        string path = Texture2DPath + field.Name + "_" + j.ToString();
                        if (!contentSource.HasAsset(path))
                        {
                            break;
                        }

                        var s = asset.Request<Texture2D>(path, AssetRequestMode.ImmediateLoad);
                        list.Add(s);
                        j++;
                    }
                    field.SetValue(null, list.ToArray());
                }
                else if (typeof(Asset<Effect>).IsAssignableFrom(field.FieldType))
                {
                    field.SetValue(null, asset.Request<Effect>(EffectsPath + field.Name));
                }
            }
        }
        public static void UnLoad() // 啊对,我就是反射,怎么了?
        {
            Type type = typeof(StarBreakerAssetHelper);
            var fields = type.GetFields(BindingFlags.Static);
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                field.SetValue(null, null);
            }
        }
    }
}
