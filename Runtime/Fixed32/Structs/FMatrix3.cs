using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Fixed32 {
	/// <summary>
	/// A 3x3 matrix stored as three column vectors.
	/// </summary>
	[Serializable]
	[StructLayout(LayoutKind.Sequential)]
	public struct FMatrix3 : IEquatable<FMatrix3> {
		public FVector3 Cx;
		public FVector3 Cy;
		public FVector3 Cz;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public FMatrix3(FVector3 cx, FVector3 cy, FVector3 cz) {
			Cx = cx;
			Cy = cy;
			Cz = cz;
		}

		public static FMatrix3 Zero {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FMatrix3(FVector3.Zero, FVector3.Zero, FVector3.Zero);
		}

		public static FMatrix3 Identity {
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => new FMatrix3(FVector3.Right, FVector3.Up, FVector3.Forward);
		}

		/// <summary>
		/// Returns a diagonal matrix with the given values.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 Diagonal(FP xx, FP yy, FP zz) {
			return new FMatrix3(
				new FVector3(xx, FP.Zero, FP.Zero),
				new FVector3(FP.Zero, yy, FP.Zero),
				new FVector3(FP.Zero, FP.Zero, zz));
		}

		/// <summary>
		/// Compute the determinant of a 3-by-3 matrix.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FP Determinant(FMatrix3 m) {
			return FVector3.Dot(m.Cx, FVector3.Cross(m.Cy, m.Cz));
		}

		/// <summary>
		/// Multiply a matrix times a column vector.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FVector3 operator *(FMatrix3 m, FVector3 a) {
			return new FVector3(
				m.Cx.X * a.X + m.Cy.X * a.Y + m.Cz.X * a.Z,
				m.Cx.Y * a.X + m.Cy.Y * a.Y + m.Cz.Y * a.Z,
				m.Cx.Z * a.X + m.Cy.Z * a.Y + m.Cz.Z * a.Z);
		}

		/// <summary>
		/// Matrix multiplication.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 operator *(FMatrix3 a, FMatrix3 b) {
			return new FMatrix3(a * b.Cx, a * b.Cy, a * b.Cz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 operator +(FMatrix3 a, FMatrix3 b) {
			return new FMatrix3(a.Cx + b.Cx, a.Cy + b.Cy, a.Cz + b.Cz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 operator -(FMatrix3 a, FMatrix3 b) {
			return new FMatrix3(a.Cx - b.Cx, a.Cy - b.Cy, a.Cz - b.Cz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 operator -(FMatrix3 a) {
			return new FMatrix3(-a.Cx, -a.Cy, -a.Cz);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 operator *(FP s, FMatrix3 a) {
			return new FMatrix3(a.Cx * s, a.Cy * s, a.Cz * s);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 operator *(FMatrix3 a, FP s) {
			return s * a;
		}

		/// <summary>
		/// Matrix transpose.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 Transpose(FMatrix3 m) {
			return new FMatrix3(
				new FVector3(m.Cx.X, m.Cy.X, m.Cz.X),
				new FVector3(m.Cx.Y, m.Cy.Y, m.Cz.Y),
				new FVector3(m.Cx.Z, m.Cy.Z, m.Cz.Z));
		}

		/// <summary>
		/// Get the component-wise absolute value of a matrix.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static FMatrix3 Abs(FMatrix3 m) {
			return new FMatrix3(FVector3.AbsComponents(m.Cx), FVector3.AbsComponents(m.Cy), FVector3.AbsComponents(m.Cz));
		}

		/// <summary>
		/// General matrix inverse. Returns <see cref="Zero"/> if the matrix is not invertible.
		/// </summary>
		public static FMatrix3 Invert(FMatrix3 m) {
			var det = Determinant(m);
			if (FP.Abs(det) > FP.CalculationsEpsilon) {
				var invDet = FP.One / det;
				var outCx = invDet * FVector3.Cross(m.Cy, m.Cz);
				var outCy = invDet * FVector3.Cross(m.Cz, m.Cx);
				var outCz = invDet * FVector3.Cross(m.Cx, m.Cy);
				return Transpose(new FMatrix3(outCx, outCy, outCz));
			}

			return Zero;
		}

		/// <summary>
		/// The inverse of the transpose of a matrix, equivalent to the transpose of the inverse.
		/// Commonly used to transform normal vectors under a non-uniform scale or shear.
		/// Returns <see cref="Zero"/> if the matrix is not invertible.
		/// </summary>
		public static FMatrix3 InvertTranspose(FMatrix3 m) {
			var det = Determinant(m);
			if (FP.Abs(det) > FP.CalculationsEpsilon) {
				var invDet = FP.One / det;
				var outCx = invDet * FVector3.Cross(m.Cy, m.Cz);
				var outCy = invDet * FVector3.Cross(m.Cz, m.Cx);
				var outCz = invDet * FVector3.Cross(m.Cx, m.Cy);
				return new FMatrix3(outCx, outCy, outCz);
			}

			return Zero;
		}

		/// <summary>
		/// Solve a matrix equation. Returns inv(m) * a. Returns zero if the matrix is not invertible.
		/// </summary>
		public static FVector3 Solve(FMatrix3 m, FVector3 a) {
			var det = Determinant(m);
			if (FP.Abs(det) > FP.CalculationsEpsilon) {
				var invDet = FP.One / det;
				var sx = FVector3.Cross(m.Cy, m.Cz);
				var sy = FVector3.Cross(m.Cz, m.Cx);
				var sz = FVector3.Cross(m.Cx, m.Cy);
				return new FVector3(
					invDet * FVector3.Dot(sx, a),
					invDet * FVector3.Dot(sy, a),
					invDet * FVector3.Dot(sz, a));
			}

			return FVector3.Zero;
		}

		/// <summary>
		/// Make a matrix from a quaternion. Useful if you need to rotate many vectors.
		/// </summary>
		public static FMatrix3 FromQuaternion(FQuaternion q) {
			var xx = q.X * q.X;
			var yy = q.Y * q.Y;
			var zz = q.Z * q.Z;
			var xy = q.X * q.Y;
			var xz = q.X * q.Z;
			var xw = q.X * q.W;
			var yz = q.Y * q.Z;
			var yw = q.Y * q.W;
			var zw = q.Z * q.W;

			return new FMatrix3(
				new FVector3(FP.One - 2 * (yy + zz), 2 * (xy + zw), 2 * (xz - yw)),
				new FVector3(2 * (xy - zw), FP.One - 2 * (xx + zz), 2 * (yz + xw)),
				new FVector3(2 * (xz + yw), 2 * (yz - xw), FP.One - 2 * (xx + yy)));
		}

		/// <summary>
		/// Compute the inertia tensor of a solid sphere about its center.
		/// </summary>
		public static FMatrix3 SphereInertia(FP mass, FP radius) {
			var i = FP.FromRatio(4, 10) * mass * radius * radius;
			return Diagonal(i, i, i);
		}

		/// <summary>
		/// Compute the inertia tensor of a solid cylinder about its center, aligned with the y-axis.
		/// </summary>
		public static FMatrix3 CylinderInertia(FP mass, FP radius, FP height) {
			var ixx = mass * (3 * radius * radius + height * height) / 12;
			var iyy = FP.Half * mass * radius * radius;
			return Diagonal(ixx, iyy, ixx);
		}

		/// <summary>
		/// Compute the inertia tensor of a solid box about its center, given the local min/max corners.
		/// </summary>
		public static FMatrix3 BoxInertia(FP mass, FVector3 min, FVector3 max) {
			var delta = max - min;
			var ixx = mass * (delta.Y * delta.Y + delta.Z * delta.Z) / 12;
			var iyy = mass * (delta.X * delta.X + delta.Z * delta.Z) / 12;
			var izz = mass * (delta.X * delta.X + delta.Y * delta.Y) / 12;
			return Diagonal(ixx, iyy, izz);
		}

		/// <summary>
		/// Get the inertia tensor of an offset point mass.
		/// https://en.wikipedia.org/wiki/Parallel_axis_theorem
		/// </summary>
		public static FMatrix3 Steiner(FP mass, FVector3 origin) {
			var ixx = mass * (origin.Y * origin.Y + origin.Z * origin.Z);
			var iyy = mass * (origin.X * origin.X + origin.Z * origin.Z);
			var izz = mass * (origin.X * origin.X + origin.Y * origin.Y);
			var ixy = -mass * origin.X * origin.Y;
			var ixz = -mass * origin.X * origin.Z;
			var iyz = -mass * origin.Y * origin.Z;

			return new FMatrix3(
				new FVector3(ixx, ixy, ixz),
				new FVector3(ixy, iyy, iyz),
				new FVector3(ixz, iyz, izz));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(FMatrix3 other) => Cx.Equals(other.Cx) && Cy.Equals(other.Cy) && Cz.Equals(other.Cz);

		public override bool Equals(object obj) => obj is FMatrix3 other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(Cx, Cy, Cz);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(FMatrix3 a, FMatrix3 b) => a.Equals(b);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(FMatrix3 a, FMatrix3 b) => !(a == b);
	}
}
