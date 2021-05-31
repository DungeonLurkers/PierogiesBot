namespace PierogiesBot.Manager.Services
{
    public interface IFactory<T>
    {
        T? Create();
    }
}