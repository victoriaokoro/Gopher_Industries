using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace foodremedy.api.Utils;

public class SigningKeyFactory
{
    public static SymmetricSecurityKey Get(string signingKey)
    {
        ArgumentException.ThrowIfNullOrEmpty(signingKey);
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
    }
}
