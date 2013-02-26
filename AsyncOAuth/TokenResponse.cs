using System.Linq;

namespace AsyncOAuth
{
    /// <summary>OAuth Response</summary>
    public class TokenResponse<T> where T : Token
    {
        public T Token { get; private set; }
        public ILookup<string, string> ExtraData { get; private set; }

        public TokenResponse(T token, ILookup<string, string> extraData)
        {
            Precondition.NotNull(token, "token");
            Precondition.NotNull(extraData, "extraData");

            this.Token = token;
            this.ExtraData = extraData;
        }
    }
}