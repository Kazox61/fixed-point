using System.Runtime.CompilerServices;
using FP32 = Fixed32.FP;
using FP64 = Fixed64.FP;

// ReSharper disable ShiftExpressionRightOperandNotEqualRealCount
// ReSharper disable HeuristicUnreachableCode
namespace Fixed
{
	public static class FConversions
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP64 To64(this FP32 value)
		{
#pragma warning disable CS0162 // Unreachable code detected
			if (FP64.FractionalBits - FP32.FractionalBits >= 0)
			{
				return FP64.FromRaw((long)value.RawValue << (FP64.FractionalBits - FP32.FractionalBits));
			}
			else
			{
				return FP64.FromRaw((long)value.RawValue >> (FP32.FractionalBits - FP64.FractionalBits));
			}
#pragma warning restore CS0162 // Unreachable code detected
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP32 To32(this FP64 value)
		{
#pragma warning disable CS0162 // Unreachable code detected
			if (FP64.FractionalBits - FP32.FractionalBits >= 0)
			{
				return FP32.FromRaw((int)(value.RawValue >> (FP64.FractionalBits - FP32.FractionalBits)));
			}
			else
			{
				return FP32.FromRaw((int)(value.RawValue << (FP32.FractionalBits - FP64.FractionalBits)));
			}
#pragma warning restore CS0162 // Unreachable code detected
		}
	}
}
