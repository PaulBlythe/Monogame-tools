/*
 * C# / XNA  port of Bullet (c) 2011 Mark Neale <xexuxjy@hotmail.com>
 *
 * Bullet Continuous Collision Detection and Physics Library
 * Copyright (c) 2003-2008 Erwin Coumans  http://www.bulletphysics.com/
 *
 * This software is provided 'as-is', without any express or implied warranty.
 * In no event will the authors be held liable for any damages arising from
 * the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose, 
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using BulletMonogame;
using BulletMonogame.BulletCollision;
using BulletMonogame.BulletDynamics;
using BulletMonogame.LinearMath;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace BulletMonogameDemo.Demos
{
    public abstract class DemoApplication 
    {
        public int m_numIterations = 0;
        public int m_maxIterations = 0;
        public IProfileManager m_profileManager;
        public IProfileIterator m_profileIterator;
        public IDebugDraw m_debugDraw;
        public SpriteBatch m_spriteBatch;
        public IList<CollisionShape> m_collisionShapes = new List<CollisionShape>();
        public IBroadphaseInterface m_broadphase;
        public CollisionDispatcher m_dispatcher;
        public IConstraintSolver m_constraintSolver;
        public DefaultCollisionConfiguration m_collisionConfiguration;
        public DynamicsWorld m_dynamicsWorld;
        public IndexedVector3 m_defaultGravity = new IndexedVector3(0, -10, 0);

        public const float STEPSIZEROTATE = MathUtil.SIMD_PI / 3f; // 60 deg a second
        public const float STEPSIZETRANSLATE = 20f; 

        public const float mousePickClamping = 30f;

        public static int gPickingConstraintId = 0;
        public static IndexedVector3 gOldPickingPos;
		public static IndexedVector3 gHitPos = new IndexedVector3(-1f);

        public static float gOldPickingDist = 0f;
        public static RigidBody pickedBody = null;//for deactivation state

        public static int gNumDeepPenetrationChecks;

        public static int gNumSplitImpulseRecoveries;
        public static int gNumGjkChecks;
        public static int gNumAlignedAllocs;
        public static int gNumAlignedFree;
        public static int gTotalBytesAlignedAllocs;

        public static int gNumClampedCcdMotions;

        // public for now to test camera acccess in xna shape draw.
        public IndexedMatrix m_lookAt = IndexedMatrix.Identity;
        public IndexedMatrix m_perspective = IndexedMatrix.Identity;

        ///constraint for mouse picking
        public TypedConstraint m_pickConstraint;

        public CollisionShape m_shootBoxShape;

        public float m_cameraDistance;

        public float m_pitch;
        public float m_yaw;

        public IndexedVector3 m_cameraPosition;
        public IndexedVector3 m_cameraTargetPosition;//look at
		public Boolean m_ortho = false;


        public float m_scaleBottom;
        public float m_scaleFactor;
        public IndexedVector3 m_cameraUp;
        public int m_forwardAxis;


        public float m_ShootBoxInitialSpeed;

        public bool m_stepping;
        public bool m_singleStep;
        public bool m_idle;
        public Keys m_lastKey;
        public KeyboardState m_lastKeyboardState;
        public MouseState m_lastMouseState;
        public GamePadState m_lastGamePadState;

        public XNA_ShapeDrawer m_shapeDrawer;
        public bool m_enableshadows;
        public IndexedVector3 m_lightDirection;
        public IndexedVector3 m_lightPosition;
        public IndexedMatrix m_lightView;
        public IndexedMatrix m_lightProjection;

        public float m_lightPower = 0.5f;
        public Vector4 m_ambientLight = new Vector4(0.1f,0.1f,0.1f,1f);
        public Vector4 m_diffuseLight = Color.LightGray.ToVector4();

        public float m_nearClip;
        public float m_farClip;
        public float m_aspect;


        public bool use6Dof = false;


        //----------------------------------------------------------------------------------------------

        public void UpdateLights()
        {
            
            //IndexedVector3 target = m_lightPosition + m_lightDirection * 10;
            IndexedVector3 target = IndexedVector3.Zero;
            float aspect = Game1.Instance.GraphicsDevice.Viewport.AspectRatio;
            float fov = MathHelper.ToRadians(40.0f);

            m_lightView = IndexedMatrix.CreateLookAt(m_lightPosition, target, new IndexedVector3(0,1,0));
            m_lightProjection = IndexedMatrix.CreatePerspectiveFieldOfView(fov, aspect, 1f, 500f);
        }

        //----------------------------------------------------------------------------------------------

        public virtual void RenderSceneAll(GameTime gameTime)
        {
            if (m_enableshadows)
            {
                //renderScenePass(1,gameTime);
            }
            m_dynamicsWorld.DebugDrawWorld();
            RenderScenePass(0, gameTime);
            IndexedVector3 location = new IndexedVector3(10, 10, 0);
            IndexedVector3 colour = new IndexedVector3(1,1,1);
            m_shapeDrawer.DrawText(String.Format("Memory [{0}]", System.GC.GetTotalMemory(false)), ref location, ref colour);
            int	xOffset = 10;
            int yStart = 20;
		    int yIncr = 15;

            ShowProfileInfo(xOffset, yStart, yIncr);

            
            m_shapeDrawer.RenderOthers(gameTime, m_lookAt, m_perspective);
        }


        public virtual void RenderScenePass(int pass, GameTime gameTime)
        {
	        IndexedMatrix m = IndexedMatrix.Identity;
	        IndexedBasisMatrix rot = IndexedBasisMatrix.Identity;
	        int numObjects = m_dynamicsWorld.GetNumCollisionObjects();
	        IndexedVector3 wireColor = new IndexedVector3(1,0,0);

            for(int i=0;i<numObjects;i++)
	        {
		        CollisionObject colObj=m_dynamicsWorld.GetCollisionObjectArray()[i];
		        RigidBody body = RigidBody.Upcast(colObj);
		        if(body != null && body.GetMotionState() != null)
		        {
			        DefaultMotionState myMotionState = (DefaultMotionState)body.GetMotionState();
                    //myMotionState.m_graphicsWorldTrans.getOpenGLMatrix(m);
                    m = myMotionState.m_graphicsWorldTrans;
			        rot=myMotionState.m_graphicsWorldTrans._basis;
		        }
		        else
		        {
                    //colObj.getWorldTransform().getOpenGLMatrix(m);
                    m = colObj.GetWorldTransform();
			        rot=colObj.GetWorldTransform()._basis;
		        }
		        wireColor = new IndexedVector3(1.0f,1.0f,0.5f); //wants deactivation
		        if((i&1) != 0) wireColor= new IndexedVector3(0f,0f,1f);
		        ///color differently for active, sleeping, wantsdeactivation states
		        if (colObj.GetActivationState() == ActivationState.ACTIVE_TAG) //active
		        {
			        if ((i & 1) != 0)
			        {
				        wireColor += new IndexedVector3(1f,0f,0f);
			        }
			        else
			        {			
				        wireColor += new IndexedVector3(.5f,0f,0f);
			        }
		        }
		        if(colObj.GetActivationState()==ActivationState.ISLAND_SLEEPING) //ISLAND_SLEEPING
		        {
                    if ((i & 1) != 0)
                    {
				        wireColor += new IndexedVector3 (0f,1f, 0f);
			        }
			        else
			        {
				        wireColor += new IndexedVector3(0f,05f,0f);
			        }
		        }

                IndexedVector3 min,max;
		        m_dynamicsWorld.GetBroadphase().GetBroadphaseAabb(out min,out max);

                min -= MathUtil.MAX_VECTOR;
                max += MathUtil.MAX_VECTOR;
        //		printf("aabbMin=(%f,%f,%f)\n",aabbMin.getX(),aabbMin.getY(),aabbMin.getZ());
        //		printf("aabbMax=(%f,%f,%f)\n",aabbMax.getX(),aabbMax.getY(),aabbMax.getZ());
        //		m_dynamicsWorld.getDebugDrawer().drawAabb(aabbMin,aabbMax,btVector3(1,1,1));

                switch(pass)
		        {
                    case 0:
                        {
                            m_shapeDrawer.DrawXNA(ref m, colObj.GetCollisionShape(), ref wireColor, m_debugDraw.GetDebugMode(), ref min, ref max, ref m_lookAt, ref m_perspective);
                            break;
                        }
                    case 1:
                        {
                            IndexedVector3 shadow = rot * m_lightDirection;
                            m_shapeDrawer.DrawShadow(ref m, ref shadow, colObj.GetCollisionShape(), ref min, ref max);
                            break;
                        }
		            case	2:
                        {
                            IndexedVector3 adjustedWireColor = wireColor * 0.3f;
                            m_shapeDrawer.DrawXNA(ref m,colObj.GetCollisionShape(),ref adjustedWireColor,0,ref min,ref max,ref m_lookAt,ref m_perspective);
                            break;
                        }
		        }
	        }

            switch (pass)
            {
                case 0:
                    {
						//m_shapeDrawer.RenderShadow(gameTime, ref m_lookAt, ref m_perspective);
                        m_shapeDrawer.RenderStandard(gameTime, ref m_lookAt, ref m_perspective);
                        m_shapeDrawer.RenderDebugLines(gameTime, ref m_lookAt, ref m_perspective);
                        break;
                    }
                case 1:
                    {
						//m_shapeDrawer.RenderShadow(gameTime, ref m_lookAt, ref m_perspective);
                        break;
                    }
                case 2:
                    {
                        m_shapeDrawer.RenderStandard(gameTime, ref m_lookAt, ref m_perspective);
                        break;
                    }
            }
        }

        //----------------------------------------------------------------------------------------------

        public DemoApplication()
        {
            m_dynamicsWorld = null;
            m_pickConstraint = null;
            m_shootBoxShape = null;
            m_cameraDistance = 30f;
            m_pitch =(20f/360f)*MathUtil.SIMD_2_PI;
            m_yaw = 0f;
            m_cameraPosition = IndexedVector3.Zero;
            m_cameraTargetPosition = IndexedVector3.Zero;
            m_scaleBottom = 0.5f;
            m_scaleFactor = 2f;
            m_cameraUp = new IndexedVector3(0, 1, 0);
            m_forwardAxis = 2;
            m_ShootBoxInitialSpeed = 40f;
            m_stepping = true;
            m_singleStep = false;
            m_idle = false;
            m_enableshadows = true;
            m_lightPosition = new IndexedVector3(5, 5, 5);
            m_lightDirection = new IndexedVector3(.5f, -.5f, .5f);
            m_lightDirection.Normalize();

            m_nearClip = 1f;
            m_farClip = 1000f;

            m_aspect = Game1.Instance.GraphicsDevice.Viewport.AspectRatio;
            m_perspective = IndexedMatrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(40.0f), m_aspect, m_nearClip, m_farClip);
            m_shapeDrawer = Game1.Instance.shape_drawer;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void Cleanup()
        {

        }

        //----------------------------------------------------------------------------------------------

        public IndexedVector3 GetLightDirection()
        {
            return m_lightDirection;
        }

        //----------------------------------------------------------------------------------------------

        public IndexedVector3 GetLightPosition()
        {
            return m_lightPosition;
        }

        //----------------------------------------------------------------------------------------------

        public IndexedMatrix GetLightViewMatrix()
        {
            return m_lightView;
        }

        //----------------------------------------------------------------------------------------------

        public IndexedMatrix GetLightProjectionMatrix()
        {
            return m_lightProjection;
        }

        //----------------------------------------------------------------------------------------------

        public float GetLightPower()
        {
            return m_lightPower;
        }
        
        //----------------------------------------------------------------------------------------------

        public Vector4 GetAmbientLight()
        {
            return m_ambientLight;
        }
        
        //----------------------------------------------------------------------------------------------

        public Vector4 GetDiffuseLight()
        {
            return m_diffuseLight;
        }

        //----------------------------------------------------------------------------------------------
        public DynamicsWorld GetDynamicsWorld()
        {
            return m_dynamicsWorld;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void SetDrawClusters(bool drawClusters)
        {

        }

        //----------------------------------------------------------------------------------------------

        public void OverrideXNAShapeDrawer(XNA_ShapeDrawer shapeDrawer)
        {
        }

        //----------------------------------------------------------------------------------------------

        public void SetOrthographicProjection()
        {
        }

        //----------------------------------------------------------------------------------------------

        public void ResetPerspectiveProjection()
        {
        }

        //----------------------------------------------------------------------------------------------

        public bool SetTexturing(bool enable)
        {
            m_shapeDrawer.EnableTexture(enable);
            return m_shapeDrawer.HasTextureEnabled();
        }

        //----------------------------------------------------------------------------------------------

        public bool SetShadows(bool enable)
        {
            bool p = m_enableshadows;
            m_enableshadows = enable;
            return (p);
        }

        //----------------------------------------------------------------------------------------------

        public bool GetTexturing()
        {
            return m_shapeDrawer.HasTextureEnabled();
        }

        //----------------------------------------------------------------------------------------------

        public bool GetShadows()
        {
            return m_enableshadows;
        }

        //----------------------------------------------------------------------------------------------

        public void SetAzi(float azi)
        {
            m_yaw = azi;
        }

        //----------------------------------------------------------------------------------------------

        public void SetCameraUp(IndexedVector3 camUp)
        {
            m_cameraUp = camUp;
        }

        //----------------------------------------------------------------------------------------------
        
        public void SetCameraForwardAxis(int axis)
        {
            m_forwardAxis = axis;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void InitializeDemo()
        {
        }

        //----------------------------------------------------------------------------------------------

        public virtual void ShutdownDemo()
        {
            //delete collision shapes
            for (int j = 0; j < m_collisionShapes.Count; j++)
            {
                m_collisionShapes[j].Cleanup();
            }

            //delete dynamics world
            m_dynamicsWorld.Cleanup();

            //delete solver
            m_constraintSolver.Cleanup();

            //delete broadphase
            m_broadphase.Cleanup();

            //delete dispatcher
            m_dispatcher.Cleanup();

            m_collisionConfiguration.Cleanup();

        }

        //----------------------------------------------------------------------------------------------

        public void ToggleIdle()
        {
            m_idle = !m_idle;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void UpdateCamera()
        {
            float rele = m_pitch;
            float razi = m_yaw;

            IndexedQuaternion rot = new IndexedQuaternion(m_cameraUp, razi);
            
            IndexedVector3 eyePos = new IndexedVector3();
            eyePos[m_forwardAxis] = m_cameraDistance;

            IndexedVector3 forward = eyePos;
            if (forward.LengthSquared() < MathUtil.SIMD_EPSILON)
            {
                forward = new IndexedVector3(0,0,-1);
            }
            IndexedVector3 right = IndexedVector3.Cross(m_cameraUp, IndexedVector3.Normalize(forward));
            IndexedQuaternion roll = new IndexedQuaternion(right, -rele);
            rot.Normalize();
            roll.Normalize();

            IndexedMatrix m1 = IndexedMatrix.CreateFromQuaternion(rot);
            IndexedMatrix m2 = IndexedMatrix.CreateFromQuaternion(roll);
            IndexedMatrix m3 = m1 * m2;

            eyePos = m3 * eyePos;

            m_cameraPosition = eyePos;

            m_cameraPosition += m_cameraTargetPosition;

            m_lookAt = IndexedMatrix.CreateLookAt(m_cameraPosition, m_cameraTargetPosition, m_cameraUp);
            Matrix t = Matrix.CreateLookAt(m_cameraPosition.ToVector3(), m_cameraTargetPosition.ToVector3(), m_cameraUp.ToVector3());
            Matrix t2 = m_lookAt.ToMatrix();

           
        }

        //----------------------------------------------------------------------------------------------

        public IndexedVector3 GetCameraPosition()
        {
            return m_cameraPosition;
        }
        
        //----------------------------------------------------------------------------------------------

        public IndexedVector3 GetCameraTargetPosition()
        {
            return m_cameraTargetPosition;
        }

        //----------------------------------------------------------------------------------------------

        public float GetDeltaTimeMicroseconds()
        {
            return 16666f;
        }

        ///glut callbacks
        //----------------------------------------------------------------------------------------------

        public float GetCameraDistance()
        {
            return m_cameraDistance;
        }

        //----------------------------------------------------------------------------------------------

        public void SetCameraDistance(float dist)
        {
            m_cameraDistance = dist;
        }
        //----------------------------------------------------------------------------------------------

        public void MoveAndDisplay(GameTime gameTime)
        {
            if (!m_idle)
            {
                ClientMoveAndDisplay(gameTime);
            }

        }

        //----------------------------------------------------------------------------------------------

        public virtual void ClientMoveAndDisplay(GameTime gameTime)
        {
            //simple dynamics world doesn't handle fixed-time-stepping

            float ms = gameTime.ElapsedGameTime.Milliseconds/1000.0f;
            ///step the simulation
            if (m_dynamicsWorld != null)
            {
                m_dynamicsWorld.StepSimulation(ms, 1);
                m_numIterations++;
                if (m_maxIterations > 0 && m_numIterations > m_maxIterations)
                {
                    Cleanup();
                }
            }
        }

        //----------------------------------------------------------------------------------------------

        public virtual void ClientResetScene()
        {			
	        gNumDeepPenetrationChecks = 0;
	        gNumGjkChecks = 0;

	        gNumClampedCcdMotions = 0;
	        int numObjects = 0;

	        if (m_dynamicsWorld != null)
	        {
                // Prefer a better place for this...
                m_dynamicsWorld.SetDebugDrawer(m_debugDraw);

		        numObjects = m_dynamicsWorld.GetNumCollisionObjects();
	        }

            IList<CollisionObject> copyArray = m_dynamicsWorld.GetCollisionObjectArray();

	        for (int i=0;i<numObjects;i++)
	        {
		        CollisionObject colObj = copyArray[i];
		        RigidBody body = RigidBody.Upcast(colObj);
		        if (body != null)
		        {
			        if (body.GetMotionState() != null)
			        {
				        DefaultMotionState myMotionState = (DefaultMotionState)body.GetMotionState();
				        myMotionState.m_graphicsWorldTrans = myMotionState.m_startWorldTrans;
				        body.SetCenterOfMassTransform(ref myMotionState.m_graphicsWorldTrans );
				        colObj.SetInterpolationWorldTransform(ref myMotionState.m_startWorldTrans );
                        if (colObj.GetActivationState() != ActivationState.DISABLE_DEACTIVATION)
                        {
                            colObj.ForceActivationState(ActivationState.ACTIVE_TAG);
                            colObj.Activate();
                            colObj.SetDeactivationTime(0);
                        }
			        }
			        //removed cached contact points (this is not necessary if all objects have been removed from the dynamics world)
                    m_dynamicsWorld.GetBroadphase().GetOverlappingPairCache().CleanProxyFromPairs(colObj.GetBroadphaseHandle(),GetDynamicsWorld().GetDispatcher());

			        if (!body.IsStaticObject())
			        {
                        IndexedVector3 zero = IndexedVector3.Zero;
				        body.SetLinearVelocity(ref zero);
				        body.SetAngularVelocity(ref zero);
			        }
		        }
	        }

	        ///reset some internal cached data in the broadphase
	        m_dynamicsWorld.GetBroadphase().ResetPool(GetDynamicsWorld().GetDispatcher());
	        m_dynamicsWorld.GetConstraintSolver().Reset();
        }

        //----------------------------------------------------------------------------------------------

        ///Demo functions
        public virtual void SetShootBoxShape()
        {
            if (m_shootBoxShape == null)
            {
                //#define TEST_UNIFORM_SCALING_SHAPE 1
#if TEST_UNIFORM_SCALING_SHAPE
			    ConvexShape childShape = new BoxShape(new IndexedVector3(1f,1f,1f));
			    m_shootBoxShape = new UniformScalingShape(childShape,0.5f);
#else
                float dims = 0.25f;
                //m_shootBoxShape = new SphereShape(dims);//BoxShape(btVector3(1.f,1.f,1.f));
                m_shootBoxShape = new BoxShape(new IndexedVector3(dims));

#endif//
            }
        }

        //----------------------------------------------------------------------------------------------

        public void ShootBox(IndexedVector3 destination)
        {
            if (m_dynamicsWorld != null)
            {
                float mass = 1f;
                IndexedMatrix startTransform = IndexedMatrix.Identity;
                IndexedVector3 camPos = new IndexedVector3(GetCameraPosition());
                startTransform._origin = camPos;

                SetShootBoxShape();
                RigidBody body = LocalCreateRigidBody(mass, ref startTransform, m_shootBoxShape);
                body.SetLinearFactor(IndexedVector3.One);
                IndexedVector3 linVel = destination - camPos;
                linVel.Normalize();
                linVel *= m_ShootBoxInitialSpeed;

                IndexedMatrix newMatrix = IndexedMatrix.CreateFromQuaternion(IndexedQuaternion.Identity);
                newMatrix._origin = camPos;
                body.SetWorldTransform(ref newMatrix);
                body.SetLinearVelocity(ref linVel);
                IndexedVector3 temp = IndexedVector3.Zero;
                body.SetAngularVelocity(ref temp);

                body.SetCcdMotionThreshold(0.5f);
                body.SetCcdSweptSphereRadius(0.9f);

            }
        }

        //----------------------------------------------------------------------------------------------

        public IndexedVector3 GetRayTo(int x, int y)
        {
            float fov = MathHelper.ToRadians(40.0f);

            IndexedVector3 rayFrom = new IndexedVector3(GetCameraPosition());
            IndexedVector3 rayForward = new IndexedVector3((GetCameraTargetPosition() - GetCameraPosition()));
            rayForward.Normalize();
            float farPlane = 10000f;
            rayForward *= farPlane;

            IndexedVector3 vertical = new IndexedVector3(m_cameraUp);

            IndexedVector3 hor = IndexedVector3.Cross(rayForward, vertical);
            hor.Normalize();
            vertical = IndexedVector3.Cross(hor, rayForward);
            vertical.Normalize();

            float tanfov = (float)Math.Tan(0.5f * fov);

            hor *= 2f * farPlane * tanfov;
            vertical *= 2f * farPlane * tanfov;

            float aspect = Game1.Instance.GraphicsDevice.Viewport.AspectRatio;

            IndexedVector3 rayToCenter = rayFrom + rayForward;
            IndexedVector3 dHor = hor * 1f / (float)Game1.Instance.GraphicsDevice.Viewport.Width;
            IndexedVector3 dVert = vertical * 1f / (float)Game1.Instance.GraphicsDevice.Viewport.Height;

            IndexedVector3 rayTo = rayToCenter - 0.5f * hor + 0.5f * vertical;
            rayTo += x * dHor;
            rayTo -= y * dVert;
            return rayTo;
        }

        //----------------------------------------------------------------------------------------------
        public RigidBody LocalCreateRigidBody(float mass, IndexedMatrix startTransform, CollisionShape shape)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape);
        }

        public RigidBody LocalCreateRigidBody(float mass, IndexedMatrix startTransform, CollisionShape shape,bool addToWorld)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape,addToWorld);
        }

        public RigidBody LocalCreateRigidBody(float mass, ref IndexedMatrix startTransform, CollisionShape shape)
        {
            return LocalCreateRigidBody(mass, ref startTransform, shape, true);
        }

        public RigidBody LocalCreateRigidBody(float mass, ref IndexedMatrix startTransform, CollisionShape shape,bool addToWorld)
        {
			
            Debug.Assert((shape == null || shape.GetShapeType() != BroadphaseNativeTypes.INVALID_SHAPE_PROXYTYPE));

            //rigidbody is dynamic if and only if mass is non zero, otherwise static
            bool isDynamic = !MathUtil.CompareFloat(mass, 0f);

            IndexedVector3 localInertia = IndexedVector3.Zero;
            if (isDynamic)
            {
                shape.CalculateLocalInertia(mass, out localInertia);
            }
            //using motionstate is recommended, it provides interpolation capabilities, and only synchronizes 'active' objects

            //#define USE_MOTIONSTATE 1
            //#ifdef USE_MOTIONSTATE
            DefaultMotionState myMotionState = new DefaultMotionState(startTransform, IndexedMatrix.Identity);

            RigidBodyConstructionInfo cInfo = new RigidBodyConstructionInfo(mass, myMotionState, shape, localInertia);

            RigidBody body = new RigidBody(cInfo);

            if (BulletGlobals.g_streamWriter != null && true)
            {
                BulletGlobals.g_streamWriter.WriteLine("localCreateRigidBody [{0}] startTransform",body.m_debugBodyId);
                MathUtil.PrintMatrix(BulletGlobals.g_streamWriter, startTransform);
                BulletGlobals.g_streamWriter.WriteLine("");
            }

            //#else
            //    btRigidBody* body = new btRigidBody(mass,0,shape,localInertia);	
            //    body.setWorldTransform(startTransform);
            //#endif//

            if (addToWorld)
            {
                m_dynamicsWorld.AddRigidBody(body);
            }

            return body;
        }

        //----------------------------------------------------------------------------------------------


        //----------------------------------------------------------------------------------------------

        public virtual void KeyboardCallback(Keys key,int x,int y,GameTime gameTime,bool released,ref KeyboardState newState,ref KeyboardState oldState)
        {
            m_lastKey = 0;
            int keyInt = (int)key;
#if !BT_NO_PROFILE
            if (keyInt >= 0x31 && keyInt <= 0x39)
            {
                int child = keyInt - 0x31;
                if (m_profileIterator != null)
                {
                    m_profileIterator.Enter_Child(child);

                }
            }
            if (keyInt == 0x30)
            {
                if (m_profileIterator != null)
                {
                    m_profileIterator.Enter_Parent();
                }
            }
#endif //BT_NO_PROFILE

            DebugDrawModes debugMode = m_debugDraw.GetDebugMode();

            switch (key)
            {
                case Keys.Q:
                    Cleanup();
                    break;

                case Keys.L: StepLeft((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.R: StepRight((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.F: StepFront((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.B: StepBack((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.Z: ZoomIn(0.4f); break;
                case Keys.X: ZoomOut(0.4f); break;
                case Keys.I: ToggleIdle(); break;
                case Keys.G: m_enableshadows = !m_enableshadows; break;
                case Keys.U: m_shapeDrawer.EnableTexture(!m_shapeDrawer.EnableTexture(false)); break;
                case Keys.H:
                    if ((debugMode & DebugDrawModes.DBG_NoHelpText) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_NoHelpText);
                    else
                        debugMode |= DebugDrawModes.DBG_NoHelpText;
                    break;

                case Keys.W:
                    if ((debugMode & DebugDrawModes.DBG_DrawWireframe) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_DrawWireframe);
                    else
                        debugMode |= DebugDrawModes.DBG_DrawWireframe;
                    break;

                case Keys.P:
                    if ((debugMode & DebugDrawModes.DBG_ProfileTimings) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_ProfileTimings);
                    else
                        debugMode |= DebugDrawModes.DBG_ProfileTimings;
                    break;

                case Keys.M:
                    if ((debugMode & DebugDrawModes.DBG_EnableSatComparison) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_EnableSatComparison);
                    else
                        debugMode |= DebugDrawModes.DBG_EnableSatComparison;
                    break;

                case Keys.N:
                    if ((debugMode & DebugDrawModes.DBG_DisableBulletLCP) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_DisableBulletLCP);
                    else
                        debugMode |= DebugDrawModes.DBG_DisableBulletLCP;
                    break;

                case Keys.T:
                    if ((debugMode & DebugDrawModes.DBG_DrawText) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_DrawText);
                    else
                        debugMode |= DebugDrawModes.DBG_DrawText;
                    break;
                case Keys.Y:
                    if ((debugMode & DebugDrawModes.DBG_DrawFeaturesText) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_DrawFeaturesText);
                    else
                        debugMode |= DebugDrawModes.DBG_DrawFeaturesText;
                    break;
                case Keys.A:
                    if ((debugMode & DebugDrawModes.DBG_DrawAabb) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_DrawAabb);
                    else
                        debugMode |= DebugDrawModes.DBG_DrawAabb;
                    break;
                case Keys.C:
                    if ((debugMode & DebugDrawModes.DBG_DrawContactPoints) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_DrawContactPoints);
                    else
                        debugMode |= DebugDrawModes.DBG_DrawContactPoints;
                    break;
                
                case Keys.D:
                    if ((debugMode & DebugDrawModes.DBG_NoDeactivation) != 0)
                        debugMode = debugMode & (~DebugDrawModes.DBG_NoDeactivation);
                    else
                        debugMode |= DebugDrawModes.DBG_NoDeactivation;
                    if ((debugMode & DebugDrawModes.DBG_NoDeactivation) != 0)
                    {
                        BulletGlobals.gDisableDeactivation = true;
                    }
                    else
                    {
                        BulletGlobals.gDisableDeactivation = false;
                    }
                    break;
                case Keys.O:
                    {
                        m_stepping = !m_stepping;
                        break;
                    }

                case Keys.Space:
                    ClientResetScene();
                    break;
                case Keys.D1:
                    {
                        if ((debugMode & DebugDrawModes.DBG_EnableCCD) != 0)
                            debugMode = debugMode & (~DebugDrawModes.DBG_EnableCCD);
                        else
                            debugMode |= DebugDrawModes.DBG_EnableCCD;
                        break;
                    }

                case Keys.OemPeriod:
                    {
                        ShootBox(GetRayTo(x, y));//getCameraTargetPosition());
                        break;
                    }

                case Keys.OemPlus:
                    {
                        m_ShootBoxInitialSpeed += 10f;
                        break;
                    }
                case Keys.OemMinus:
                    {
                        m_ShootBoxInitialSpeed -= 10f;
                        break;
                    }
	            case Keys.F1:
		            {

			            break;
		            }

	            case Keys.F2:
		            {

			            break;
		            }
	            case Keys.End:
		            {
                        
                        break;
		            }
	            case Keys.Left : StepLeft((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.Right: StepRight((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.Up: StepFront((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.Down: StepBack((STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds)); break;
                case Keys.PageUp: ZoomIn(0.4f); break;
                case Keys.PageDown: ZoomOut(0.4f); break;
	            case Keys.Home : ToggleIdle(); break;

                default:
                    //        std::cout << "unused key : " << key << std::endl;
                    break;
            }

            Game1.Instance.shape_drawer.SetDebugMode(debugMode);
            m_debugDraw.SetDebugMode(debugMode);
        }

        //----------------------------------------------------------------------------------------------

        public virtual void SpecialKeyboard(int key, int x, int y)
        {
        }

        //----------------------------------------------------------------------------------------------

        public virtual void SpecialKeyboardUp(int key, int x, int y)
        {
        }

        //----------------------------------------------------------------------------------------------

        public virtual void Reshape(int w, int h)
        {
        }

        //----------------------------------------------------------------------------------------------

        public virtual void MouseFunc(ref MouseState oldMouseState, ref MouseState newMouseState)
        {
            IndexedVector3 rayTo = GetRayTo(newMouseState.X, newMouseState.Y);

            if (WasReleased(ref oldMouseState,ref newMouseState,2))
            {
                ShootBox(rayTo);
            }
            else if (WasReleased(ref oldMouseState,ref newMouseState,1))
            {
                //apply an impulse
                if (m_dynamicsWorld != null)
                {
                    using (ClosestRayResultCallback rayCallback = BulletGlobals.ClosestRayResultCallbackPool.Get())
                    {
                        rayCallback.Initialize(new IndexedVector3(m_cameraPosition), rayTo);
                        IndexedVector3 ivPos = new IndexedVector3(m_cameraPosition);
                        IndexedVector3 ivTo = new IndexedVector3(rayTo);
                        m_dynamicsWorld.RayTest(ref ivPos, ref ivTo, rayCallback);
                        if (rayCallback.HasHit())
                        {
                            RigidBody body = RigidBody.Upcast(rayCallback.m_collisionObject);
                            if (body != null)
                            {
                                body.SetActivationState(ActivationState.ACTIVE_TAG);
                                IndexedVector3 impulse = rayTo;
                                impulse.Normalize();
                                float impulseStrength = 10f;
                                impulse *= impulseStrength;
                                IndexedVector3 relPos = rayCallback.m_hitPointWorld - body.GetCenterOfMassPosition();
                                body.ApplyImpulse(ref impulse, ref relPos);
                            }
                        }
                    }
                }
            }
            else if (WasPressed(ref oldMouseState,ref newMouseState,0))
            {
                //add a point to point constraint for picking
                if (m_dynamicsWorld != null)
                {

					IndexedVector3 rayFrom;
					if (m_ortho)
					{
						rayFrom = rayTo;
						rayFrom.Z = -100.0f;
					}
					else
					{
						rayFrom = new IndexedVector3(m_cameraPosition);
					}


                    ClosestRayResultCallback rayCallback = new ClosestRayResultCallback(ref rayFrom, ref rayTo);
                    IndexedVector3 ivPos = new IndexedVector3(m_cameraPosition);
                    IndexedVector3 ivTo = new IndexedVector3(rayTo);
                    m_dynamicsWorld.RayTest(ref ivPos, ref ivTo, rayCallback);
                    if (rayCallback.HasHit())
                    {
                        RigidBody body = RigidBody.Upcast(rayCallback.m_collisionObject);
                        if (body != null)
                        {
                            //other exclusions?
                            if (!(body.IsStaticObject() || body.IsKinematicObject()))
                            {
                                pickedBody = body;
                                pickedBody.SetActivationState(ActivationState.DISABLE_DEACTIVATION);


                                IndexedVector3 pickPos = rayCallback.m_hitPointWorld;

                                IndexedVector3 localPivot = body.GetCenterOfMassTransform().Inverse() * pickPos;

                                if (use6Dof)
                                {
                                    IndexedMatrix tr = IndexedMatrix.Identity;
                                    tr._origin = localPivot;
                                    Generic6DofConstraint dof6 = new Generic6DofConstraint(body, ref tr, false);
                                    dof6.SetLinearLowerLimit(new IndexedVector3(0, 0, 0));
                                    dof6.SetLinearUpperLimit(new IndexedVector3(0, 0, 0));
                                    dof6.SetAngularLowerLimit(new IndexedVector3(0, 0, 0));
                                    dof6.SetAngularUpperLimit(new IndexedVector3(0, 0, 0));

                                    m_dynamicsWorld.AddConstraint(dof6);
                                    m_pickConstraint = dof6;

                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_CFM, 0.8f, 0);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_CFM, 0.8f, 1);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_CFM, 0.8f, 2);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_CFM, 0.8f, 3);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_CFM, 0.8f, 4);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_CFM, 0.8f, 5);

                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_ERP, 0.1f, 0);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_ERP, 0.1f, 1);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_ERP, 0.1f, 2);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_ERP, 0.1f, 3);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_ERP, 0.1f, 4);
                                    dof6.SetParam(ConstraintParams.BT_CONSTRAINT_STOP_ERP, 0.1f, 5);
                                }
                                else
                                {
                                    Debug.Assert(m_pickConstraint == null);
                                    Point2PointConstraint p2p = new Point2PointConstraint(body, ref localPivot);
                                    m_dynamicsWorld.AddConstraint(p2p, false);

                                    p2p.m_setting.m_impulseClamp = mousePickClamping;
                                    p2p.m_setting.m_tau = 0.001f;


                                    if (m_pickConstraint != null)
                                    {
                                        int ibreak = 0;
                                    }
                                    m_pickConstraint = p2p;
                                }
                                //save mouse position for dragging
                                gOldPickingPos = rayTo;
								gHitPos = pickPos;

								gOldPickingDist = (pickPos - rayFrom).Length();

                                //very weak constraint for picking
                            }
                        }
                    }
                }

            }
            else if (WasReleased(ref oldMouseState,ref newMouseState,0))
            {
                if (m_pickConstraint != null && m_dynamicsWorld != null)
                {
                    m_dynamicsWorld.RemoveConstraint(m_pickConstraint);
                    m_pickConstraint = null;
                    //printf("removed constraint %i",gPickingConstraintId);
                    m_pickConstraint = null;
                    pickedBody.ForceActivationState(ActivationState.ACTIVE_TAG);
                    pickedBody.SetDeactivationTime(0f);
                    pickedBody = null;
                }
            }
        }

        //----------------------------------------------------------------------------------------------

        public virtual void MouseMotionFunc(ref MouseState mouseState)
        {
            if (m_pickConstraint != null)
            {
                //move the constraint pivot
				Point2PointConstraint p2p = m_pickConstraint as Point2PointConstraint;
                if (p2p != null)
                {
                    //keep it at the same picking distance

                    IndexedVector3 newRayTo = GetRayTo(mouseState.X, mouseState.Y);
					IndexedVector3 rayFrom;
					IndexedVector3 oldPivotInB = p2p.GetPivotInB();
					IndexedVector3 newPivotB;
					if (m_ortho)
					{
						newPivotB = oldPivotInB;
						newPivotB.X = newRayTo.X;
						newPivotB.Y = newRayTo.Y;
					}
					else
					{
						rayFrom = new IndexedVector3(m_cameraPosition);
						IndexedVector3 dir = newRayTo - rayFrom;
						dir.Normalize();
						dir *= gOldPickingDist;
						newPivotB = rayFrom + dir;
					}

                    p2p.SetPivotB(ref newPivotB);
                }
            }
        }

        //----------------------------------------------------------------------------------------------

        public virtual void DisplayCallback()
        {
        }

        //----------------------------------------------------------------------------------------------

        //public virtual void renderme()
        //{
        //    m_shapeDrawer.startDraw(GraphicsDevice, ref m_lookAt, ref m_perspective);
        //}

        //----------------------------------------------------------------------------------------------

        public void StepLeft(float delta)
        {
            m_yaw -= delta;
        }

        //----------------------------------------------------------------------------------------------

        public void StepRight(float delta)
        {
            m_yaw += delta;
            if (m_yaw >= MathUtil.SIMD_2_PI)
            {
                m_yaw -= MathUtil.SIMD_2_PI;
            } 
        }
        
        //----------------------------------------------------------------------------------------------

        public void StepFront(float delta)
        {
            m_pitch += delta;
            if (m_pitch >= MathUtil.SIMD_2_PI)
            {
                m_pitch -= MathUtil.SIMD_2_PI;
            }
        }

        //----------------------------------------------------------------------------------------------

        public void StepBack(float delta)
        {
            m_pitch -= delta;
            if (m_pitch < 0)
            {
                m_pitch += MathUtil.SIMD_2_PI;
            }
        }

        //----------------------------------------------------------------------------------------------

        public void ZoomIn(float delta)
        {
            m_cameraDistance -= delta; 
            if (m_cameraDistance < 0.1f)
            {
                m_cameraDistance = 0.1f;
            }
        }

        //----------------------------------------------------------------------------------------------

        public void ZoomOut(float delta)
        {
            m_cameraDistance += delta; 
        }

        //----------------------------------------------------------------------------------------------

        public bool IsIdle()
        {
            return m_idle;
        }

        //----------------------------------------------------------------------------------------------

        public void SetIdle(bool idle)
        {
            m_idle = idle;
        }

        //----------------------------------------------------------------------------------------------

        public virtual void Initialize()
        {
            InitializeDemo();
        }

        //----------------------------------------------------------------------------------------------

        public void Dispose(bool disposing)
        {
            ShutdownDemo();
        }

        //----------------------------------------------------------------------------------------------

        public virtual void Update(GameTime gameTime)
        {
            UpdateCamera();
            UpdateLights();

            KeyboardState currentKeyboardState = Keyboard.GetState();
            GenerateKeyEvents(ref m_lastKeyboardState, ref currentKeyboardState,gameTime);
            m_lastKeyboardState = currentKeyboardState;

            MouseState mouseState = Mouse.GetState();
            GenerateMouseEvents(ref m_lastMouseState, ref mouseState);
            m_lastMouseState = mouseState;


            GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);
            if (gamePadState.IsConnected)
            {
                GenerateGamePadEvents(ref m_lastGamePadState, ref gamePadState,gameTime);
            }

            MoveAndDisplay(gameTime);

        }

        //----------------------------------------------------------------------------------------------
        
        private void GenerateGamePadEvents(ref GamePadState old, ref GamePadState current, GameTime gameTime)
        {
            if (current.ThumbSticks.Right.LengthSquared() > 0)
            {
                Vector2 right = current.ThumbSticks.Right * STEPSIZEROTATE * (float)gameTime.ElapsedGameTime.TotalSeconds;
                StepLeft(right.X);
                StepFront(right.Y);
            }
            if (current.ThumbSticks.Left.LengthSquared() > 0)
            {
                Vector2 left = current.ThumbSticks.Left * STEPSIZETRANSLATE * (float)gameTime.ElapsedGameTime.TotalSeconds;

                IndexedVector3 forward = m_cameraTargetPosition - m_cameraPosition;
                forward.Normalize();

                float rele = m_pitch;
                float razi = m_yaw;

                IndexedVector3 right = IndexedVector3.Cross(forward, m_cameraUp);

                m_cameraPosition += forward * left.Y;
                m_cameraPosition += right * left.X;
            }

            if (current.Triggers.Left > 0f)
            {
                m_cameraPosition.Y -= STEPSIZETRANSLATE * current.Triggers.Left * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (current.Triggers.Right> 0f)
            {
                m_cameraPosition.Y += STEPSIZETRANSLATE * current.Triggers.Right * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            if (current.Buttons.Back == ButtonState.Pressed)
            {
                Cleanup();
            }

			if (current.Buttons.A == ButtonState.Pressed)
			{
				ClientResetScene();
			}


        }


        //----------------------------------------------------------------------------------------------

        static Enum[] keysEnumValues = GetEnumValues(typeof(Keys));
        private void GenerateKeyEvents(ref KeyboardState old, ref KeyboardState current,GameTime gameTime)
        {
            foreach (Keys key in keysEnumValues)
            {
                bool released = WasReleased(ref old,ref current, key);
                if (released || IsHeldKey(ref current,key))
                {
                    KeyboardCallback(key,0,0,gameTime,released,ref current,ref old);
                }
            }
        }
 
        //----------------------------------------------------------------------------------------------
        // workaround from justastro at : http://forums.create.msdn.com/forums/p/1610/157478.aspx
        
        public static Enum[] GetEnumValues(Type enumType)
        {

          if (enumType.BaseType == typeof(Enum))
          {
            FieldInfo[] info = enumType.GetFields(BindingFlags.Static | BindingFlags.Public);
            Enum[] values = new Enum[info.Length];
            for (int i=0; i<values.Length; ++i)
            {
              values[i] = (Enum)info[i].GetValue(null);
            }
            return values;
          }
          else
          {
             throw new Exception("Given type is not an Enum type");
          }
        }

        //----------------------------------------------------------------------------------------------
        // This is a way of generating 'pressed' events for keys that we want to hold down
        private bool IsHeldKey(ref KeyboardState current,Keys key)
        {
            return (current.IsKeyDown(key) && ((key == Keys.Left || key == Keys.Right || key == Keys.Up || 
                key == Keys.Down || key == Keys.PageUp || key == Keys.PageDown)));
        }
        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref KeyboardState old, ref KeyboardState current, Keys key)
        {
            // figure out if the key was released between states.
            return old.IsKeyDown(key) && !current.IsKeyDown(key);
        }

        //----------------------------------------------------------------------------------------------

        private bool WasReleased(ref MouseState old, ref MouseState current, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                return old.LeftButton == ButtonState.Pressed && current.LeftButton == ButtonState.Released;
            }
            if (buttonIndex == 1)
            {
                return old.MiddleButton == ButtonState.Pressed && current.MiddleButton == ButtonState.Released;
            }
            if (buttonIndex == 2)
            {
                return old.RightButton == ButtonState.Pressed && current.RightButton == ButtonState.Released;
            }
            return false;
        }

        //----------------------------------------------------------------------------------------------

        private bool WasPressed(ref MouseState old, ref MouseState current, int buttonIndex)
        {
            if (buttonIndex == 0)
            {
                return old.LeftButton == ButtonState.Released && current.LeftButton == ButtonState.Pressed;
            }
            if (buttonIndex == 1)
            {
                return old.MiddleButton == ButtonState.Released && current.MiddleButton == ButtonState.Pressed;
            }
            if (buttonIndex == 2)
            {
                return old.RightButton == ButtonState.Released && current.RightButton == ButtonState.Pressed;
            }
            return false;
        }


       //----------------------------------------------------------------------------------------------

        public void GenerateMouseEvents(ref MouseState oldState, ref MouseState newState)
        {
            MouseFunc(ref oldState, ref newState);
			MouseMotionFunc(ref newState);
        }
        
        
        
        //----------------------------------------------------------------------------------------------

        public void Draw(GameTime gameTime)
        {
            RenderSceneAll(gameTime);
            
        }
        //----------------------------------------------------------------------------------------------
        public void ShowProfileInfo(int xOffset, int yStart, int yIncr)
        {
            if (m_profileManager != null)
            {
                double time_since_reset = 0f;
                if (!m_idle)
                {
                    time_since_reset = m_profileManager.Get_Time_Since_Reset();
                }


                {
                    //recompute profiling data, and store profile strings
                    String blockTime;
                    double totalTime = 0;

                    int frames_since_reset = m_profileManager.Get_Frame_Count_Since_Reset();

                    m_profileIterator.First();

                    double parent_time = m_profileIterator.Is_Root() ? time_since_reset : m_profileIterator.Get_Current_Parent_Total_Time();

                    {
                        blockTime = String.Format("--- Profiling: {0} (total running time: {1:0.000} ms) ---", m_profileIterator.Get_Current_Parent_Name(), parent_time);
                        DisplayProfileString(xOffset, yStart, blockTime);
                        yStart += yIncr;

                        blockTime = "press number (1,2...) to display child timings, or 0 to go up to parent";
                        DisplayProfileString(xOffset, yStart, blockTime);
                        yStart += yIncr;

                    }


                    double accumulated_time = 0f;

                    for (int i = 0; !m_profileIterator.Is_Done(); m_profileIterator.Next())
                    {
                        double current_total_time = m_profileIterator.Get_Current_Total_Time();
                        accumulated_time += current_total_time;
                        double fraction = parent_time > MathUtil.SIMD_EPSILON ? (current_total_time / parent_time) * 100 : 0f;

                        blockTime = String.Format("{0} -- {1} ({2:0.00} %%) :: {3:0.000} ms / frame ({4} calls)",
                            ++i, m_profileIterator.Get_Current_Name(), fraction,
                            (current_total_time / (double)frames_since_reset), m_profileIterator.Get_Current_Total_Calls());
                        DisplayProfileString(xOffset, yStart, blockTime);
                        yStart += yIncr;
                        totalTime += current_total_time;
                    }

                    blockTime = String.Format("{0} ({1:0.000}%) :: {2:0.000} ms", "Unaccounted",
                        // (min(0, time_since_reset - totalTime) / time_since_reset) * 100);
                        parent_time > MathUtil.SIMD_EPSILON ? ((parent_time - accumulated_time) / parent_time) * 100 : 0f, parent_time - accumulated_time);

                    DisplayProfileString(xOffset, yStart, blockTime);
                    yStart += yIncr;



                    blockTime = "-------------------------------------------------";
                    DisplayProfileString(xOffset, yStart, blockTime);
                    yStart += yIncr;
                }
            }
        }

        
        //----------------------------------------------------------------------------------------------

        private void DisplayProfileString(int xOffset, int yStart, String message)
        {
            m_shapeDrawer.DrawText(message,new IndexedVector3(xOffset,yStart,0),new IndexedVector3(1,1,1));
        }

        //----------------------------------------------------------------------------------------------

        public void LoadContent()
        {

            // This needs to be here so that the GraphicsDevice has been created first.

            //VertexDeclaration vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
            //BasicEffect basicEffect = new BasicEffect(GraphicsDevice, null);
            //m_debugDraw = new DefaultDebugDraw(vertexDeclaration,basicEffect);

            //debugMode = DebugDrawModes.DBG_DrawWireframe | DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits;
            //DebugDrawModes debugMode = DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits | DebugDrawModes.DBG_DrawWireframe;
            DebugDrawModes debugMode = DebugDrawModes.DBG_DrawConstraints | DebugDrawModes.DBG_DrawConstraintLimits | DebugDrawModes.DBG_DrawNormals;
            m_shapeDrawer = Game1.Instance.shape_drawer;
            m_debugDraw = m_shapeDrawer;
            m_debugDraw.SetDebugMode(debugMode);
            BulletGlobals.gDebugDraw = m_debugDraw;
            m_shapeDrawer.LoadContent();
            m_shapeDrawer.EnableTexture(true);
            m_enableshadows = true;

        }
        //----------------------------------------------------------------------------------------------

        public void SetSize(int x, int y)
        {
           
        }

        
    }
}
