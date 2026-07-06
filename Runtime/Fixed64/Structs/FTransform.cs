using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed64 {
	/// <summary>
	/// A rigid transform: a position and a rotation.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FTransform : IEquatable<FTransform> {
		public FVector3 Position;
		public FQuaternion Rotation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FTransform(FVector3 position, FQuaternion rotation) {
			Position = position;
			Rotation = rotation;
		}

		public static FTransform Identity {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FTransform(FVector3.Zero, FQuaternion.Identity);
		}

		/// <summary>
		/// Multiply two transforms. If the result is applied to a point p local to frame B,
		/// the transform would first convert p to a point local to frame A, then into a point
		/// in the world frame. This is useful if frame B is a child of frame A.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FTransform Mul(FTransform a, FTransform b) {
			return new FTransform(a.Rotation * b.Position + a.Position, a.Rotation * b.Rotation);
		}

		/// <summary>
		/// Creates a transform that converts a local point in frame B to a local point in frame A.
		/// This is useful for transforming points between the local spaces of two frames that are
		/// in world space.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FTransform InvMul(FTransform a, FTransform b) {
			var invA = FQuaternion.Inverse(a.Rotation);
			return new FTransform(invA * (b.Position - a.Position), invA * b.Rotation);
		}

		/// <summary>
		/// Get the inverse of a transform.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FTransform Invert(FTransform t) {
			var invRotation = FQuaternion.Inverse(t.Rotation);
			return new FTransform(invRotation * -t.Position, invRotation);
		}

		/// <summary>
		/// Transform a point.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 TransformPoint(FTransform t, FVector3 v) {
			return t.Rotation * v + t.Position;
		}

		/// <summary>
		/// Inverse transform a point.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 InvTransformPoint(FTransform t, FVector3 v) {
			return FQuaternion.Inverse(t.Rotation) * (v - t.Position);
		}

		/// <summary>
		/// Transform an axis-aligned bounding box. This can create a larger box
		/// than if you recomputed the AABB of the original shape with the transform applied.
		/// </summary>
		public static FAABB TransformAABB(FTransform t, FAABB a) {
			var center = TransformPoint(t, a.Center);
			var m = FMatrix3.FromQuaternion(t.Rotation);
			var extent = FMatrix3.Abs(m) * a.Extents;
			return new FAABB(center - extent, center + extent);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FTransform operator *(FTransform a, FTransform b) => Mul(a, b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FTransform t, FVector3 v) => TransformPoint(t, v);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FTransform other) => Position.Equals(other.Position) && Rotation.Equals(other.Rotation);

		public override bool Equals(object obj) => obj is FTransform other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Position, Rotation);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FTransform a, FTransform b) => a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FTransform a, FTransform b) => !(a == b);
	}
}
