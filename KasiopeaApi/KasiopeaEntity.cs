namespace KasiopeaApi
{
    public abstract class KasiopeaEntity
    {
        protected const string BaseUrl = "https://kasiopea.matfyz.cz/";

        protected KasiopeaEntity(string url) {
            Url = url;
        }

        public virtual string Url { get; protected set; }
    }
}