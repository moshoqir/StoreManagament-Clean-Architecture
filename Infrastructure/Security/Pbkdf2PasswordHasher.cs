using Application.Interfaces.Security;
using System.Security.Cryptography;

namespace Infrastructure.Security
{
 
    public sealed class Pbkdf2PasswordHasher : IPasswordHasher
    {
        // this is the number of iterations for the PBKDF2 algorithm. A higher number of iterations increases the time it takes to hash a password, making it more secure against brute-force attacks.
        private const int Iterations = 100_000;
        // this is the size of the salt used in the PBKDF2 algorithm. A salt is a random value that is added to the password before hashing to make it more secure against rainbow table attacks.
        private const int SaltSize = 16; // Size of the salt in bytes
        // this is the size of the hash produced by the PBKDF2 algorithm. A larger hash size increases the security of the hashed password.
        private const int HashSize = 32;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty.");

            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password,
                salt,
                Iterations,
                HashAlgorithmName.SHA256,
                HashSize);

            return $"PBKDF2${Iterations}${Convert.ToBase64String(salt)}${Convert.ToBase64String(hash)}";
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            // checck both pass and hash are empty
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
                return false;

            // this to split the password hash into its components (algorithm, iterations, salt, and hash) and check if the format is valid. If the format is invalid, the method returns false.
            string[] parts = passwordHash.Split('$');
            if (parts.Length != 4 || !string.Equals(parts[0], "PBKDF2", StringComparison.Ordinal))
                return false;

            // this to parse the number of iterations from the password hash and check if it is a valid integer. If the parsing fails, the method returns false.
            if (!int.TryParse(parts[1], out int iterations))
                return false;

            // this to convert the salt and expected hash from Base64 strings to byte arrays, and then hash the provided password using the same salt and number of iterations. The resulting hash is compared to the expected hash using a fixed-time comparison to prevent timing attacks. If the hashes match, the method returns true; otherwise, it returns false.
            try
            {
                byte[] salt = Convert.FromBase64String(parts[2]);
                byte[] expectedHash = Convert.FromBase64String(parts[3]);
                byte[] actualHash = Rfc2898DeriveBytes.Pbkdf2(
                    password,
                    salt,
                    iterations,
                    HashAlgorithmName.SHA256,
                    expectedHash.Length);

                return CryptographicOperations.FixedTimeEquals(actualHash, expectedHash);
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
