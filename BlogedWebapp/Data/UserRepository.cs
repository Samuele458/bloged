using BlogedWebapp.Entities;
using BlogedWebapp.Helpers;
using Microsoft.Extensions.Options;

namespace BlogedWebapp.Data
{

    public interface IUserRepository
    {
        void Create(User user);
    }

    public class UserRepository : IUserRepository
    {
        private readonly AppSettings AppSettings;
        private readonly DataContext Context;

        public UserRepository(DataContext context, IOptions<AppSettings> appSettings)
        {
            this.Context = context;
            this.AppSettings = appSettings.Value;
        }

        public void Create(User user)
        {
            Context.Users.Add(user);
            Context.SaveChanges();
        }
    }
}
