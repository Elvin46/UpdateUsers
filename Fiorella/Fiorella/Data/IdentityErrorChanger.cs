using Microsoft.AspNetCore.Identity;

namespace Fiorella.Data
{
    public class IdentityErrorChanger : IdentityErrorDescriber
    {
        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError
            {
                Code = "Email",
                Description = "This Email is already taken."
            };
        }
    }
}
