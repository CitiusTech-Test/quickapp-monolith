// ---------------------------------------
// Email: quickapp@ebenmonney.com
// Templates: www.ebenmonney.com/templates
// (c) 2024 www.ebenmonney.com/mit-license
// ---------------------------------------

using System.Text.RegularExpressions;
using QuickApp.Notification.Contracts.Models;

namespace QuickApp.Notification.Service.Templating
{
    /// <summary>
    /// Deterministic, dependency-free template renderer for the POC. It performs
    /// simple <c>{{token}}</c> substitution against <see cref="NotificationRequest.Data"/>
    /// and does not load any production template files or SMTP configuration.
    /// </summary>
    public sealed partial class StubTemplateRenderer : ITemplateRenderer
    {
        public RenderedMessage Render(NotificationRequest request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var subject = Substitute($"[{request.TemplateId}]", request.Data);
            var body = request.Data.Count == 0
                ? $"Notification '{request.TemplateId}'."
                : Substitute(BuildDefaultBody(request), request.Data);

            return new RenderedMessage(subject, body);
        }

        private static string BuildDefaultBody(NotificationRequest request)
        {
            if (request.Data.TryGetValue("body", out var explicitBody))
                return explicitBody;

            var lines = request.Data
                .OrderBy(kvp => kvp.Key, StringComparer.Ordinal)
                .Select(kvp => $"{kvp.Key}: {{{{{kvp.Key}}}}}");

            return $"Notification '{request.TemplateId}'.\n" + string.Join("\n", lines);
        }

        private static string Substitute(string template, IReadOnlyDictionary<string, string> data)
        {
            return TokenPattern().Replace(template, match =>
            {
                var key = match.Groups[1].Value;
                return data.TryGetValue(key, out var value) ? value : match.Value;
            });
        }

        [GeneratedRegex(@"\{\{(\w+)\}\}")]
        private static partial Regex TokenPattern();
    }
}
