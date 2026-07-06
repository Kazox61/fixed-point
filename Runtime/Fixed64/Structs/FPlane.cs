using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed64 {
	/// <summary>
	/// A plane. separation = dot(normal, point) - offset
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FPlane : IEquatable<FPlane> {
		public FVector3 Normal;
		public FP Offset;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FPlane(FVector3 normal, FP offset) {
			Normal = normal;
			Offset = offset;
		}

		/// <summary>
		/// Signed distance from the plane to a point. Positive when the point is on the side the normal points to.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Separation(FPlane plane, FVector3 point) {
			return FVector3.Dot(plane.Normal, point) - plane.Offset;
		}

		/// <summary>Build a plane from a normal and a point on the plane.</summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FPlane FromNormalAndPoint(FVector3 normal, FVector3 point) {
			return new FPlane(normal, FVector3.Dot(normal, point));
		}

		/// <summary>Build a plane from three points, in counter-clockwise winding order.</summary>
		public static FPlane FromPoints(FVector3 point1, FVector3 point2, FVector3 point3) {
			var normal = FVector3.Normalize(FVector3.Cross(point2 - point1, point3 - point1));
			return new FPlane(normal, FVector3.Dot(normal, point1));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FPlane other) => Normal.Equals(other.Normal) && Offset == other.Offset;

		public override bool Equals(object obj) => obj is FPlane other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Normal, Offset);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FPlane a, FPlane b) => a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FPlane a, FPlane b) => !(a == b);
	}
}
