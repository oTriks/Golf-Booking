namespace GolfBookingUI.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; }
        public string? Token { get; private set; }
        public string? Username { get; private set; }

        public event Action? OnAuthStateChanged;

        public void SetAuthenticationState(string? token, string? username)
        {
            Token = token;
            Username = username;
            IsAuthenticated = !string.IsNullOrWhiteSpace(token);
            OnAuthStateChanged?.Invoke();
        }

        public void ApplyBearerToken(HttpClient httpClient)
        {
            if (!string.IsNullOrEmpty(Token))
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            }
            else
            {
                httpClient.DefaultRequestHeaders.Authorization = null;
            }
        }

        public void Logout()
        {
            Token = null;
            Username = null;
            IsAuthenticated = false;
            OnAuthStateChanged?.Invoke();
        }
    }
}
