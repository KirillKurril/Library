namespace Library.Application.Common.Interfaces
{
    public interface IDbInitializer
    {
        public Task Seed();
    }
}
