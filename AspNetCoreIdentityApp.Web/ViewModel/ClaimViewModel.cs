namespace AspNetCoreIdentityApp.Web.ViewModel
{
    public class ClaimViewModel
    {
        public string Issuer { get; set; } = null!;//claim dağıtan 
        public string Type { get; set; } = null!;
        public string Value { get; set; } = null!;
    }
}
