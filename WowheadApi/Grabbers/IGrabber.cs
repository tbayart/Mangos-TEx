namespace WowheadApi.Grabbers
{
    public interface IGrabber<T>
        where T : class
    {
        T Extract(string data, int id);
    }
}
