using UserManagementAPI.Exceptions;

namespace UserManagementAPI.Utilities
{
    public static class PasswordValidator
    {
        private const int MinLength = 8;

        public static void Validate(string password)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(password))
            {
                errors.Add("Password is required");
            }
            else
            {
                if (password.Length < MinLength)
                {
                    errors.Add($"Password must be at least {MinLength} characters long");
                }

                if (!password.Any(c => char.IsLetter(c)))
                {
                    errors.Add("Password must contain at least one letter");
                }

                if (!password.Any(c => char.IsDigit(c)))
                {
                    errors.Add("Password must contain at least one number");
                }
            }

            if (errors.Count > 0)
            {
                throw new ValidationException(errors);
            }
        }
    }
}