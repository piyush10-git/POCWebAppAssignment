namespace POCWebAppAssignment.Model.AuthDTOs
{
    public class JwtConfig
    {
        public string jwtKey { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public string expiresIn { get; set; }
    }
}
