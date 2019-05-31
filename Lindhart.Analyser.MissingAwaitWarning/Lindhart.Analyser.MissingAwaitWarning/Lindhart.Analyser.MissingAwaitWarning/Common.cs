using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Lindhart.Analyser.MissingAwaitWarning
{
    internal static class Common
    {
        internal static readonly Type[] AwaitableTypes = new[]
        {
            typeof(Task),
            typeof(Task<>),
            typeof(ConfiguredTaskAwaitable),
            typeof(ConfiguredTaskAwaitable<>),
            //typeof(ValueTask), // Type not available yet in .net standard
            typeof(ValueTask<>),
            //typeof(ConfiguredValueTaskAwaitable), // Type not available yet in .net standard
            typeof(ConfiguredValueTaskAwaitable<>)
        };

        /// <summary>
        /// Checks if the <paramref name="typeSymbol"/> is one of the types specified
        /// </summary>
        /// <param name="typeSymbol"></param>
        /// <param name="semanticModel">Semantic Model of the current context</param>
        /// <param name="types">List of parameters that should match the symbol's type</param>
        /// <returns></returns>
        public static bool EqualsType(ITypeSymbol typeSymbol, SemanticModel semanticModel, params Type[] types)
        {
            var namedTypeSymbols = types.Select(x => semanticModel.Compilation.GetTypeByMetadataName(x.FullName));

            var namedSymbol = typeSymbol as INamedTypeSymbol;
            if (namedSymbol == null)
                return false;

            if (namedSymbol.IsGenericType)
                return namedTypeSymbols.Any(t => namedSymbol.ConstructedFrom.Equals(t));
            else
                return namedTypeSymbols.Any(t => typeSymbol.Equals(t));
        }
    }
}
