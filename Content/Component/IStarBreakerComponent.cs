using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarBreaker.Content.Component
{
    public interface IStarBreakerComponent
    {
        public bool ShouldRemove { get; set; }
        public void Remove();
        public void Apply();
        public void RemoveUpdate<T>(Dictionary<Type,T> pairs) where T : IStarBreakerComponent
        {
            if (ShouldRemove)
            {
                pairs.Remove(GetType());
                Remove();
            }
        }
    }
}
