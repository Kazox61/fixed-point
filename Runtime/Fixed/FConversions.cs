using System.Runtime.CompilerServices;
using Fixed32;

// ReSharper disable ShiftExpressionRightOperandNotEqualRealCount
// ReSharper disable HeuristicUnreachableCode
namespace Fixed {
	public static class FConversions {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
<<<<<<< HEAD
		public static Fixed64.FP To64(this FP value)
		{
#pragma warning disable CS0162 // Unreachable code detected
			if (Fixed64.FP.FractionalBits - FP.FractionalBits >= 0)
			{
				return Fixed64.FP.FromRaw((long)value.RawValue << (Fixed64.FP.FractionalBits - FP.FractionalBits));
			}
			else
			{
				return Fixed64.FP.FromRaw((long)value.RawValue >> (FP.FractionalBits - Fixed64.FP.FractionalBits));
=======
		public static FP64 To64(this FP32 value) {
#pragma warning disable CS0162 // Unreachable code detected
			if (FP64.FractionalBits - FP32.FractionalBits >= 0) {
				return FP64.FromRaw((long)value.RawValue << (FP64.FractionalBits - FP32.FractionalBits));
			}
			else {
				return FP64.FromRaw((long)value.RawValue >> (FP32.FractionalBits - FP64.FractionalBits));
>>>>>>> d0c32a2 (Add math for box3d physics)
			}
#pragma warning restore CS0162 // Unreachable code detected
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
<<<<<<< HEAD
		public static FP To32(this Fixed64.FP value)
		{
#pragma warning disable CS0162 // Unreachable code detected
			if (Fixed64.FP.FractionalBits - FP.FractionalBits >= 0)
			{
				return FP.FromRaw((int)(value.RawValue >> (Fixed64.FP.FractionalBits - FP.FractionalBits)));
			}
			else
			{
				return FP.FromRaw((int)(value.RawValue << (FP.FractionalBits - Fixed64.FP.FractionalBits)));
=======
		public static FP32 To32(this FP64 value) {
#pragma warning disable CS0162 // Unreachable code detected
			if (FP64.FractionalBits - FP32.FractionalBits >= 0) {
				return FP32.FromRaw((int)(value.RawValue >> (FP64.FractionalBits - FP32.FractionalBits)));
			}
			else {
				return FP32.FromRaw((int)(value.RawValue << (FP32.FractionalBits - FP64.FractionalBits)));
>>>>>>> d0c32a2 (Add math for box3d physics)
			}
#pragma warning restore CS0162 // Unreachable code detected
		}

<<<<<<< HEAD
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FVector2 To64(this FVector2 value)
			=> new Fixed64.FVector2(value.X.To64(), value.Y.To64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 To32(this Fixed64.FVector2 value)
			=> new FVector2(value.X.To32(), value.Y.To32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FVector3 To64(this FVector3 value)
			=> new Fixed64.FVector3(value.X.To64(), value.Y.To64(), value.Z.To64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 To32(this Fixed64.FVector3 value)
			=> new FVector3(value.X.To32(), value.Y.To32(), value.Z.To32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FQuaternion To64(this FQuaternion value)
			=> new Fixed64.FQuaternion(value.X.To64(), value.Y.To64(), value.Z.To64(), value.W.To64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FQuaternion To32(this Fixed64.FQuaternion value)
			=> new FQuaternion(value.X.To32(), value.Y.To32(), value.Z.To32(), value.W.To32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FAABB To64(this FAABB value)
			=> new Fixed64.FAABB(value.LowerBound.To64(), value.UpperBound.To64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB To32(this Fixed64.FAABB value)
			=> new FAABB(value.LowerBound.To32(), value.UpperBound.To32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FAABB2 To64(this FAABB2 value)
			=> new Fixed64.FAABB2(value.Min.To64(), value.Max.To64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAABB2 To32(this Fixed64.FAABB2 value)
			=> new FAABB2(value.Min.To32(), value.Max.To32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FAngle To64(this FAngle value)
			=> Fixed64.FAngle.FromRadians(value.Radians.To64());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FAngle To32(this Fixed64.FAngle value)
			=> FAngle.FromRadians(value.Radians.To32());

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Fixed64.FRotation2D To64(this FRotation2D value)
			=> new Fixed64.FRotation2D { Sin = value.Sin.To64(), OneMinusCos = value.OneMinusCos.To64() };

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FRotation2D To32(this Fixed64.FRotation2D value)
			=> new FRotation2D { Sin = value.Sin.To32(), OneMinusCos = value.OneMinusCos.To32() };
=======
		/// <summary>
		/// Narrow a 64-bit value to 32-bit, rounding toward negative infinity. This is exactly what
		/// <see cref="To32"/> already does (a right-shift on a signed integer floors), named here so
		/// the intent at a precision boundary (e.g. building a conservative bounding box) is explicit.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP32 ToFP32Floor(this FP64 value) {
			return value.To32();
		}

		/// <summary>
		/// Narrow a 64-bit value to 32-bit, rounding toward positive infinity. Use with
		/// <see cref="ToFP32Floor"/> to build a conservative box that always contains the 64-bit
		/// bounds, where a plain narrowing conversion could clip.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP32 ToFP32Ceil(this FP64 value) {
#pragma warning disable CS0162 // Unreachable code detected
			if (FP64.FractionalBits - FP32.FractionalBits >= 0) {
				var shift = FP64.FractionalBits - FP32.FractionalBits;
				var mask = (1L << shift) - 1;
				var floored = value.RawValue >> shift;
				if ((value.RawValue & mask) != 0) {
					floored += 1;
				}

				return FP32.FromRaw((int)floored);
			}
			else {
				// Widening: no precision is lost, so ceiling and floor coincide.
				return value.To32();
			}
#pragma warning restore CS0162 // Unreachable code detected
		}
>>>>>>> d0c32a2 (Add math for box3d physics)
	}
}
