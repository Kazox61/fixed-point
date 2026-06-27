using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed64
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FAABB2 : IEquatable<FAABB2>
	{
		public FVector2 Min;
		public FVector2 Max;

		public FAABB2(FVector2 min, FVector2 max)
		{
			Min = min;
			Max = max;
		}

		public FVector2 Center
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Min + Max) * FP.Half;
		}

		public FVector2 Extents
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (Max - Min) * FP.Half;
		}
		
		public FVector2 Size
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Max - Min;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Contains(FVector2 point)
		{
			return point.X >= Min.X && point.X <= Max.X &&
					point.Y >= Min.Y && point.Y <= Max.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(FVector2 origin, FVector2 direction)
		{
			return RayIntersect(origin, direction, out _);
		}

		// Adapted from jitterphysics2
		// https://github.com/notgiven688/jitterphysics2/blob/main/src/Jitter2/LinearMath/JBoundingBox.cs
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool RayIntersect(FVector2 origin, FVector2 direction, out FP enter)
		{
			enter = FP.Zero;
			var exit = FP.MaxValue;

			if (!Intersect1D(origin.X, direction.X, Min.X, Max.X, ref enter, ref exit))
			{
				return false;
			}

			if (!Intersect1D(origin.Y, direction.Y, Min.Y, Max.Y, ref enter, ref exit))
			{
				return false;
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Intersect1D(FP start, FP dir, FP min, FP max, ref FP enter, ref FP exit)
		{
			if (dir * dir < FP.CalculationsEpsilonSqr)
			{
				return start >= min && start <= max;
			}

			var t0 = (min - start) / dir;
			var t1 = (max - start) / dir;

			if (t0 > t1)
			{
				(t0, t1) = (t1, t0);
			}

			if (t0 > exit || t1 < enter)
			{
				return false;
			}

			if (t0 > enter)
			{
				enter = t0;
			}
			if (t1 < exit)
			{
				exit = t1;
			}
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB2 FromCenterAndExtents(FVector2 center, FVector2 extents)
		{
			return new FAABB2(
				center - extents,
				center + extents
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Overlaps(FAABB2 a, FAABB2 b)
		{
			return a.Min.X <= b.Max.X && a.Max.X >= b.Min.X &&
					a.Min.Y <= b.Max.Y && a.Max.Y >= b.Min.Y;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB2 Union(FAABB2 a, FAABB2 b)
		{
			return new FAABB2(
				FVector2.MinComponents(a.Min, b.Min),
				FVector2.MaxComponents(a.Max, b.Max)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB2 Encapsulate(FAABB2 aabb, FVector2 point)
		{
			return new FAABB2(
				FVector2.MinComponents(aabb.Min, point),
				FVector2.MaxComponents(aabb.Max, point)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB2 Stretch(FAABB2 aabb, FVector2 distance)
		{
			return new FAABB2(
				FVector2.MinComponents(aabb.Min, aabb.Min + distance),
				FVector2.MaxComponents(aabb.Max, aabb.Max + distance)
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB2 Move(FAABB2 aabb, FVector2 delta)
		{
			return new FAABB2(
				aabb.Min + delta,
				aabb.Max + delta
			);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FAABB2 other)
		{
			return Min.Equals(other.Min) && Max.Equals(other.Max);
		}

		public override bool Equals(object obj)
		{
			return obj is FAABB2 other && Equals(other);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Min, Max);
		}
	}
}
