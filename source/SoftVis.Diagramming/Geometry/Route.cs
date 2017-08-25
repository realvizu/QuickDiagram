using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Codartis.SoftVis.Util;
using MoreLinq;

namespace Codartis.SoftVis.Geometry
{
    /// <summary>
    /// A route is a series of points in 2D space.
    /// </summary>
    /// <remarks>
    /// The route makes sure that all consecutive points are different.
    /// If the same point is added consecutively multiple times then it gets added only once.
    /// </remarks>
    public class Route : IEnumerable<Point2D>
    {
        public static readonly Route Empty = new Route();

        private readonly List<Point2D> _routePoints;

        public Route(IEnumerable<Point2D> routePoints = null)
        {
            _routePoints = new List<Point2D>();
            Add(routePoints);
        }

        public IEnumerator<Point2D> GetEnumerator()
        {
            return _routePoints.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(Point2D point)
        {
            if (point.IsDefined && IsNewPointInRoute(point))
                _routePoints.Add(point);
        }

        public void Add(IEnumerable<Point2D> points)
        {
            foreach (var point in points.EmptyIfNull())
                Add(point);
        }

        public bool IsDefined => _routePoints.Any();

        /// <summary>
        /// Modifies the first and last point of a route to attach to the suppied source and target rect's perimeter.
        /// </summary>
        /// <param name="sourceRect"></param>
        /// <param name="targetRect"></param>
        public void AttachToSourceRectAndTargetRect(Rect2D sourceRect, Rect2D targetRect)
        {
            var routePointCount = _routePoints.Count;
            if (routePointCount < 2)
                throw new InvalidOperationException("AttachToSourceRectAndTargetRect requires a route with at least 2 points.");

            _routePoints[0] = sourceRect.GetAttachPointToward(_routePoints[1]);
            _routePoints[routePointCount - 1] = targetRect.GetAttachPointToward(_routePoints[routePointCount - 2]);
        }

        private bool IsNewPointInRoute(Point2D point)
        {
            return !_routePoints.Any() || point != _routePoints.Last();
        }

        protected bool Equals(Route other)
        {
            return other != null && _routePoints.SequenceEqual(other._routePoints);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Route)obj);
        }

        public override int GetHashCode()
        {
            return _routePoints.GetHashCode();
        }

        public static bool operator ==(Route left, Route right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Route left, Route right)
        {
            return !Equals(left, right);
        }

        public override string ToString()
        {
            return $"[{_routePoints.ToDelimitedString("->")}]";
        }
    }
}
