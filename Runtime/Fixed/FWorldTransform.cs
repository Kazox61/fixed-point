using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fixed32;

namespace Fixed {
	/// <summary>
	/// A world transform: a 64-bit world position (<see cref="FPos"/>) plus a 32-bit rotation.
	/// This is the fixed-point analog of b3WorldTransform in BOX3D_DOUBLE_PRECISION mode. Rotation
	/// is frame-local and never needs the extra range, the same split box3d uses.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FWorldTransform : IEquatable<FWorldTransform> {
		public FPos Position;
		public FQuaternion Rotation;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FWorldTransform(FPos position, FQuaternion rotation) {
			Position = position;
			Rotation = rotation;
		}

		public static FWorldTransform Identity {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FWorldTransform(FPos.Zero, FQuaternion.Identity);
		}

		/// <summary>
		/// Promote a local transform to a world transform. Lossless.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FWorldTransform FromLocal(FTransform t) {
			return new FWorldTransform(FPos.FromLocal(t.Position), t.Rotation);
		}

		/// <summary>
		/// Compose a world transform with a local transform.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FWorldTransform Mul(FWorldTransform a, FTransform b) {
			var rotation = a.Rotation * b.Rotation;
			var offset = a.Rotation * b.Position;
			return new FWorldTransform(a.Position + offset, rotation);
		}

		/// <summary>
		/// Relative transform of frame B in frame A. The narrow phase precision boundary: one
		/// 64-bit subtraction, then everything else in the 32-bit tier.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FTransform InvMul(FWorldTransform a, FWorldTransform b) {
			var invA = FQuaternion.Inverse(a.Rotation);
			var d = b.Position - a.Position;
			return new FTransform(invA * d, invA * b.Rotation);
		}

		/// <summary>
		/// Transform a local point to a world position. Rotation in the 32-bit tier, translation in the 64-bit tier.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FPos TransformPoint(FWorldTransform t, FVector3 localPoint) {
			return t.Position + t.Rotation * localPoint;
		}

		/// <summary>
		/// Transform a world position to a local point. One 64-bit subtraction, then the 32-bit tier.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 InvTransformPoint(FWorldTransform t, FPos worldPoint) {
			var d = worldPoint - t.Position;
			return FQuaternion.Inverse(t.Rotation) * d;
		}

		/// <summary>
		/// Shift a world transform into the frame of a base position.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FTransform ToRelativeTransform(FWorldTransform t, FPos basePosition) {
			return new FTransform(t.Position - basePosition, t.Rotation);
		}

		/// <summary>
		/// Translate a local AABB by a world origin, rounding outward so the 32-bit box always
		/// contains the 64-bit box. Far from the origin a plain conversion could clip a shape out
		/// of its own box.
		/// </summary>
		public static FAABB OffsetAABB(FAABB localBox, FPos origin) {
			return new FAABB(
				new FVector3(
					(origin.X + localBox.LowerBound.X.To64()).ToFP32Floor(),
					(origin.Y + localBox.LowerBound.Y.To64()).ToFP32Floor(),
					(origin.Z + localBox.LowerBound.Z.To64()).ToFP32Floor()),
				new FVector3(
					(origin.X + localBox.UpperBound.X.To64()).ToFP32Ceil(),
					(origin.Y + localBox.UpperBound.Y.To64()).ToFP32Ceil(),
					(origin.Z + localBox.UpperBound.Z.To64()).ToFP32Ceil()));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FWorldTransform other) => Position.Equals(other.Position) && Rotation.Equals(other.Rotation);

		public override bool Equals(object obj) => obj is FWorldTransform other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Position, Rotation);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FWorldTransform a, FWorldTransform b) => a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FWorldTransform a, FWorldTransform b) => !(a == b);
	}
}
