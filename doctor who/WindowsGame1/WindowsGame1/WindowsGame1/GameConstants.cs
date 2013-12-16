using System;
using System.Collections.Generic;
using System.Text;


namespace WindowsGame1
{
    static class GameConstants
    {
        //camera constants
        public const float CameraHeight = 80.0f;
        public const float PlayfieldSizeX = 90f;
        public const float PlayfieldSizeZ = 60f;
        //Dalek constants
        public const int NumAsteroids = 15;
        public const int smallAsteroid = 2;
        public const float AsteroidMinSpeed = 3.0f;
        public const float AsteroidMaxSpeed = 10.0f;
        public const float AsteroidSpeedAdjustment = 2.5f;
        public const float AsteroidScalar = 0.5f;
        //Alien Constants
        public const int NumAliens = 2;
        public const float alienSpeedX = 4.0f;
        public const float alienSpeedY = 13.0f;
        public const float AlienSpeedAdjustment = 2.5f;
        public const float AlienScalar = 0.03f;
        //collision constants
        public const float AsteroidBoundingSphereScale = 0.35f;  //50% size
        public const float SpaceshipBoundingSphereScale = 0.09f;  //50% size
        public const float LaserBoundingSphereScale = 0.85f;
        public const float AlienBoundingSphereScale = 0.09f;//50% size
        //bullet constants
        public const int NumLasers = 30;
        public const float LaserSpeedAdjustment = 3.0f;
        public const float LaserScalar = 8.0f;
        //Spaceship constants
        public const float velocityScale = 5.0f;
    }
}
