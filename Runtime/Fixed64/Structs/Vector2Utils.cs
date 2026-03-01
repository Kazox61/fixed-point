using System.Runtime.CompilerServices;

namespace Fixed64
{
	public static class Vector2Utils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 RotateAround(this FVector2 vector, FVector2 rotationOrigin, FRotation2D rotation2D)
		{
			return rotation2D * (vector - rotationOrigin) + rotationOrigin;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 With(this FVector2 vector, FP? x = null, FP? y = null)
		{
			return new FVector2
			{
				X = x ?? vector.X,
				Y = y ?? vector.Y
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 FromXZ(this FVector2 vector, FP y = default) => new FVector3(vector.X, y, vector.Y);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 FromXY(this FVector2 vector, FP z = default) => new FVector3(vector.X, vector.Y, z);
	}
}
