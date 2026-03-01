using System.Runtime.CompilerServices;

namespace Fixed32
{
	public static class Vector3Utils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 RotateAround(this FVector3 vector, FVector3 rotationOrigin, FQuaternion rotation)
		{
			return rotation * (vector - rotationOrigin) + rotationOrigin;
		}
		
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 With(this FVector3 vector, FP? x = null, FP? y = null, FP? z = null)
		{
			return new FVector3
			{
				X = x ?? vector.X,
				Y = y ?? vector.Y,
				Z = z ?? vector.Z
			};
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 ToXZ(this FVector3 vector) => new FVector2(vector.X, vector.Z);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector2 ToXY(this FVector3 vector) => new FVector2(vector.X, vector.Y);
	}
}
