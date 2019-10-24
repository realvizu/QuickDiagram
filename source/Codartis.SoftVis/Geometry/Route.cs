using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codartis.Util;
using static MoreLinq.Extensions.ToDelimitedStringExtension;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// A route is a series of points in 2D space.
    /// Immutable.
    /// </summary>
    /// <remarks>
    /// The route makes sure that all consecutive points are different.
    /// If the same point is added consecutively multiple times then it gets added only once.
    /// </remarks>
    public struct Route : IEnumerable<Point2D>, IEquatable<Route>
    {
        public static readonly Route Empty = new Route();

        private readonly ImmutableList<Point2D> _routePoints;

        public Route(IEnumerable<Point2D> routePoints = null)
        {
            var normalizedRoutePoints = Normalize(routePoints).ToImmutableList();

            _routePoints = normalizedRoutePoints.Any()
                ? normalizedRoutePoints
                : null;
        }

        public Route(params Point2D[] routePoints)
            : this(routePoints as IEnumerable<Point2D>)
        {
        }

        public IEnumerator<Point2D> GetEnumerator() => _routePoints?.GetEnumerator() ?? Enumerable.Empty<Point2D>().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool IsDefined => _routePoints?.Any() == true;

        public Route AddPoint(Point2D point) => Add(point.ToEnumerable());

        public Route Add(IEnumerable<Point2D> points)
        {
            return points == null
                ? this
                : new Route(_routePoints?.Concat(points) ?? points);
        }

        public Route Translate(Point2D vector) => new Route(_routePoints?.Select(i => i + vector));

        /// <summary>
        /// Modifies the first and last point of a route to attach to the supplied source and target rect's perimeter.
        /// </summary>
        public Route AttachToSourceRectAndTargetRect(Rect2D sourceRect, Rect2D targetRect)
        {
            if (_routePoints == null || _routePoints.Count < 2)
                throw new InvalidOperationException("AttachToSourceRectAndTargetRect requires a route with at least 2 points.");

            var firstPoint = sourceRect.GetAttachPointToward(_routePoints[1]);
            var lastPoint = targetRect.GetAttachPointToward(_routePoints[_routePoints.Count - 2]);

            return new Route(firstPoint.Concat(_routePoints.Skip(1).TakeButLast()).Append(lastPoint));
        }

        public bool Equals(Route other)
        {
            return (_routePoints == null && other._routePoints == null) ||
                   (_routePoints != null && other._routePoints != null && _routePoints.SequenceEqual(other._routePoints));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;

            return obj is Route && Equals((Route)obj);
        }

        public override int GetHashCode()
        {
            return (_routePoints != null ? _routePoints.GetHashCode() : 0);
        }

        public static bool operator ==(Route left, Route right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Route left, Route right)
        {
            return !left.Equals(right);
        }

        public override string ToString()
        {
            return $"[{_routePoints?.ToDelimitedString("->")}]";
        }

        /// <summary>
        /// Substitutes consecutive same points with just one instance of that point and filters out undefined points.
        /// </summary>
        /// <param name="routePoints">A collection of points.</param>
        /// <returns>Normalized collection of points.</returns>
        private static IEnumerable<Point2D> Normalize(IEnumerable<Point2D> routePoints)
        {
            if (routePoints == null)
                yield break;

            Point2D? previousPoint = null;
            foreach (var routePoint in routePoints.Where(i => i.IsDefined))
            {
                if (routePoint == previousPoint)
                    continue;

                yield return routePoint;

                previousPoint = routePoint;
            }
        }

        public class Builder : IEnumerable<Point2D>
        {
            private readonly List<Point2D> _builderPoints;

            public Builder(List<Point2D> builderPoints = null)
            {
                _builderPoints = builderPoints ?? new List<Point2D>();
            }

            public IEnumerator<Point2D> GetEnumerator() => _builderPoints.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void Add(Point2D point) => _builderPoints.Add(point);

            public void Add(IEnumerable<Point2D> points)
            {
                foreach (var point in points)
                    Add(point);
            }

            public Route ToRoute() => new Route(_builderPoints);
        }
    }
}