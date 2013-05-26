using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

#pragma warning disable 612, 618

namespace AsyncOAuth
{
    /// <summary>represents OAuth Token</summary>
    [DebuggerDisplay("Key = {Key}, Secret = {Secret}")]
    [DataContract]
    public abstract class Token
    {
        [DataMember(Order = 1)]
        public string Key { get; private set; }
        [DataMember(Order = 2)]
        public string Secret { get; private set; }

        /// <summary>for serialize.</summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public Token()
        {

        }

        public Token(string key, string secret)
        {
            Precondition.NotNull(key, "key");
            Precondition.NotNull(secret, "secret");

            this.Key = key;
            this.Secret = secret;
        }
    }

    /// <summary>represents OAuth AccessToken</summary>
    [DataContract]
    public class AccessToken : Token
    {
        /// <summary>for serialize.</summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AccessToken()
        {

        }

        public AccessToken(string key, string secret)
            : base(key, secret)
        { }
    }

    /// <summary>represents OAuth RequestToken</summary>
    [DataContract]
    public class RequestToken : Token
    {
        /// <summary>
        /// for serialize.
        /// </summary>
        [Obsolete("this is used for serialize")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public RequestToken()
        {

        }

        public RequestToken(string key, string secret)
            : base(key, secret)
        { }
    }
}

#pragma warning restore 612, 618