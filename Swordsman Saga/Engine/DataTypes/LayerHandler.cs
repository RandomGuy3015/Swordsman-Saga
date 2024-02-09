using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using Swordsman_Saga.Engine.ObjectManagement;
using Vector2 = Microsoft.Xna.Framework.Vector2;


namespace Swordsman_Saga.Engine.DataTypes
{
    class LayerHandler
    {
        public SortedDictionary<long, IGameObject> mSortedDictionary;
        //public SortedDictionary<float, IGameObject> mLayerStack;
        public PriorityQueue<IGameObject, float> mPriorityQueue;
        private double mRotation;
        private double mMax;
        private Matrix mTransformMatrix;
        double mMatrix211;
        double mMatrix212;
        double mMatrix221;
        double mMatrix222;


        public LayerHandler()
        {
            
            mRotation = Math.Atan(0.5);
            mMax = 10750;
            
            //mSortedDictionary = new SortedDictionary<long, IGameObject>();
            mPriorityQueue = new PriorityQueue<IGameObject, float>();
            //mLayerStack = new SortedDictionary<float, IGameObject>();
            
            double help1 = Math.Cos(mRotation) * mMax + Math.Sin(mRotation) * mMax / 2;
            double help2 = Math.Cos(mRotation) * mMax;
            double matrix111 = 1 - ((help1 - mMax) / help1);
            double matrix112 = (1 - ((help1 - mMax) / help1)) * ((help1 - Math.Sin(mRotation) * mMax) / help2);
            double matrix122 = 1 + ((mMax - help2) / help2);
            mMatrix211 = Math.Cos(mRotation) * matrix111 - Math.Sin(mRotation) * matrix112;
            mMatrix212 = Math.Sin(mRotation) * matrix111 + Math.Cos(mRotation) * matrix112;
            mMatrix221 = -Math.Sin(mRotation) * matrix122;
            mMatrix222 = Math.Cos(mRotation) * matrix122;
        }
        
        public Vector2 TransformCoordinate(Vector2 position)
        {
            return new Vector2((float)(mMatrix211 * position.X + mMatrix212 * position.Y), (float)(mMatrix221 * position.X + mMatrix222 * position.Y));
        }

        public float GetLayerValue(IGameObject gameObject)
        {
            Vector2 coordinate = TransformCoordinate(gameObject.Position);
            return coordinate.X + coordinate.Y;
        }

        public float GetLayerValue2(Vector2 coordinate)
        {
            return coordinate.X + coordinate.Y;
        }

        /**
        public void SetLayerStack()
        {
            foreach (var gameObjectPair in mObjects)
            {
                var gameObject = gameObjectPair.Value;
                mLayerStack.Add(GetLayerValue(gameObject), gameObject);
            }
        }

        public void UpdateLayerStack(Dictionary<string, IGameObject> objects)
        {
            foreach (var gameObjectPair in mObjects)
            {
                mLayerStack.Add(GetLayerValue(gameObjectPair.Value), gameObjectPair.Value);
            }
        }

        public void RemoveFromLayerStack(IGameObject gameObject)
        {
            mLayerStack.Remove(GetLayerValue(gameObject));
        }

        public void AddToStack(float key, IGameObject gameObject)
        {
            if (mLayerStack.ContainsKey(key))
            {
                AddToStack(key + 0.0005f, gameObject);
            }
            else
            {
                mLayerStack.Add(key, gameObject);
            }
        }
        */
        public void UpdateQueue(Dictionary<string, IGameObject> objects)
        {
            foreach (var objectPair in objects)
            {
                mPriorityQueue.Enqueue(objectPair.Value, GetLayerValue(objectPair.Value));
            }
        }
        





    }
}
