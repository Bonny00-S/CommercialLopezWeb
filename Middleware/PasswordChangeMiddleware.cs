namespace ProyectoWebCommercialLopez.Middleware
{
    public class PasswordChangeMiddleware
    {

        private readonly RequestDelegate _next;

        public PasswordChangeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var user = context.User;

            if (user.Identity != null && user.Identity.IsAuthenticated)
            {
                var statePassword = user.Claims.FirstOrDefault(c => c.Type == "StatePassword")?.Value ?? "0";
                var path = context.Request.Path.Value.ToLower();

                bool isChangePassword = path.StartsWith("/auth/changepassword");
                bool isLogout = path.StartsWith("/auth/logout");

                if (statePassword == "1" && !isChangePassword && !isLogout)
                {
                    context.Response.Redirect("/Auth/ChangePassword");
                    return;
                }
            }

            await _next(context);
        }
    }
}
