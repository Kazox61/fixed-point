using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Fixed32;
using FP64 = Fixed64.FP;

namespace Fixed {
	/// <summary>
	/// A world position using 64-bit fixed point components so coordinates stay accurate far from
	/// the origin. This is the fixed-point analog of b3Pos in BOX3D_DOUBLE_PRECISION mode: local,
	/// shape-space math stays in the 32-bit tier (<see cref="FVector3"/>), while body world positions
	/// live here.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FPos : IEquatable<FPos> {
		public FP64 X;
		public FP64 Y;
		public FP64 Z;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FPos(FP64 x, FP64 y, FP64 z) {
			X = x;
			Y = y;
			Z = z;
		}

		public static FPos Zero {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FPos(FP64.Zero, FP64.Zero, FP64.Zero);
		}

		/// <summary>
		/// Promote a local vector to a world position. Lossless.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FPos FromLocal(FVector3 v) {
			return new FPos(v.X.To64(), v.Y.To64(), v.Z.To64());
		}

		/// <summary>
		/// p + d
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FPos operator +(FPos p, FVector3 d) {
			return new FPos(p.X + d.X.To64(), p.Y + d.Y.To64(), p.Z + d.Z.To64());
		}

		/// <summary>
		/// a - b, demoted to a local vector. The primary precision boundary operation.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator -(FPos a, FPos b) {
			return new FVector3((a.X - b.X).To32(), (a.Y - b.Y).To32(), (a.Z - b.Z).To32());
		}

		/// <summary>
		/// World position interpolation for sweeps and sampling.
		/// </summary>
		public static FPos Lerp(FPos a, FPos b, FP t) {
			var t64 = t.To64();
			var oneMinusT = FP64.One - t64;
			return new FPos(
				oneMinusT * a.X + t64 * b.X,
				oneMinusT * a.Y + t64 * b.Y,
				oneMinusT * a.Z + t64 * b.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FPos other) => X == other.X && Y == other.Y && Z == other.Z;

		public override bool Equals(object obj) => obj is FPos other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(X, Y, Z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FPos a, FPos b) => a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FPos a, FPos b) => !(a == b);

		public override string ToString() => $"({X}, {Y}, {Z})";
	}
}
