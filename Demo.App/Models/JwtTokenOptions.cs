using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.App.Models
{
    public class JwtTokenOptions
    {
        /// <summary>
        /// Gets or sets a string that represents a valid issuer that will be used to check against the token's issuer.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Gets or sets a string that represents a valid audience that will be used to check against the token's audience.
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Gets or sets the string that is to be used for signature validation by converions into Microsoft.IdentityModel.Tokens.SecurityKey.
        /// </summary>
        public string SigningKey { get; set; }

        /// <summary>
        /// Token expiration time
        /// </summary>
        public int ExpirationInMinutes { get; set; } = 20;

        /// <summary>
        /// If is true, then in every response is send new token with shifted expiration
        /// </summary>
        public bool SendNewKeyInEveryResponse { get; set; }
    }
}
