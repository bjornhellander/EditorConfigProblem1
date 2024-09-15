using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;

namespace Analyzer1
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class Analyzer1Analyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "Analyzer1";

        private static readonly LocalizableString Title = "Wrong options value";
        private static readonly LocalizableString MessageFormat = "Options value was '{0}'";
        private static readonly LocalizableString Description = "Wrong options value.";
        private const string Category = "Naming";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true, description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();

            context.RegisterSyntaxTreeAction(HandleSyntaxTree);
        }

        private void HandleSyntaxTree(SyntaxTreeAnalysisContext context)
        {
            var analyzerConfigOptionsProvider = context.Options.AnalyzerConfigOptionsProvider;
            var analyzerOptions = analyzerConfigOptionsProvider.GetOptions(context.Tree);

            analyzerOptions.TryGetValue("insert_final_newline", out var value);
            if (value != "true")
            {
                var location = Location.Create(context.Tree, new TextSpan(0, 1));
                var diagnostic = Diagnostic.Create(Rule, location, value);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
