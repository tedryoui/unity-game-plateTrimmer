// Original CSG.JS library by Evan Wallace (http://madebyevan.com), under the MIT license.
// GitHub: https://github.com/evanw/csg.js/
//
// C++ port by Tomasz Dabrowski (http://28byteslater.com), under the MIT license.
// GitHub: https://github.com/dabroz/csgjs-cpp/
//
// C# port by Karl Henkel (parabox.co), under MIT license.
//
// Constructive Solid Geometry (CSG) is a modeling technique that uses Boolean
// operations like union and intersection to combine 3D solids. This library
// implements CSG operations on meshes elegantly and concisely using BSP trees,
// and is meant to serve as an easily understandable implementation of the
// algorithm. All edge cases involving overlapping coplanar polygons in both
// solids are correctly handled.

using UnityEngine;
using System.Collections.Generic;

namespace Parabox.CSG
{
    /// <summary>
    /// Base class for CSG operations. Contains GameObject level methods for Subtraction, Intersection, and Union
    /// operations. The GameObjects passed to these functions will not be modified.
    /// </summary>
    public static class CSG
    {
        public enum BooleanOp
        {
            Intersection,
            Union,
            Subtraction
        }

        const float k_DefaultEpsilon = 0.00001f;
        static float s_Epsilon = k_DefaultEpsilon;

        /// <summary>
        /// Tolerance used by <see cref="Plane.SplitPolygon"/> determine whether planes are coincident.
        /// </summary>
        public static float epsilon
        {
            get => s_Epsilon;
            set => s_Epsilon = value;
        }

        /// <summary>
        /// Performs a boolean operation on two GameObjects.
        /// </summary>
        /// <param name="lhs">The base GameObject of the boolean operation</param>
        /// <param name="rhs">The input GameObject of the boolean operation</param>
        /// <returns>A new mesh.</returns>
        public static Model Perform(BooleanOp op, GameObject lhs, GameObject rhs)
        {
            Model lhsModel = new Model(lhs);
            Model rhsModel = new Model(rhs);
            Model newModel;
            
            switch (op)
            {
                case BooleanOp.Intersection:
                    newModel = Intersect(lhsModel, rhsModel);
                    break;
                case BooleanOp.Union:
                    newModel = Union(lhsModel, rhsModel);
                    break;
                case BooleanOp.Subtraction:
                    newModel = Subtract(lhsModel, rhsModel);
                    break;
                default:
                    newModel = null;
                    break;
            }

            return newModel;
        }
        
        public static Model Perform(BooleanOp op, Model lhs, Model rhs)
        {
            Model newModel;
            
            switch (op)
            {
                case BooleanOp.Intersection:
                    newModel = Intersect(lhs, rhs);
                    break;
                case BooleanOp.Union:
                    newModel = Union(lhs, rhs);
                    break;
                case BooleanOp.Subtraction:
                    newModel = Subtract(lhs, rhs);
                    break;
                default:
                    newModel = null;
                    break;
            }

            return newModel;
        }

        /// <summary>
        /// Returns a new mesh by merging @csgModelB with @csgModelB.
        /// </summary>
        /// <param name="csgModelA">The base mesh of the boolean operation.</param>
        /// <param name="csgModelB">The input mesh of the boolean operation.</param>
        /// <returns>A new mesh if the operation succeeds, or null if an error occurs.</returns>
        public static Model Union(Model csgModelA, Model csgModelB)
        {
            Node a = new Node(csgModelA.ToPolygons());
            Node b = new Node(csgModelB.ToPolygons());
        
            List<Polygon> polygons = Node.Union(a, b).AllPolygons();
            
            return new Model(polygons);
        }
        
        /// <summary>
        /// Returns a new mesh by subtracting @csgModelB with @rhs.
        /// </summary>
        /// <param name="csgModelA">The base mesh of the boolean operation.</param>
        /// <param name="csgModelB">The input mesh of the boolean operation.</param>
        /// <returns>A new mesh if the operation succeeds, or null if an error occurs.</returns>
        public static Model Subtract(Model csgModelA, Model csgModelB)
        {
            Node a = new Node(csgModelA.ToPolygons());
            Node b = new Node(csgModelB.ToPolygons());

            List<Polygon> polygons = Node.Subtract(a, b).AllPolygons();
            
            return new Model(polygons);
        }

        /// <summary>
        /// Returns a new mesh by intersecting @csgModelA with @csgModelB.
        /// </summary>
        /// <param name="csgModelA">The base mesh of the boolean operation.</param>
        /// <param name="csgModelB">The input mesh of the boolean operation.</param>
        /// <returns>A new mesh if the operation succeeds, or null if an error occurs.</returns>
        public static Model Intersect(Model csgModelA, Model csgModelB)
        {
            Node a = new Node(csgModelA.ToPolygons());
            Node b = new Node(csgModelB.ToPolygons());

            List<Polygon> polygons = Node.Intersect(a, b).AllPolygons();

            return new Model(polygons);
        }
    }
}
