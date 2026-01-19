namespace Restaurant.API.Middlewares.languageMid
{
    public static class RequestLanguesMidelwareExtentions
    {
        public static IApplicationBuilder UseRequestLanguesMidelware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLanguesMidelware>();
        }
    }
}
