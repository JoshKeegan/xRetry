using System;
using TechTalk.SpecFlow;

namespace UnitTests.SpecFlow.TestClasses
{
    public class ScenarioId : IEquatable<ScenarioId>
    {
        private readonly FeatureInfo featureInfo;
        private readonly ScenarioInfo scenarioInfo;
        private readonly string internalId;

        public ScenarioId(FeatureInfo featureInfo, ScenarioInfo scenarioInfo)
        {
            this.featureInfo = featureInfo;
            this.scenarioInfo = scenarioInfo;

            // FeatureInfo & ScenarioInfo don't implement equals checks, so build up a simple string that we can use as an internal ID
            internalId = $"{featureInfo.FolderPath} - {featureInfo.Title} - {scenarioInfo.Title}"; ;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ScenarioId) obj);
        }

        public bool Equals(ScenarioId other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return internalId.Equals(other.internalId);
        }

        public override int GetHashCode() => internalId.GetHashCode();

        public override string ToString() => $"Feature \"{featureInfo.Title}\", Scenario \"{scenarioInfo.Title}\"";
    }
}
