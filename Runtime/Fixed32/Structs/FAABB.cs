using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed32 {
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FAABB : IEquatable<FAABB> {
		public FVector3 LowerBound;
		public FVector3 UpperBound;

		public FAABB(FVector3 lowerBound, FVector3 upperBound) {
			LowerBound = lowerBound;
			UpperBound = upperBound;
		}

		public FVector3 Center {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (LowerBound + UpperBound) * FP.Half;
		}

		public FVector3 Extents {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (UpperBound - LowerBound) * FP.Half;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(FVector3 point) {
			return point.X >= LowerBound.X && point.X <= UpperBound.X &&
				point.Y >= LowerBound.Y && point.Y <= UpperBound.Y &&
				point.Z >= LowerBound.Z && point.Z <= UpperBound.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(FVector3 origin, FVector3 direction) {
			return RayIntersect(origin, direction, out _);
		}

		// Adapted from jitterphysics2
		// https://github.com/notgiven688/jitterphysics2/blob/main/src/Jitter2/LinearMath/JBoundingBox.cs
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(FVector3 origin, FVector3 direction, out FP enter) {
			enter = FP.Zero;
			var exit = FP.MaxValue;

			if (!Intersect1D(origin.X, direction.X, LowerBound.X, UpperBound.X, ref enter, ref exit)) {
				return false;
			}

			if (!Intersect1D(origin.Y, direction.Y, LowerBound.Y, UpperBound.Y, ref enter, ref exit)) {
				return false;
			}

			if (!Intersect1D(origin.Z, direction.Z, LowerBound.Z, UpperBound.Z, ref enter, ref exit)) {
				return false;
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Intersect1D(FP start, FP dir, FP min, FP max, ref FP enter, ref FP exit) {
			if (dir * dir < FP.CalculationsEpsilonSqr) {
				return start >= min && start <= max;
			}

			var t0 = (min - start) / dir;
			var t1 = (max - start) / dir;

			if (t0 > t1) {
				(t0, t1) = (t1, t0);
			}

			if (t0 > exit || t1 < enter) {
				return false;
			}

			if (t0 > enter) {
				enter = t0;
			}
			if (t1 < exit) {
				exit = t1;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB FromCenterAndExtents(FVector3 center, FVector3 extents) {
			return new FAABB(
				center - extents,
				center + extents
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(FAABB a, FAABB b) {
			return a.LowerBound.X <= b.UpperBound.X && a.UpperBound.X >= b.LowerBound.X &&
				a.LowerBound.Y <= b.UpperBound.Y && a.UpperBound.Y >= b.LowerBound.Y &&
				a.LowerBound.Z <= b.UpperBound.Z && a.UpperBound.Z >= b.LowerBound.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB Union(FAABB a, FAABB b) {
			return new FAABB(
				FVector3.MinComponents(a.LowerBound, b.LowerBound),
				FVector3.MaxComponents(a.UpperBound, b.UpperBound)
			);
		}

		/// <summary>Does this AABB fully contain b?</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(FAABB b) {
			if (LowerBound.X > b.LowerBound.X || b.UpperBound.X > UpperBound.X) {
				return false;
			}
			if (LowerBound.Y > b.LowerBound.Y || b.UpperBound.Y > UpperBound.Y) {
				return false;
			}
			if (LowerBound.Z > b.LowerBound.Z || b.UpperBound.Z > UpperBound.Z) {
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get the surface area (perimeter, actually double surface area over one dimension) of an AABB.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Perimeter(FAABB a) {
			var w = a.UpperBound - a.LowerBound;
			return 2 * (w.X * w.Z + w.Y * w.X + w.Z * w.Y);
		}

		/// <summary>
		/// Enlarge a to contain b.
		/// </summary>
		/// <returns>true if the AABB grew.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Enlarge(ref FAABB a, FAABB b) {
			var changed = false;

			if (b.LowerBound.X < a.LowerBound.X) {
				a.LowerBound.X = b.LowerBound.X;
				changed = true;
			}
			if (b.LowerBound.Y < a.LowerBound.Y) {
				a.LowerBound.Y = b.LowerBound.Y;
				changed = true;
			}
			if (b.LowerBound.Z < a.LowerBound.Z) {
				a.LowerBound.Z = b.LowerBound.Z;
				changed = true;
			}
			if (a.UpperBound.X < b.UpperBound.X) {
				a.UpperBound.X = b.UpperBound.X;
				changed = true;
			}
			if (a.UpperBound.Y < b.UpperBound.Y) {
				a.UpperBound.Y = b.UpperBound.Y;
				changed = true;
			}
			if (a.UpperBound.Z < b.UpperBound.Z) {
				a.UpperBound.Z = b.UpperBound.Z;
				changed = true;
			}

			return changed;
		}

		/// <summary>
		/// Get the point on the AABB farthest from p (the corner diagonally opposite the closest corner).
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 FarthestPoint(FAABB b, FVector3 p) {
			return new FVector3(
				(p.X - b.LowerBound.X) > (b.UpperBound.X - p.X) ? b.LowerBound.X : b.UpperBound.X,
				(p.Y - b.LowerBound.Y) > (b.UpperBound.Y - p.Y) ? b.LowerBound.Y : b.UpperBound.Y,
				(p.Z - b.LowerBound.Z) > (b.UpperBound.Z - p.Z) ? b.LowerBound.Z : b.UpperBound.Z);
		}

		/// <summary>
		/// Ray cast an AABB using the slab method. Returns the fraction interval [minFraction, maxFraction]
		/// along the p1-p2 segment where the ray is inside the box.
		/// </summary>
		public static bool RayCast(FAABB a, FVector3 p1, FVector3 p2, out FP minFraction, out FP maxFraction) {
			minFraction = FP.Zero;
			maxFraction = FP.Zero;

			var d = p2 - p1;
			var rayLength = FVector3.Length(d);

			if (rayLength < FP.CalculationsEpsilon) {
				if (a.Contains(p1)) {
					return true;
				}

				return false;
			}

			var rayDir = d / rayLength;

			var tMin = FP.Zero;
			var tMax = rayLength;

			if (!ClipSlab(rayDir.X, p1.X, a.LowerBound.X, a.UpperBound.X, ref tMin, ref tMax)) {
				return false;
			}
			if (!ClipSlab(rayDir.Y, p1.Y, a.LowerBound.Y, a.UpperBound.Y, ref tMin, ref tMax)) {
				return false;
			}
			if (!ClipSlab(rayDir.Z, p1.Z, a.LowerBound.Z, a.UpperBound.Z, ref tMin, ref tMax)) {
				return false;
			}

			if (tMax < FP.Zero) {
				return false;
			}

			minFraction = FP.Clamp(tMin / rayLength, FP.Zero, FP.One);
			maxFraction = FP.Clamp(tMax / rayLength, FP.Zero, FP.One);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool ClipSlab(FP rayComponent, FP rayStart, FP boxMin, FP boxMax, ref FP tMin, ref FP tMax) {
			if (FP.Abs(rayComponent) < FP.CalculationsEpsilon) {
				return rayStart >= boxMin && rayStart <= boxMax;
			}

			var t1 = (boxMin - rayStart) / rayComponent;
			var t2 = (boxMax - rayStart) / rayComponent;

			if (t1 > t2) {
				(t1, t2) = (t2, t1);
			}

			tMin = FP.Max(tMin, t1);
			tMax = FP.Min(tMax, t2);

			return tMin <= tMax;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FAABB other) {
			return LowerBound.Equals(other.LowerBound) && UpperBound.Equals(other.UpperBound);
		}

		public override bool Equals(object obj) {
			return obj is FAABB other && Equals(other);
		}

		public override int GetHashCode() {
			return HashCode.Combine(LowerBound, UpperBound);
		}
	}
}
