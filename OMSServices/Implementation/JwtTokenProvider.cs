using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace OMSServices.Implementation
{
    public class JwtTokenProvider
    {
        public bool Enabled { get; }
        public string Token { get; }

        public JwtTokenProvider(IConfiguration configuration)
        {
            Enabled = Convert.ToBoolean(configuration["JwtSettingsForQueryServer:Enabled"]);
            if (Enabled)
            {
                Token = SignToken(configuration["JwtSettingsForQueryServer:Key"], configuration["JwtSettingsForQueryServer:OTP"]);
            }
        }

        private static string SignToken(string keyString, string otp)
        {
            var key = Encoding.ASCII.GetBytes(keyString);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Claims = new Dictionary<string, object>
                {
                    { "OTP", otp }
                },
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
