using Konscious.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EventlyBackEnd.Functions
{
    public class Authenticate
    {
        public string HashPassword(string password)
        {
            // Generate a salt
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Hash the password with the salt using Argon2id
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                Iterations = 4,
                MemorySize = 256 * 1024 //256MB
            };

            // Generate the hash
            byte[] hashBytes = argon2.GetBytes(64);

            // Combine the salt and hashed bytes for storage
            return $"{Convert.ToBase64String(salt)}:{Convert.ToBase64String(hashBytes)}";
        }

        public bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Split the stored hash into salt and hash bytes
            string[] parts = storedHash.Split(':');
            byte[] salt = Convert.FromBase64String(parts[0]);
            byte[] storedHashBytes = Convert.FromBase64String(parts[1]);

            // Hash the entered password using the stored salt
            var argon2 = new Argon2id(Encoding.UTF8.GetBytes(enteredPassword))
            {
                Salt = salt,
                DegreeOfParallelism = 4,
                Iterations = 4,
                MemorySize = 256 * 1024
            };

            // Generate the hash
            byte[] enteredHashBytes = argon2.GetBytes(64);

            // Compare the entered hash with the stored hash
            return StructuralComparisons.StructuralEqualityComparer.Equals(storedHashBytes, enteredHashBytes);
        }

        public string GenerateJwtToken(long userId, string email, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, userId.ToString()), // Claim for user ID
                    new Claim(ClaimTypes.Email, email) // Claim for email
                }),
                Expires = DateTime.UtcNow.AddDays(1), // Token expiration time
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }


        public bool VerifyJwtToken(string token, string secretKey)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            return validatedToken != null;
        }
    }
}
