using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace WindowsGame1
{
    class Spaceship
    {
        public Model spaceship;
        //-------------------------------------
        //Matrices
        //-------------------------------------
        public Matrix[] transforms;
        public Matrix rotationMatrix = Matrix.Identity;
        //-------------------------------------
        //Vector3's
        //-------------------------------------
        public Vector3 position = Vector3.Zero;
        public Vector3 velocity = Vector3.Zero;
        //-------------------------------------
        //Booleans
        //-------------------------------------
        public bool isActive = true;
        //-------------------------------------
        //Floats
        //-------------------------------------
        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set {
                float newRot = value;
                while(newRot>= MathHelper.TwoPi)
                {
                    newRot -= MathHelper.TwoPi;
                }
                while(newRot < 0)
                {
                    newRot += MathHelper.TwoPi;
                }
                if(rotation != newRot)
                {
                    rotation = newRot;
                    rotationMatrix = Matrix.CreateRotationY(rotation);
                }
            }
        }

    }
}
