using BlogApi.DAL.Entities;


namespace EMSBLL.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}