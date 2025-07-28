namespace POCWebAppAssignment.Model.AuthDTOs
{
    public class LoginResultDto
    {
        public bool IsValid { get; set; }
        public string? Token { get; set; }
        public string? Error { get; set; }
        public UserWithRolesDto? User { get; set; }

        public LoginResultDto(bool isValid, string? token, string? error, UserWithRolesDto? user)
        {
            IsValid = isValid;
            Token = token;
            Error = error;
            User = user;
        }
    }
}
