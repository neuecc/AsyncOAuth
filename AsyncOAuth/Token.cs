using System.Diagnostics;

namespace AsyncOAuth
{
    /// <summary>represents OAuth Token</summary>
    [DebuggerDisplay("Key = {Key}, Secret = {Secret}")]
    public abstract class Token
    {
        public string Key { get; private set; }
        public string Secret { get; private set; }

        public Token(string key, string secret)
        {
            Precondition.NotNull(key, "key");
            Precondition.NotNull(secret, "secret");

            this.Key = key;
            this.Secret = secret;
        }
    }

    /// <summary>represents OAuth AccessToken</summary>
    public class AccessToken : Token
    {
        public AccessToken(string key, string secret)
            : base(key, secret)
        { }
    }

    /// <summary>represents OAuth RequestToken</summary>
    public class RequestToken : Token
    {
        public RequestToken(string key, string secret)
            : base(key, secret)
        { }
    }
}