using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyModel;
using NLog;

namespace Rocinante
{
    /// <summary>
    /// A helper class for locating all plugins in the bin directory that reference Rocinante.Types
    /// 
    /// Based off of https://github.com/aspnet/Mvc/blob/dev/src/Microsoft.AspNetCore.Mvc.Core/Internal/DefaultAssemblyPartDiscoveryProvider.cs
    /// which is licensed under the Apache 2.0 license
    /// </summary>
    internal class DependencyResolver
    {
        private readonly Logger Log = LogManager.GetCurrentClassLogger();

        // Returns a list of libraries that references Rocinante.Types.
        internal static IEnumerable<RuntimeLibrary> GetCandidateLibraries(DependencyContext dependencyContext)
        {
            var candidatesResolver = new DependencyResolver(dependencyContext.RuntimeLibraries);
            return candidatesResolver.GetCandidates();
        }

        private readonly IDictionary<string, Dependency> _runtimeDependencies;

        public DependencyResolver(IReadOnlyList<RuntimeLibrary> runtimeDependencies)
        {
            var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
            foreach (var dependency in runtimeDependencies)
            {
                if (dependenciesWithNoDuplicates.ContainsKey(dependency.Name))
                {
                    throw new InvalidOperationException($"Duplicate Dependency Found: {dependency.Name}");
                }
                dependenciesWithNoDuplicates.Add(dependency.Name, CreateDependency(dependency));
            }

            _runtimeDependencies = dependenciesWithNoDuplicates;
        }

        private Dependency CreateDependency(RuntimeLibrary library)
        {
            var classification = DependencyClassification.Unknown;
            if (library.Name == "rocinante.types" || library.Name == "rocinante")
            {
                classification = DependencyClassification.Self;
            }

            return new Dependency(library, classification);
        }

        private DependencyClassification ComputeClassification(string dependency)
        {
            if (!_runtimeDependencies.ContainsKey(dependency))
            {
                // Library does not have runtime dependency. Since we can't infer
                // anything about it's references, we'll assume it does not have a reference to Mvc.
                return DependencyClassification.DoesNotReferenceRocinante;
            }

            var candidateEntry = _runtimeDependencies[dependency];
            if (candidateEntry.Classification != DependencyClassification.Unknown)
            {
                return candidateEntry.Classification;
            }
            else
            {
                var classification = DependencyClassification.DoesNotReferenceRocinante;
                foreach (var candidateDependency in candidateEntry.Library.Dependencies)
                {
                    var dependencyClassification = ComputeClassification(candidateDependency.Name);
                    if (dependencyClassification == DependencyClassification.ReferencesRocinante ||
                        dependencyClassification == DependencyClassification.Self)
                    {
                        classification = DependencyClassification.ReferencesRocinante;
                        break;
                    }
                }

                candidateEntry.Classification = classification;

                return classification;
            }
        }

        public IEnumerable<RuntimeLibrary> GetCandidates()
        {
            foreach (var dependency in _runtimeDependencies)
            {
                if (ComputeClassification(dependency.Key) == DependencyClassification.ReferencesRocinante)
                {
                    Log.Trace("{0} references Rocinante.Types", dependency.Key);
                    yield return dependency.Value.Library;
                }
            }
        }

        private class Dependency
        {
            public Dependency(RuntimeLibrary library, DependencyClassification classification)
            {
                Library = library;
                Classification = classification;
            }

            public RuntimeLibrary Library { get; }

            public DependencyClassification Classification { get; set; }

            public override string ToString()
            {
                return $"Library: {Library.Name}, Classification: {Classification}";
            }
        }

        private enum DependencyClassification
        {
            Unknown = 0,

            /// <summary>
            /// References (directly or transitively) Rocinante.Types
            /// </summary>
            ReferencesRocinante = 1,

            /// <summary>
            /// Does not reference (directly or transitively) Rocinante.Types
            /// </summary>
            DoesNotReferenceRocinante = 2,

            /// <summary>
            /// Rocinante or Rocinante.Types
            /// </summary>
            Self = 3,
        }
    }
}