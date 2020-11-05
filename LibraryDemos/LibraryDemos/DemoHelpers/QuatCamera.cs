using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LibraryDemos.DemoHelpers
{
    public class QuatCamera
    {
        public Vector3 Position;            //Position of the camera
        private Quaternion m_quatRotation;  //Camera rotation when it uses a Quaternion for rotations (m_bTiltMode = false)
        private Matrix m_mtxWorld;          //The world matrix
        private Matrix m_mtxView;           //The view matrix
        private Matrix m_mtxProjection;     //The projection matrix
        private Viewport m_Viewport;        //Viewport representing the camera


        #region Move Camera

        /// <summary>
        /// Moves the camera along the Y axis
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void MoveY(float fDistance)
        {
            Move(new Vector3(0, 1, 0), fDistance);
        }

        /// <summary>
        /// Moves the camera along the Z axis 
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void MoveZ(float fDistance)
        {
            Move(new Vector3(0, 0, 1), fDistance);
        }

        /// <summary>
        /// Moves the camera along the X axis 
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void MoveX(float fDistance)
        {
            Move(new Vector3(1, 0, 0), fDistance);
        }

        /// <summary>
        /// Pans the camera along the X, Y, and Z directions
        /// </summary>
        /// <param name="vecDistance">The amount of X, Y, and Z distance to pan the camera. Units are assumed
        /// to be world units.</param>
        public void Move(Vector3 axis, float fDistance)
        {
            Vector3 vecDistance;
            //Because we use an inverted matrix during updates, we flip the sign on the distance vector.
            vecDistance = -axis * fDistance;
            Position += Vector3.Transform(vecDistance, Matrix.CreateFromQuaternion(m_quatRotation));
        }
        #endregion

        #region Rotate Camera

        /// <summary>
        /// Rotates the camera along the Z axis.
        /// </summary>
        /// <param name="fDegrees">The amount of degrees to rotate the camera by.</param>
        public void RotateZ(float fDegrees)
        {
            Rotate(new Vector3(0, 0, 1), fDegrees);
        }

        /// <summary>
        /// Rotates the camera along the X axis.
        /// </summary>
        /// <param name="fDegrees">The amount of degrees to rotate the camera by.</param>
        public void RotateX(float fDegrees)
        {
            Rotate(new Vector3(1, 0, 0), fDegrees);
        }

        /// <summary>
        /// Rotates the camera along the Y axis.
        /// </summary>
        /// <param name="fDegrees">The amount of degrees to rotate the camera by.</param>
        public void RotateY(float fDegrees)
        {
            Rotate(new Vector3(0, 1, 0), fDegrees);
        }

        /// <summary>
        /// Rotates the camera along an arbitrary axis, relative to the position of the camera, not the world.
        /// </summary>
        /// <param name="axis">A Vector3 containing the axis along which to rotate the camera.</param>
        /// <param name="fDegrees">The number of degrees to rotate the camera by.</param>
        public void Rotate(Vector3 axis, float fDegrees)
        {
            axis = Vector3.Transform(axis, Matrix.CreateFromQuaternion(m_quatRotation));

            //Because we use an inverted matrix, we flip the sign on the degrees or the camera will go in
            //the opposite direction that we expect.
            m_quatRotation = Quaternion.Normalize(Quaternion.CreateFromAxisAngle(axis, MathHelper.ToRadians(-fDegrees)) *
                m_quatRotation);
        }
        #endregion

        #region Revolve Camera on Target
        /// <summary>
        /// Revolves the camera along the Y axis using a target as the reference point
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void RevolveY(Vector3 vecTarget, float fDegrees)
        {
            Revolve(vecTarget, new Vector3(0, 1, 0), fDegrees);
        }

        /// <summary>
        /// Revolves the camera along the Z axis using a target as the reference point
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void RevolveZ(Vector3 vecTarget, float fDegrees)
        {
            Revolve(vecTarget, new Vector3(0, 0, 1), fDegrees);
        }

        /// <summary>
        /// Revolves the camera along the X axis using a target as the reference point
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void RevolveX(Vector3 vecTarget, float fDegrees)
        {
            Revolve(vecTarget, new Vector3(1, 0, 0), fDegrees);
        }

        /// <summary>
        /// Revolves the camera along an axis using a target as the reference point
        /// </summary>
        /// <param name="vecTarget">The target to revolve around</param>
        /// <param name="vecAxis">The axis of rotation</param>
        /// <param name="fDegrees">The degree of rotation</param>
        public void Revolve(Vector3 vecTarget, Vector3 vecAxis, float fDegrees)
        {
            Quaternion quatRotation;


            //Rotate camera to proper angle
            Rotate(vecAxis, fDegrees);

            //Transform axis of rotation in terms of the Quaternion
            vecAxis = Vector3.Transform(vecAxis, Matrix.CreateFromQuaternion(m_quatRotation));

            //Rotate the Quaternion about the axis
            quatRotation = Quaternion.CreateFromAxisAngle(vecAxis, MathHelper.ToRadians(-fDegrees));

            //Update our position
            Position = Vector3.Transform(vecTarget - Position, Matrix.CreateFromQuaternion(quatRotation));
        }
        #endregion

        #region ChaseCamera

        public void ChaseTarget(Vector3 vecTarget, Vector3 vecDistance)
        {

        }
        #endregion

        #region Functions
        /// <summary>
        /// Resets the camera to the World-coordinate origin.
        /// </summary>
        public void ReturnToOrigin()
        {
            Position = new Vector3(0, 0, 1);
        }

        /// <summary>
        /// Returns the camera to the upright position. Required in preparation for landing. ;)
        /// Camera will have no rotation in the X, Y, or Z directions.
        /// </summary>
        public void ZeroRotations()
        {
            m_quatRotation = new Quaternion(0, 0, 0, 1);
        }
        #endregion

        #region Constructor & Update
        /// <summary>
        /// Constructor of a camera object, placed at the World-coordinate origin and with no rotation.
        /// </summary>
        /// <param name="newViewport">A Viewport object representing the view of the camera.</param>
        public QuatCamera(Viewport newViewport)
        {
            this.Viewport = newViewport;
            this.ReturnToOrigin();
            this.ZeroRotations();

            Update();
        }


        /// <summary>
        /// Updates the rotation and positioning of the camera. Called just before drawing a scene.
        /// </summary>
        public void Update()
        {
            m_mtxWorld = Matrix.Identity;

            m_mtxView = Matrix.Invert(
                Matrix.CreateFromQuaternion(m_quatRotation) *
                Matrix.CreateTranslation(Position));

            Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, m_Viewport.AspectRatio, m_Viewport.MinDepth, m_Viewport.MaxDepth, out m_mtxProjection);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets/sets the Viewport of the camera. When setting the Viewport, the minimum depth becomes 1,
        /// and the maximum depth becomes 1000.
        /// </summary>
        public Viewport Viewport
        {
            get
            {
                return m_Viewport;
            }
            set
            {
                m_Viewport = value;

                m_Viewport.MinDepth = 0.5f;
                m_Viewport.MaxDepth = 8000f;
            }
        }

        /// <summary>
        /// Gets/sets the projection matrix of the camera. This is the viewing frustum of the camera.
        /// </summary>
        public Matrix Projection
        {
            get { return m_mtxProjection; }
            set { m_mtxProjection = value; }
        }

        /// <summary>
        /// Gets/sets the view matrix of the camera. 
        /// </summary>
        public Matrix View
        {
            get { return m_mtxView; }
            set { m_mtxView = value; }
        }

        /// <summary>
        /// Gets/sets the world matrix of the camera.
        /// </summary>
        public Matrix World
        {
            get { return m_mtxWorld; }
            set { m_mtxWorld = value; }
        }

        public Quaternion Rotation
        {
            get
            {
                return m_quatRotation;
            }
        }

        #endregion
    }
}
