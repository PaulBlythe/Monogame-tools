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

using Microsoft.Xna.Framework;
using BulletMonogame.BulletCollision;
using BulletMonogame.BulletDynamics;
using BulletMonogame.LinearMath;
using Microsoft.Xna.Framework.Input;

namespace BulletMonogameDemo.Demos
{
    public class CharacterDemo : DemoApplication
    {
        public override void InitializeDemo()
        {
            base.InitializeDemo();
            SetCameraDistance(50f);

            //string filename = @"E:\users\man\bullet\xna-basic-output-1.txt";
            //FileStream filestream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            //BulletGlobals.g_streamWriter = new StreamWriter(filestream);

            ///collision configuration contains default setup for memory, collision setup
            m_collisionConfiguration = new DefaultCollisionConfiguration();

            ///use the default collision dispatcher. For parallel processing you can use a diffent dispatcher (see Extras/BulletMultiThreaded)
            m_dispatcher = new CollisionDispatcher(m_collisionConfiguration);

            IndexedVector3 worldMin = new IndexedVector3 (-1000,-1000,-1000);
	        IndexedVector3 worldMax = -worldMin;
            m_broadphase = new AxisSweep3Internal(ref worldMin, ref worldMax, 0xfffe, 0xffff, 16384, null, false);
            //pairCache = new SortedOverlappingPairCache();

            //m_broadphase = new SimpleBroadphase(1000, pairCache);

            ///the default constraint solver. For parallel processing you can use a different solver (see Extras/BulletMultiThreaded)
            SequentialImpulseConstraintSolver sol = new SequentialImpulseConstraintSolver();
            m_constraintSolver = sol;

            m_dynamicsWorld = new DiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_constraintSolver, m_collisionConfiguration);
            m_dynamicsWorld.GetDispatchInfo().SetAllowedCcdPenetration(0.0001f);

            IndexedVector3 gravity = new IndexedVector3(0, -10, 0);
            m_dynamicsWorld.SetGravity(ref gravity);

            ///create a few basic rigid bodies
            IndexedVector3 halfExtents = new IndexedVector3(50, 50, 50);
            //IndexedVector3 halfExtents = new IndexedVector3(10, 10, 10);
            CollisionShape groundShape = new BoxShape(ref halfExtents);
            //CollisionShape groundShape = new StaticPlaneShape(new IndexedVector3(0,1,0), 50);

            m_collisionShapes.Add(groundShape);

            IndexedMatrix groundTransform = IndexedMatrix.CreateTranslation(new IndexedVector3(0, -50, 0));
            //IndexedMatrix groundTransform = IndexedMatrix.CreateTranslation(new IndexedVector3(0,-10,0));
            float mass = 0f;
            LocalCreateRigidBody(mass, ref groundTransform, groundShape);



            #region CharacterController
	        IndexedMatrix startTransform = IndexedMatrix.Identity;
	        //startTransform.setOrigin (btVector3(0.0, 4.0, 0.0));
	        startTransform._origin = new IndexedVector3(10.210098f,-1.6433364f,16.453260f);
	        
            m_ghostObject = new PairCachingGhostObject();
	        m_ghostObject.SetWorldTransform(startTransform);
	        m_broadphase.GetOverlappingPairCache().SetInternalGhostPairCallback(new GhostPairCallback());
	        float characterHeight=1.75f;
	        float characterWidth =1.75f;
	        ConvexShape capsule = new CapsuleShape(characterWidth,characterHeight);
	        m_ghostObject.SetCollisionShape (capsule);
	        m_ghostObject.SetCollisionFlags (CollisionFlags.CF_CHARACTER_OBJECT);

	        float stepHeight = 0.35f;
            int upAxis = 1;
	        m_character = new KinematicCharacterController (m_ghostObject,capsule,stepHeight,upAxis);

            m_dynamicsWorld.AddCollisionObject(m_ghostObject, CollisionFilterGroups.CharacterFilter, CollisionFilterGroups.StaticFilter | CollisionFilterGroups.DefaultFilter);
	        m_dynamicsWorld.AddAction(m_character);

            #endregion














        }

        public override void ClientMoveAndDisplay(GameTime gameTime)
        {
            IndexedVector3 walkDirection = IndexedVector3.Zero;
            float walkVelocity = 1.1f * 4.0f; 
            float walkSpeed = walkVelocity * gameTime.ElapsedGameTime.Milliseconds / 1000.0f;

            IndexedMatrix xform = m_ghostObject.GetWorldTransform();
            IndexedVector3 forwardDir = xform._basis[2];
            IndexedVector3 upDir = xform._basis[1];
            IndexedVector3 strafeDir = xform._basis[0];

            forwardDir.Normalize();
            upDir.Normalize();
            strafeDir.Normalize();

            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.I))
            {
                walkDirection += forwardDir;

            }
            if (keyboardState.IsKeyDown(Keys.K))
            {
                walkDirection -= forwardDir;

            }
            if (keyboardState.IsKeyDown(Keys.J))
            {
                IndexedMatrix orn = m_ghostObject.GetWorldTransform();
                orn._basis *= IndexedBasisMatrix.CreateFromAxisAngle(new IndexedVector3(0, 1, 0), 0.01f);
                m_ghostObject.SetWorldTransform(orn);

            }
            if (keyboardState.IsKeyDown(Keys.L))
            {
                IndexedMatrix orn = m_ghostObject.GetWorldTransform();
                orn._basis *= IndexedBasisMatrix.CreateFromAxisAngle(new IndexedVector3(0, 1, 0), -0.01f);
                m_ghostObject.SetWorldTransform(orn);

            }

            IndexedVector3 result = walkDirection * walkSpeed;
            m_character.SetWalkDirection(ref result);

            base.ClientMoveAndDisplay(gameTime);
        }



        PairCachingGhostObject m_ghostObject;
        KinematicCharacterController m_character;

    }
}
