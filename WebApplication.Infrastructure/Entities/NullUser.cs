namespace WebApplication.Infrastructure.Entities
{
    public sealed class NullUser: User
    {
        public override bool IsNull => true;
    }
}