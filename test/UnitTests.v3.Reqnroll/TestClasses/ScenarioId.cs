using System;
using Reqnroll;

namespace UnitTests.Reqnroll.v3.TestClasses
{
    public sealed class ScenarioId : IEquatable<ScenarioId>
    {
        private readonly FeatureInfo featureInfo;
        private readonly string internalId;
        private readonly ScenarioInfo scenarioInfo;

        public ScenarioId(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            this.featureInfo = featureInfo;
            this.scenarioInfo = scenarioInfo;

            // FeatureInfo & ScenarioInfo don't implement equals checks, so build up a simple string that we can use as an internal ID
            internalId = $"{featureInfo.FolderPath} - {featureInfo.Title} - {scenarioInfo.Title}";
        }

        public bool Equals(ScenarioId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return internalId.Equals(other.internalId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ScenarioId)obj);
        }

        public override int GetHashCode()
        {
            return internalId.GetHashCode();
        }

        public override string ToString()
        {
            return $"Feature \"{featureInfo.Title}\", Scenario \"{scenarioInfo.Title}\"";
        }
    }
}