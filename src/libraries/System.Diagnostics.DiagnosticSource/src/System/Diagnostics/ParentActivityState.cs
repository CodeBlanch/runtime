// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Diagnostics
{
    public readonly struct ParentActivityState : IEquatable<ParentActivityState>
    {
        public ParentActivityState(ActivityDataRequest requestedData, ActivityContext activityContext)
        {
            RequestedData = requestedData;
            ActivityContext = activityContext;
        }

        public ActivityDataRequest RequestedData { get; }

        public ActivityContext ActivityContext { get; }

        public bool Equals(ParentActivityState other) => ActivityContext.Equals(other.ActivityContext) && RequestedData == other.RequestedData;

        public override int GetHashCode() => HashCode.Combine(RequestedData, ActivityContext);

        public override bool Equals(object? obj) => (obj is ParentActivityState context) && Equals(context);
        public static bool operator ==(ParentActivityState left, ParentActivityState right) => left.Equals(right);
        public static bool operator !=(ParentActivityState left, ParentActivityState right) => !(left == right);
    }
}
