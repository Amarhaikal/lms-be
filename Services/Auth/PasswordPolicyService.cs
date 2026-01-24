using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace QUANTM.Services.Auth
{
    public class PasswordPolicyService
    {
        public ValidationResult ValidatePassword(string password, string username)
        {
            var errors = new List<string>();

            // Length check
            if (password.Length < 12)
            {
                errors.Add("Password must be at least 12 characters long");
            }

            // Uppercase check
            if (!password.Any(char.IsUpper))
            {
                errors.Add("Password must contain at least one uppercase letter");
            }

            // Lowercase check
            if (!password.Any(char.IsLower))
            {
                errors.Add("Password must contain at least one lowercase letter");
            }

            // Number check
            if (!password.Any(char.IsDigit))
            {
                errors.Add("Password must contain at least one number");
            }

            // Special character check
            if (!Regex.IsMatch(password, @"[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]"))
                errors.Add("Password must contain at least one special character");

            // Username check (case-insensitive)
            if (password.ToLower().Contains(username.ToLower()))
                errors.Add("Password cannot contain your username");

            return new ValidationResult
            {
                IsValid = errors.Count == 0,
                Errors = errors
            };
        }

        public int CalculatePasswordStrength(string password)
        {
            int strength = 0;

            if (password.Length >= 12) strength += 20;
            if (password.Length >= 16) strength += 20;
            if (Regex.IsMatch(password, @"[A-Z]")) strength += 15;
            if (Regex.IsMatch(password, @"[a-z]")) strength += 15;
            if (Regex.IsMatch(password, @"[0-9]")) strength += 15;
            if (Regex.IsMatch(password, @"[!@#$%^&*]")) strength += 15;

            return Math.Min(strength, 100);
        }

        public class ValidationResult
        {
            public bool IsValid { get; set; }
            public List<string> Errors { get; set; } = new();
        }
    }
}