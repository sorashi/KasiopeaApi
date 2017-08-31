using System.Threading.Tasks;

namespace KasiopeaApi
{
    public abstract class KasiopeaResolvableEntity<T> : KasiopeaEntity
    {
        protected KasiopeaResolvableEntity(string url) : base(url) {
        }

        public bool Resolved { get; protected set; }

        public virtual Task<T> Resolve(KasiopeaInterface kasiopeaInterface) {
            Resolved = true;
            return Task.FromResult(default(T));
        }
    }
}