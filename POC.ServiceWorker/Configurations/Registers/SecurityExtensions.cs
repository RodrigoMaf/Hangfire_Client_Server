using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace POC.ServiceWorker.Configurations.Registers
{
    /// <summary>Extensões de configuração de segurança da api</summary>
    /// <remarks>
    /// <para>Para gerar certificado como dev deve executar os comandos abaixo no prompt</para>
    /// <para>dotnet dev-certs https --clean</para>
    /// <para>dotnet dev-certs https --trust</para>
    /// </remarks>
    public static class SecurityExtensions
    {
        /// <summary>Faz configuração de Antiforgery</summary>
        /// <param name="services">Serviço DI</param>
        /// <param name="configuration">Configurações do sistema</param>        
        public static IServiceCollection AddSecurityAntiforgery(this IServiceCollection services, IConfiguration configuration)
            => services
                .AddAntiforgery(
                options =>
                {
                    configuration.GetSection("Security:Antiforgery").Bind(options);
                });

        /// <summary>Faz configuração de Hsts</summary>
        /// <param name="services">Serviço DI</param>
        /// <param name="configuration">Configurações do sistema</param>        
        public static IServiceCollection AddSecurityFeatures(this IServiceCollection services, IConfiguration configuration) 
            => services
                   .AddHsts(options =>
                   {
                       configuration.GetSection("Security:Hsts").Bind(options);
                       options.ExcludedHosts.Clear();
                   })
                   .AddSecurityAntiforgery(configuration);

        /// <summary>Configura as features de segurança do serviço</summary>
        /// <param name="app">Provedor de builder da aplicação</param>
        /// <param name="environment">Provedor de configurações de ambiente do serviço</param>
        public static IApplicationBuilder UseSecurityFeatures(this IApplicationBuilder app, IWebHostEnvironment environment)
            => environment.IsDevelopment() ?
                app
                 //.UseHsts()
                 //.UseHttpsRedirection()
                 //.UseSecurityHeadersFeatures()
                 .UseDeveloperExceptionPage() 
                :
                app
                .UseHsts()
                .UseSecurityHeadersFeatures()
                .UseHttpsRedirection();

        /// <summary>Configura as features de headers de segurança do serviço</summary>
        /// <param name="app">Provedor de builder da aplicação</param>
        public static IApplicationBuilder UseSecurityHeadersFeatures(this IApplicationBuilder app)
            => app.Use((context, next) =>
                {
                    context
                    .Response
                    .OnStarting(
                        state =>
                        {
                            var httpContext = (HttpContext)state;

                            ////context.Response.Headers.Add("X-Frame-Options", "DENY");
                            ////context.Response.Headers.Remove("Server");
                            ////context.Response.Headers.Remove("X-Powered-By");
                            ////context.Response.Headers.Remove("X-SourceFiles");

                            IncludeXssProtection(httpContext);
                            IncludeCSPProtection(httpContext);
                            IncludeMimeSniffProtection(httpContext);
                            IncludeRefererPolicyProtection(httpContext);
                            IncludeCrossDomainPoliciesProtection(httpContext);
                            IncludeFeaturePolicyProtection(httpContext);
                            IncludeContentSecurityPolicyProtection(httpContext);
                            IncludeFrameOptionsProtection(httpContext);

                            return Task.CompletedTask;
                        },
                        context
                    );
                    return next();
                });

        #region Private Methods

        /// <summary>
        /// <para>Configura Cross Site Scripting Protection (Xss Protection)</para>
        /// <para>X - XSS - Protection: 0                               -> Desabilita o filtro de XSS</para>
        /// <para>X - XSS - Protection: 1                               -> Habilita o filtro de XSS. Se o browser detectar um código malicioso, irá remover e continuar o carregamento</para>
        /// <para>X - XSS - Protection: 1; mode = block                 -> Habilita o filtro, mas ao invés de remover o código malicioso o browser irá parar de carregar o conteúdo.</para>
        /// <para>X - XSS - Protection: 1; report = (reporting-uri)     -> </para>
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeXssProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("X-Xss-Protection", "1; mode = block");

        /// <summary>
        /// <para>Content Security Policy(CSP)</para>
        /// <para>Pode ser considerado uma versão melhorada do X-XSS-Protection.</para> 
        /// <para>O CSP usa o cabeçalho Content-Security-Policy. </para>
        /// <para>Com esse header o servidor cria uma white list de fontes com conteúdo confiável.</para> 
        /// <para>Instruindo o navegador a executar ou renderizar recursos somente dessas fontes</para>
        /// <para>https://owasp.org/www-project-secure-headers/</para>
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeCSPProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("Content-Security-Policy-Report-Only", "base-uri 'self'; block-all-mixed-content; default-src 'self'; img-src data: https:; object-src 'none'; upgrade-insecure-requests; report-to /api/1/Enum/Report");

        /// <summary>The value of nosniff will prevent primarily old browsers from MIME-sniffing.</summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeMimeSniffProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("X-Content-Type-Options", "nosniff");

        /// <summary>
        /// Referrer-Policy
        /// When you click a link on a website, the calling URL is automatically transferred to the linked site.
        /// Unless this is necessary, you should disable it using the Referrer-Policy header
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeRefererPolicyProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("Referrer-Policy", "no-referrer");

        /// <summary>
        /// X-Permitted-Cross-Domain-Policies
        /// You are probably not using Flash. Right? Right!!? 
        /// If not, you can disable the possibility of Flash making cross-site requests using the X-Permitted-Cross-Domain-Policies header
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeCrossDomainPoliciesProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("X-Permitted-Cross-Domain-Policies", "none");

        /// <summary>
        /// Feature-Policy
        /// The Feature-Policy header tells the browser which platform features your website needs.
        /// Most web apps won't need to access the microphone or the vibrator functions available on mobile browsers. 
        /// Why not be explicit about it to avoid imported scripts or framed pages to do things you don't expect
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeFeaturePolicyProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("Feature-Policy", "microphone'none'; payment'none'; sync-xhr 'self");
        ////"accelerometer 'none'; camera 'none'; geolocation 'none'; gyroscope 'none'; magnetometer 'none'; microphone 'none'; payment 'none'; usb 'none'; sync-xhr 'self'");

        /// <summary>
        /// Content-Security-Policy
        /// I already wrote a rather long blog post about the Content-Security-Policy header.
        /// To avoid having to repeat myself, check out Content-Security-Policy in ASP.NET MVC for details.
        /// A content security policy can be easily added in ASP.NET Core by adding the header:
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeContentSecurityPolicyProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("Content-Security-Policy", "default-src 'self'");

        /// <summary>
        /// X-Frame-Options
        /// O header x-frame-options previne o ataque conhecido como clickjacking, desativando iframes no seu site.
        /// Os iframes podem ser usados para carregar sites maliciosos.
        /// Esta técnica consiste em enganar o usuário sobre o site do qual ele realmente está, através do iframe.
        /// X-Frame-Options: deny                               -> Desabilita iframe completamente
        /// X-Frame-Options: sameorigin                         -> Permite apenas iframes do mesmo dominio
        /// X-Frame-Options: allow-from https://example.com/    -> Permite iframes de um dominio especifico
        /// </summary>
        /// <param name="httpContext">Http request context</param>
        private static void IncludeFrameOptionsProtection(HttpContext httpContext)
            => httpContext.Response.Headers.Add("X-Frame-Options", "sameorigin");

        #endregion
    }
}
