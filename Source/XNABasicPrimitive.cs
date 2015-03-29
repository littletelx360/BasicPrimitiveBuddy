﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace BasicPrimitiveBuddy
{
	/// <summary>
	/// Render a simple 2D shape.
	/// </summary>
	public class XnaBasicPrimitive : IBasicPrimitive
	{
		#region Fields

		/// <summary>
		/// The position of the primitive object.
		/// </summary>
		private Vector2 _position = Vector2.Zero;

		/// <summary>
		/// The sprite batch this dude gonna render with.
		/// </summary>
		private readonly SpriteBatch _spriteBatch;

		#endregion //Fields

		#region Properties

		/// <summary>
		/// Blank 1x1 pixel that creates the shape.
		/// </summary>
		public Texture2D Texture { get; set; }

		/// <summary>
		/// Get/Set the colour of the primitive object.
		/// </summary>
		private Color Color { get; set; }

		/// <summary>
		/// List of vectors.
		/// </summary>
		private List<Vector2> VectorList { get; set; }

		/// <summary>
		/// Get/Set the position of the primitive object.
		/// </summary>
		public Vector2 Position
		{
			get { return _position; }
			set { _position = value; }
		}

		/// <summary>
		/// Get/Set the render depth of the primitive line object (0 = front, 1 = back).
		/// </summary>
		public float Depth { get; set; }

		/// <summary>
		/// Get/Set the thickness of the shape's edge.
		/// </summary>
		public float Thickness { get; set; }

		public int NumCircleSegments { get; set; }

		#endregion // Properties

		#region Initialization

		/// <summary>
		/// Creates a new primitive object.
		/// </summary>
		/// <param name="graphicsDevice">The graphics device object to use.</param>
		/// <param name="spritebatch">The spritebatch object to use.</param>
		public XnaBasicPrimitive(GraphicsDevice graphicsDevice, SpriteBatch spritebatch)
		{
			NumCircleSegments = 20;
			_spriteBatch = spritebatch;
			VectorList = new List<Vector2>();
			Color = Color.White;

			// Create the pixel texture.
			Texture = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
			Texture.SetData<Color>(new Color[] { Color.White });
			Thickness = 1.0f;
		}

		/// <summary>
		/// get ready to darw a new primitive
		/// </summary>
		private void Clear()
		{
			Color = Color.White;
			Position = Vector2.Zero;
			VectorList.Clear();
		}

		#endregion // Initialization

		#region Creation Methods

		/// <summary> 
		/// Create a line primitive.
		/// </summary>
		/// <param name="start">Start of the line, in pixels.</param>
		/// <param name="end">End of the line, in pixels.</param>
		private void CreateLine(Vector2 start, Vector2 end)
		{
			VectorList.Clear();
			VectorList.Add(start);
			VectorList.Add(end);
		}

		/// <summary>
		/// Create a triangle primitive.
		/// </summary>
		/// <param name="p1">Fist point, in pixels.</param>
		/// <param name="p2">Second point, in pixels.</param>
		/// <param name="p3">Third point, in pixels.</param>
		private void CreateTriangle(Vector2 p1, Vector2 p2, Vector2 p3)
		{
			VectorList.Clear();
			VectorList.Add(p1);
			VectorList.Add(p2);
			VectorList.Add(p3);
			VectorList.Add(p1);
		}

		/// <summary>
		/// Create a square primitive.
		/// </summary>
		/// <param name="topLeft">Top left hand corner of the square.</param>
		/// <param name="bottomRight">Bottom right hand corner of the square.</param>
		private void CreateSquare(Vector2 topLeft, Vector2 bottomRight)
		{
			VectorList.Clear();
			VectorList.Add(topLeft);
			VectorList.Add(new Vector2(topLeft.X, bottomRight.Y));
			VectorList.Add(bottomRight);
			VectorList.Add(new Vector2(bottomRight.X, topLeft.Y));
			VectorList.Add(topLeft);
		}

		/// <summary>
		/// Creates a circle starting from (0, 0).
		/// </summary>
		/// <param name="radius">The radius (half the width) of the circle.</param>
		/// <param name="sides">The number of sides on the circle. (64 is average).</param>
		private void CreateCircle(float radius, int sides)
		{
			VectorList.Clear();

			float fMax = (float)MathHelper.TwoPi;
			float fStep = fMax / (float)sides;

			// Create the full circle.
			for (float fTheta = fMax; fTheta >= -1; fTheta -= fStep)
			{
				VectorList.Add(new Vector2(radius * (float)Math.Cos((double)fTheta),
											 radius * (float)Math.Sin((double)fTheta)));
			}
		}

		/// <summary>
		/// Creates an ellipse starting from (0, 0) with the given width and height.
		/// Vectors are generated using the parametric equation of an ellipse.
		/// </summary>
		/// <param name="semiMajorAxis">The width of the ellipse at its center.</param>
		/// <param name="semiMinorAxis">The height of the ellipse at its center.</param>
		/// <param name="sides">The number of sides on the ellipse. (64 is average).</param>
		private void CreateEllipse(float semiMajorAxis, float semiMinorAxis, int sides)
		{
			VectorList.Clear();

			// Local variables.
			float fMax = (float)MathHelper.TwoPi;
			float fStep = fMax / (float)sides;

			// Create full ellipse.
			for (float fTheta = fMax; fTheta >= -1; fTheta -= fStep)
			{
				VectorList.Add(new Vector2((float)(semiMajorAxis * Math.Cos(fTheta)),
											 (float)(semiMinorAxis * Math.Sin(fTheta))));
			}
		}

		#endregion // Creation Methods

		#region Render Methods

		/// <summary>
		/// Render points of the primitive.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		private void RenderPointPrimitive()
		{
			// Validate.
			if (VectorList.Count <= 0)
				return;

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
			float fAngle = 0f;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Stretch the pixel between the two vectors.
				_spriteBatch.Draw(Texture,
								  Position + VectorList[i],
								  null,
								  Color,
								  fAngle,
								  new Vector2(0.5f, 0.5f),
								  Thickness,
								  SpriteEffects.None,
								  Depth);
			}
		}

		/// <summary>
		/// Render points of the primitive.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		/// <param name="_fAngle">The counterclockwise rotation in radians. (0.0f is default).</param>
		/// <param name="_vPivot">Position in which to rotate around.</param>
		private void RenderPointPrimitive(float _fAngle, Vector2 _vPivot)
		{
			// Validate.
			if (VectorList.Count <= 0)
				return;

			// Rotate object based on pivot.
			Rotate(_fAngle, _vPivot);

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
			float fAngle = 0f;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Stretch the pixel between the two vectors.
				_spriteBatch.Draw(Texture,
								  Position + VectorList[i],
								  null,
								  Color,
								  fAngle,
								  new Vector2(0.5f, 0.5f),
								  Thickness,
								  SpriteEffects.None,
								  Depth);
			}
		}

		/// <summary>
		/// Render the lines of the primitive.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		private void RenderLinePrimitive()
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Stretch the pixel between the two vectors.
				_spriteBatch.Draw(Texture,
								  Position + vPosition1,
								  null,
								  Color,
								  fAngle,
								  new Vector2(0, 0.5f),
								  new Vector2(fDistance, Thickness),
								  SpriteEffects.None,
								  Depth);
			}
		}

		/// <summary>
		/// Render the lines of the primitive.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		/// <param name="_fAngle">The counterclockwise rotation in radians. (0.0f is default).</param>
		/// <param name="_vPivot">Position in which to rotate around.</param>
		private void RenderLinePrimitive(float _fAngle, Vector2 _vPivot)
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Rotate object based on pivot.
			Rotate(_fAngle, _vPivot);

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Stretch the pixel between the two vectors.
				_spriteBatch.Draw(Texture,
								  Position + vPosition1,
								  null,
								  Color,
								  fAngle,
								  new Vector2(0, 0.5f),
								  new Vector2(fDistance, Thickness),
								  SpriteEffects.None,
								  Depth);
			}
		}

		/// <summary>
		/// Render primitive by using a square algorithm.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		private void RenderSquarePrimitive()
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;
			int nCount = 0;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Calculate length.
				vLength = vPosition2 - vPosition1;
				vLength.Normalize();

				// Calculate count for roundness.
				nCount = (int)Math.Round(fDistance);

				// Run through and render the primitive.
				while (nCount-- > 0)
				{
					// Increment position.
					vPosition1 += vLength;

					// Stretch the pixel between the two vectors.
					_spriteBatch.Draw(Texture,
									  Position + vPosition1,
									  null,
									  Color,
									  0,
									  Vector2.Zero,
									  Thickness,
									  SpriteEffects.None,
									  Depth);
				}
			}
		}

		/// <summary>
		/// Render primitive by using a square algorithm.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		/// <param name="_fAngle">The counterclockwise rotation in radians. (0.0f is default).</param>
		/// <param name="_vPivot">Position in which to rotate around.</param>
		private void RenderSquarePrimitive(float _fAngle, Vector2 _vPivot)
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Rotate object based on pivot.
			Rotate(_fAngle, _vPivot);

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;
			int nCount = 0;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Calculate length.
				vLength = vPosition2 - vPosition1;
				vLength.Normalize();

				// Calculate count for roundness.
				nCount = (int)Math.Round(fDistance);

				// Run through and render the primitive.
				while (nCount-- > 0)
				{
					// Increment position.
					vPosition1 += vLength;

					// Stretch the pixel between the two vectors.
					_spriteBatch.Draw(Texture,
									  Position + vPosition1,
									  null,
									  Color,
									  0,
									  Vector2.Zero,
									  Thickness,
									  SpriteEffects.None,
									  Depth);
				}
			}
		}

		/// <summary>
		/// Render primitive by using a round algorithm.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		private void RenderRoundPrimitive()
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;
			int nCount = 0;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Calculate length.
				vLength = vPosition2 - vPosition1;
				vLength.Normalize();

				// Calculate count for roundness.
				nCount = (int)Math.Round(fDistance);

				// Run through and render the primitive.
				while (nCount-- > 0)
				{
					// Increment position.
					vPosition1 += vLength;

					// Stretch the pixel between the two vectors.
					_spriteBatch.Draw(Texture,
									  Position + vPosition1 + 0.5f * (vPosition2 - vPosition1),
									  null,
									  Color,
									  fAngle,
									  new Vector2(0.5f, 0.5f),
									  new Vector2(fDistance, Thickness),
									  SpriteEffects.None,
									  Depth);
				}
			}
		}

		/// <summary>
		/// Render primitive by using a round algorithm.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		/// <param name="_fAngle">The counterclockwise rotation in radians. (0.0f is default).</param>
		/// <param name="_vPivot">Position in which to rotate around.</param>
		private void RenderRoundPrimitive(float _fAngle, Vector2 _vPivot)
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Rotate object based on pivot.
			Rotate(_fAngle, _vPivot);

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero, vLength = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;
			int nCount = 0;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Calculate length.
				vLength = vPosition2 - vPosition1;
				vLength.Normalize();

				// Calculate count for roundness.
				nCount = (int)Math.Round(fDistance);

				// Run through and render the primitive.
				while (nCount-- > 0)
				{
					// Increment position.
					vPosition1 += vLength;

					// Stretch the pixel between the two vectors.
					_spriteBatch.Draw(Texture,
									  Position + vPosition1 + 0.5f * (vPosition2 - vPosition1),
									  null,
									  Color,
									  fAngle,
									  new Vector2(0.5f, 0.5f),
									  new Vector2(fDistance, Thickness),
									  SpriteEffects.None,
									  Depth);
				}
			}
		}

		/// <summary>
		/// Render primitive by using a point and line algorithm.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		private void RenderPolygonPrimitive()
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Stretch the pixel between the two vectors.
				_spriteBatch.Draw(Texture,
								  Position + vPosition1 + 0.5f * (vPosition2 - vPosition1),
								  null,
								  Color,
								  fAngle,
								  new Vector2(0.5f, 0.5f),
								  new Vector2(fDistance, Thickness),
								  SpriteEffects.None,
								  Depth);

				// Render the points of the polygon.
				_spriteBatch.Draw(Texture,
								  Position + vPosition1,
								  null,
								  Color,
								  fAngle,
								  new Vector2(0.5f, 0.5f),
								  Thickness,
								  SpriteEffects.None,
								  Depth);
			}
		}

		/// <summary>
		/// Render primitive by using a point and line algorithm.
		/// </summary>
		/// <param name="_spriteBatch">The sprite batch to use to render the primitive object.</param>
		/// <param name="_fAngle">The counterclockwise rotation in radians. (0.0f is default).</param>
		/// <param name="_vPivot">Position in which to rotate around.</param>
		private void RenderPolygonPrimitive(float _fAngle, Vector2 _vPivot)
		{
			// Validate.
			if (VectorList.Count < 2)
				return;

			// Rotate object based on pivot.
			Rotate(_fAngle, _vPivot);

			// Local variables.
			Vector2 vPosition1 = Vector2.Zero, vPosition2 = Vector2.Zero;
			float fDistance = 0f, fAngle = 0f;

			// Run through the list of vectors.
			for (int i = VectorList.Count - 1; i >= 1; --i)
			{
				// Store positions.
				vPosition1 = VectorList[i - 1];
				vPosition2 = VectorList[i];

				// Calculate the distance between the two vectors.
				fDistance = Vector2.Distance(vPosition1, vPosition2);

				// Calculate the angle between the two vectors.
				fAngle = (float)Math.Atan2((double)(vPosition2.Y - vPosition1.Y),
										   (double)(vPosition2.X - vPosition1.X));

				// Stretch the pixel between the two vectors.
				_spriteBatch.Draw(Texture,
								  Position + vPosition1 + 0.5f * (vPosition2 - vPosition1),
								  null,
								  Color,
								  fAngle,
								  new Vector2(0.5f, 0.5f),
								  new Vector2(fDistance, Thickness),
								  SpriteEffects.None,
								  Depth);

				// Render the points of the polygon.
				_spriteBatch.Draw(Texture,
								  Position + vPosition1,
								  null,
								  Color,
								  fAngle,
								  new Vector2(0.5f, 0.5f),
								  Thickness,
								  SpriteEffects.None,
								  Depth);
			}
		}

		#endregion // Render Methods

		#region Public Methods

		/// <summary>
		/// Rotate primitive object based on pivot.
		/// </summary>
		/// <param name="_fAngle">The counterclockwise rotation in radians. (0.0f is default).</param>
		/// <param name="_vPivot">Position in which to rotate around.</param>
		public void Rotate(float _fAngle, Vector2 _vPivot)
		{
			// Subtract pivot from all points.
			for (int i = VectorList.Count - 1; i >= 0; --i)
				VectorList[i] -= _vPivot;

			// Rotate about the origin.
			Matrix mat = Matrix.CreateRotationZ(_fAngle);
			for (int i = VectorList.Count - 1; i >= 0; --i)
				VectorList[i] = Vector2.Transform(VectorList[i], mat);

			// Add pivot to all points.
			for (int i = VectorList.Count - 1; i >= 0; --i)
				VectorList[i] += _vPivot;
		}

		/// <summary>
		/// draw a single point
		/// </summary>
		/// <param name="vPosition">where to draw the circle</param>
		/// <param name="fRadius">radius of the desired circle</param>
		/// <param name="myColor">color of the circle to draw</param>
		/// <param name="mySpriteBatch">graphic object used to draw</param>
		public void Point(Vector2 vPosition, Color myColor)
		{
			Clear();
			Position = vPosition;
			Color = myColor;
			CreateCircle(1.0f, NumCircleSegments);
			RenderLinePrimitive();
		}

		/// <summary>
		/// draw a quick circle
		/// </summary>
		/// <param name="vPosition">where to draw the circle</param>
		/// <param name="fRadius">radius of the desired circle</param>
		/// <param name="myColor">color of the circle to draw</param>
		/// <param name="mySpriteBatch">graphic object used to draw</param>
		public void Circle(Vector2 vPosition, float fRadius, Color myColor)
		{
			Clear();
			Position = vPosition;
			Color = myColor;
			CreateCircle(fRadius, NumCircleSegments);
			RenderLinePrimitive();
		}

		/// <summary>
		/// draw a quick line
		/// </summary>
		/// <param name="vStart">start point</param>
		/// <param name="vEnd">end point</param>
		/// <param name="myColor">color of the line to draw</param>
		/// <param name="mySpriteBatch">graphic object used to draw</param>
		public void Line(Vector2 vStart, Vector2 vEnd, Color myColor)
		{
			Clear();
			Color = myColor;
			CreateLine(vStart, vEnd);
			RenderLinePrimitive();
		}

		/// <summary>
		/// draw a quick box
		/// </summary>
		/// <param name="vUpperLeft">start point</param>
		/// <param name="vLowerRight">end point</param>
		/// <param name="myColor">color of the line to draw</param>
		/// <param name="mySpriteBatch">graphic object used to draw</param>
		public void AxisAlignedBox(Vector2 vUpperLeft, Vector2 vLowerRight, Color myColor)
		{
			Clear();
			Color = myColor;
			CreateSquare(vUpperLeft, vLowerRight);
			RenderLinePrimitive();
		}

		/// <summary>
		/// draw a quick box
		/// </summary>
		/// <param name="rect">the rectangle to draw</param>
		/// <param name="myColor">color of the line to draw</param>
		public void Rectangle(Rectangle rect, Color myColor)
		{
			Clear();
			AxisAlignedBox(new Vector2(rect.Left, rect.Top),
						   new Vector2(rect.Left + rect.Width, rect.Top + rect.Height),
						   myColor);
		}

		/// <summary>
		/// Draw a stupid rectanlge.
		/// This is the easiest way to draw a rectangle
		/// </summary>
		/// <param name="vUpperLeft">start point</param>
		/// <param name="vLowerRight">end point</param>
		/// <param name="fScale">the scale to draw the rectangle</param>
		/// <param name="myColor">the color to use to draw the rectangle</param>
		public void Rectangle(Vector2 vUpperLeft, Vector2 vLowerRight, float fRotation, float fScale, Color myColor)
		{
			Clear();
			Color = myColor;
			CreateSquare(vUpperLeft, vLowerRight);
			Rotate(fRotation, vUpperLeft); //this prolly dont work
			RenderLinePrimitive();
		}

		/// <summary>
		/// draw a pie shape
		/// </summary>
		/// <param name="Position">location to draw the pie</param>
		/// <param name="iRadius">the radius of the pie</param>
		/// <param name="fStartAngle">the angle to start the pie</param>
		/// <param name="fSweepAngle">the sweep angle of the pie</param>
		/// <param name="rColor">color dat pie</param>
		public void Pie(Vector2 position, int iRadius, float fStartAngle, float fSweepAngle, Color rColor)
		{
			//TODO: draw a pie shape.
		}

		#endregion // Public Methods
	}
}