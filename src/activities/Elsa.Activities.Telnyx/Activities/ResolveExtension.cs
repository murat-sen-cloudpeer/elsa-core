﻿using System;
using System.Threading.Tasks;
using Elsa.Activities.Telnyx.Services;
using Elsa.ActivityResults;
using Elsa.Attributes;
using Elsa.Builders;
using Elsa.Expressions;
using Elsa.Services;
using Elsa.Services.Models;

namespace Elsa.Activities.Telnyx.Activities
{
    [Action(
        Category = Constants.Category,
        Description = "Resolve an extension number into a real number (DID or SIP url).",
        Outcomes = new[] { OutcomeNames.Done, "Resolved", "Unresolved" },
        DisplayName = "Resolve Extension"
    )]
    public class ResolveExtension : Activity
    {
        private readonly IExtensionProvider _extensionProvider;

        public ResolveExtension(IExtensionProvider extensionProvider) => _extensionProvider = extensionProvider;

        [ActivityProperty(Hint = "The extension to resolve. If the extension could not be resolved, it is returned as-is.", SupportedSyntaxes = new[] { SyntaxNames.JavaScript, SyntaxNames.Liquid })]
        public string Extension { get; set; } = default!;

        protected override async ValueTask<IActivityExecutionResult> OnExecuteAsync(ActivityExecutionContext context)
        {
            var resolvedNumber = await _extensionProvider.GetAsync(Extension, context.CancellationToken);
            var result = resolvedNumber != null ? Outcome("Resolved", resolvedNumber) : Outcome("Unresolved", Extension);
            return Combine(result);
        }
    }

    public static class ResolveExtensionExtensions
    {
        public static ISetupActivity<ResolveExtension> WithExtension(this ISetupActivity<ResolveExtension> setup, Func<ActivityExecutionContext, ValueTask<string?>> value) => setup.Set(x => x.Extension, value);
        public static ISetupActivity<ResolveExtension> WithExtension(this ISetupActivity<ResolveExtension> setup, Func<ActivityExecutionContext, string?> value) => setup.Set(x => x.Extension, value);
        public static ISetupActivity<ResolveExtension> WithExtension(this ISetupActivity<ResolveExtension> setup, Func<ValueTask<string?>> value) => setup.Set(x => x.Extension, value);
        public static ISetupActivity<ResolveExtension> WithExtension(this ISetupActivity<ResolveExtension> setup, Func<string?> value) => setup.Set(x => x.Extension, value);
        public static ISetupActivity<ResolveExtension> WithExtension(this ISetupActivity<ResolveExtension> setup, string? value) => setup.Set(x => x.Extension, value);
    }
}