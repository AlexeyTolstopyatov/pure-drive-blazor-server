
namespace puredrive.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public string Login { get; set; }

        public string? Email { get; set; }
        public int Gid { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }

        public User(int id, string login, string password, string? email, int gid, string? name, string? surName)
        {
            Id = id;
            Login = login;
            Password = password;
            Email = email;
            Gid = gid;
            Name = name;
            SurName = surName;
        }

        public User() 
        {
            Id = 0;
            Login    =  Constants.User.NO_ID;
            Password =  Constants.User.NO_PASSWORD;
        }
    }
}
