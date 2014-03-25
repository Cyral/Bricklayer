using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BricklayerClient.World
{
    /// <summary>
    /// A camera object which can focus around a point and be used for drawing at a certain position, zoom, and rotation
    /// </summary>
    public class Camera
    {
        //Properties
        public Vector2 Position { get { return position; } set {
            position.X = (float)MathHelper.Clamp(value.X, MinBounds.X, MaxBounds.X - size.X);
            position.Y = (float)MathHelper.Clamp(value.Y, MinBounds.Y, MaxBounds.Y - size.Y);
        } }
        public Vector2 Origin {
            get { return new Vector2(size.X / 2.0f, size.Y / 2.0f); }
            set { Position = new Vector2(value.X - size.X / 2.0f, value.Y - size.Y / 2.0f); }
        }   

        public float Zoom { get; set; }
        public float Rotation { get; set; }

        //Bounds
        public float Top { get { return Position.Y; } }
        public float Left { get { return Position.X; } }
        public float Bottom { get { return Position.Y + size.Y; } }
        public float Right { get { return Position.X + size.X; } }

        //Maximum positions
        public Vector2 MaxBounds { get; set; }
        public Vector2 MinBounds { get; set; }

        //Fields
        private Vector2 size;
        private Vector2 position;

        /// <summary>
        /// Creates a new camera with the specified size
        /// </summary>
        /// <param name="size">Ususally close to the viewport size, defines the size of the camera</param>
        public Camera(Vector2 size)
        {
            this.size = size;
            Zoom = 1.0f;
        }
        /// <summary>
        /// Get a Matrix that can be used with a spritebatch for drawing objects in the camera
        /// </summary>
        public Matrix GetViewMatrix(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                   Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                   Matrix.CreateRotationZ(Rotation) *
                   Matrix.CreateScale(Zoom, Zoom, 1) *
                   Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }
        /// <summary>
        /// Will move the position a certain amount
        /// </summary>
        /// <param name="displacement">Amount to move</param>
        /// <param name="respectRotation">Account for the current rotation</param>
        public void Move(Vector2 displacement, bool respectRotation = false)
        {
            if (respectRotation)
            {
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));
            }

            Position += displacement;
        }
        /// <summary>
        /// Sets the position to look at a certain point, automatically factoring for the center of the camera
        /// </summary>
        public void LookAt(Vector2 position)
        {
            Position = position - Origin;
        }
        /// <summary>
        /// Transforms world coordinates to screen coordinates
        /// </summary>
        public Vector2 WorldToScreen(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, GetViewMatrix(Vector2.One));
        }
        /// <summary>
        /// Transforms screen coordinates to world coordinates
        /// </summary>
        public Vector2 ScreenToWorld(Vector2 screenPosition)
        {
            return Vector2.Transform(screenPosition, Matrix.Invert(GetViewMatrix(Vector2.One)));
        }
    }
}
