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

using BulletMonogame;
using BulletMonogame.BulletCollision;
using BulletMonogame.BulletDynamics;
using BulletMonogame.LinearMath;

namespace BulletMonogameDemo.Demos
{
	public class LargeMeshDemo : DemoApplication
	{
		const float SCALING = 1.0f;
		public override void InitializeDemo()
		{
			base.InitializeDemo();
			SetCameraDistance(SCALING * 50f);

            //string filename = @"e:\users\man\bullet\xna-largemesh-output.txt";
            //FileStream filestream = File.Open(filename, FileMode.Create, FileAccess.Write, FileShare.Read);
            //BulletGlobals.g_streamWriter = new StreamWriter(filestream);

			///collision configuration contains default setup for memory, collision setup
			m_collisionConfiguration = new DefaultCollisionConfiguration();

			///use the default collision dispatcher. For parallel processing you can use a diffent dispatcher (see Extras/BulletMultiThreaded)
			m_dispatcher = new CollisionDispatcher(m_collisionConfiguration);

			m_broadphase = new DbvtBroadphase();
			IOverlappingPairCache pairCache = null;
			//pairCache = new SortedOverlappingPairCache();

			m_broadphase = new SimpleBroadphase(1000, pairCache);

			///the default constraint solver. For parallel processing you can use a different solver (see Extras/BulletMultiThreaded)
			SequentialImpulseConstraintSolver sol = new SequentialImpulseConstraintSolver();
			m_constraintSolver = sol;

			m_dynamicsWorld = new DiscreteDynamicsWorld(m_dispatcher, m_broadphase, m_constraintSolver, m_collisionConfiguration);

			IndexedVector3 gravity = new IndexedVector3(0, -10, 0);
			m_dynamicsWorld.SetGravity(ref gravity);

			///create a few basic rigid bodies
			IndexedVector3 halfExtents = new IndexedVector3(50, 50, 50);
			//IndexedVector3 halfExtents = new IndexedVector3(10, 10, 10);
			//CollisionShape groundShape = new BoxShape(ref halfExtents);
			//CollisionShape groundShape = new StaticPlaneShape(IndexedVector3.Up, 50);
			CollisionShape groundShape = BuildLargeMesh();
			m_collisionShapes.Add(groundShape);

			IndexedMatrix groundTransform = IndexedMatrix.CreateTranslation(new IndexedVector3(0, 0, 0));
			//IndexedMatrix groundTransform = IndexedMatrix.CreateTranslation(new IndexedVector3(0,-10,0));
            //IndexedMatrix rotateMatrix = IndexedMatrix.CreateFromYawPitchRoll(0, MathUtil.SIMD_PI / 2.0f, 0);
            //IndexedMatrix rotateMatrix = IndexedMatrix.Identity;
            IndexedMatrix rotateMatrix = IndexedMatrix.Identity;
            rotateMatrix._basis.SetEulerZYX(0, 0, MathUtil.SIMD_PI * 0.7f);


            rotateMatrix._origin = IndexedVector3.Zero;
			float mass = 0f;
			LocalCreateRigidBody(mass, ref rotateMatrix, groundShape);


            CollisionShape boxShape = new BoxShape(new IndexedVector3(0.2f, 0.2f, 0.2f));
            //CollisionShape boxShape = new SphereShape(0.2f);
            //CollisionShape boxShape = new CylinderShapeX(new IndexedVector3(0.2f, 0.4f, 0.2f));
            //CollisionShape boxShape = new CapsuleShape(0.2f, 0.4f);
            IndexedMatrix boxTransform = IndexedMatrix.Identity;
            boxTransform._basis.SetEulerZYX(MathUtil.SIMD_PI * 0.2f, MathUtil.SIMD_PI * 0.4f, MathUtil.SIMD_PI * 0.7f);
			boxTransform._origin = new IndexedVector3(0.0f, 5.0f, 0.0f);


            LocalCreateRigidBody(1.25f, boxTransform, boxShape);

            ClientResetScene();

		}



//static int numTriangles = 2;

//static IndexedVector3[] vertices = {
//    new IndexedVector3(-5,0 ,-5),
//    new IndexedVector3(5 ,-0,-5),
//    new IndexedVector3(5,0,5),
//    new IndexedVector3(-5,-0,5)
//};

//static IndexedVector3[] vertices = {
//    new IndexedVector3(1.78321f ,1.78321f ,-1.783479f),
//    new IndexedVector3(1.78321f ,-1.78321f ,-1.783479f),
//    new IndexedVector3(-1.78321f ,-1.78321f ,-1.783479f),
//    new IndexedVector3(-1.78321f ,1.78321f ,-1.783479f)
//                            };


//static int[] indices = { 0, 1,2 , 0, 2,3 };

static int numTriangles = 14;
static IndexedVector3[] vertices = {
    new IndexedVector3(1.78321f ,1.78321f ,-1.783479f),
    new IndexedVector3(1.78321f ,-1.78321f ,-1.783479f),
    new IndexedVector3(-1.78321f ,-1.78321f ,-1.783479f),
    new IndexedVector3(-1.78321f ,1.78321f ,-1.783479f),
    new IndexedVector3(0.0f ,0.0f ,1.783479f),
    new IndexedVector3(0.0f ,0.0f ,1.783479f),
    new IndexedVector3(0.0f ,0.0f ,1.783479f),
    new IndexedVector3(0.0f ,0.0f ,1.783479f),
    new IndexedVector3(-1.78321f ,-1.78321f ,-1.783479f),
    new IndexedVector3(0.0f ,0.0f ,1.783479f)
};

static int[] indices = { 0, 1, 2, 3, 0, 2, 4, 5, 6, 4, 6, 7, 4, 2, 1, 4, 1, 5, 5, 1, 0, 5, 0, 6, 6, 0, 3, 6, 3, 7, 7, 3, 8, 7, 8, 9, 9, 8, 2, 9, 2, 4 };



CollisionShape BuildLargeMesh()
{
	//int vertStride = sizeof(IndexedVector3);
	//int indexStride = 3*sizeof(int);

	int vertStride = 1;
	int indexStride = 3;

	ObjectArray<IndexedVector3> vertexArray = new ObjectArray<IndexedVector3>();
	for (int i = 0; i < vertices.Length; ++i)
	{
		vertexArray.Add(vertices[i]);
	}

	ObjectArray<int> intArray = new ObjectArray<int>();
	for (int i = 0; i < indices.Length; ++i)
	{
		intArray.Add(indices[i]);
	}
    //TriangleIndexVertexArray indexVertexArray = new TriangleIndexVertexArray(DemoMeshes.BUNNY_NUM_TRIANGLES, DemoMeshes.gBunnyIndices, 3, DemoMeshes.BUNNY_NUM_VERTICES, DemoMeshes.gBunnyVertices, 3);

    TriangleIndexVertexArray indexVertexArray = new TriangleIndexVertexArray(numTriangles, intArray, indexStride, vertexArray.Count, vertexArray, vertStride);
    TriangleMeshShape triangleMesh = new TriangleMeshShape(indexVertexArray);
    //TriangleMeshShape triangleMesh = new BvhTriangleMeshShape(indexVertexArray,true,true);
	return triangleMesh;

}


	}
}
