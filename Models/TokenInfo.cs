namespace AllbotHttp
{
    public class TokenInfo
    {

        public string access_token { get; set; }
        public string token_type { get; set; }

        public int expires_in { get; set; }

        public override string ToString()
        {
            return $"{access_token} ,\n\n{token_type} ,\n{expires_in}";
        }
    }
}
