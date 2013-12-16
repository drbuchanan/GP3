using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

//using XNAButtonState = Microsoft.Xna.Framework.Input.ButtonState;
//using XNAKeys = Microsoft.Xna.Framework.Input.Keys;

namespace WindowsGame1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
       
        KeyboardState keyboardState;
        KeyboardState oldKeyboardstate;

        GameScreen activescreen;
        StartScreen startScreen;
        ActionScreen actionScreen;
        EndScreen endScreen;
        Instructions instructions;

        public SpriteBatch spriteBatch;
        
        #region User Defined Variables
        //------------------------------------------
        // Added for use with fonts
        //------------------------------------------
        SpriteFont fontToUse;

        //Cameras
        //private Camera cam;
        
        //--------------------------------------------------
        // Added for use with playing Audio via Media player
        //--------------------------------------------------
        private Song bkgMusic;
        private Song introMusic;
        private String songInfo;
        //--------------------------------------------------
        //Set the sound effects to use
        //--------------------------------------------------
        private SoundEffectInstance tardisSoundInstance;
        private SoundEffect tardisSound;
        private SoundEffect explosionSound;
        private SoundEffect firingSound;
        private SoundEffect alienSound;

         // Set the 3D model to draw.
         private Model spaceship;
         private Matrix[] mdlTardisTransforms;
         Spaceship objSpaceship = new Spaceship();

        //Create an alien model
         private Model alien;
         private Matrix[] mdlAlienTransforms;
         private Alien objAlien;       

         // create an array of enemy daleks
         private Model mdlAsteroid;
         private Matrix[] mdAsteroidTransforms;
         private Asteroids[] asteroidList = new Asteroids[GameConstants.NumAsteroids];

         // create an array of laser bullets
         private Model mdlLaser;
         private Matrix[] mdlLaserTransforms;
         private Laser objLaser;
         
        //Random number generator
         private Random random = new Random();

        //Integers
         private KeyboardState lastState;
         private int hitCount;
         private int lives = 3;
         private int score = 0;
         int timer = 100;
        // int alienTimer = 2000;

        // The aspect ratio determines how to scale 3d to 2d projection.
        //Floats
        private float aspectRatio;
        private float laserRotation;

        // Set the position of the camera in world space, for our view matrix.
        //Vector3's
        private Vector3 cameraPosition = new Vector3(0.0f, GameConstants.CameraHeight, 50.0f);

        //Matrices
        private Matrix RotationMatrix;    
        private Matrix viewMatrix;
        private Matrix viewMatrix2;
        private Matrix projectionMatrix;
        private Matrix projectionMatrix2;

        //Texture2D's
        private Texture2D startBackground;
        private Texture2D mainBackground;
        private Texture2D endBackground;
        private Texture2D instructionsBackground;

        //Booleans
        private bool gameStarted = false;
        private bool sound = true;
        private bool camBool = true;

        //Initializes a camera that looks at a dertain position and its field of view
        private void InitCamera()
        {
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;

            viewMatrix = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(60), aspectRatio, 50.0f, 500.0f);
        }
        //Creates a second camera similar to first camera. This one follows the player model
        private void InitCamera2()
        {
            Vector3 camPos2 = new Vector3(0, 20, 50);
            RotationMatrix = Matrix.CreateRotationY(objSpaceship.position.Y);

            Vector3 transformedReference = Vector3.Transform(camPos2, RotationMatrix);

            camPos2 = transformedReference + objSpaceship.position;


            viewMatrix2 = Matrix.CreateLookAt(camPos2,objSpaceship.position,Vector3.Up);
            Viewport view = graphics.GraphicsDevice.Viewport;
            float aspectRatio = (float)view.Width / (float)view.Height;
            projectionMatrix2 = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(90),aspectRatio,50.0f,500.0f);
        }
        
        //Method that sets up button press functions and the movement of the model
        private void MoveModel()
        {
            {
                KeyboardState keyboardState = Keyboard.GetState();
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

                // Create some velocity if the right trigger is down.
                Vector3 mdlVelocityAdd = Vector3.Zero;

                // Find out what direction we should be thrusting, using rotation.
                mdlVelocityAdd.Z = -(float)Math.Cos(objSpaceship.Rotation);
                mdlVelocityAdd.X = -(float)Math.Sin(objSpaceship.Rotation);
                if (gamePadState.IsConnected)
                {
                    if (gamePadState.DPad.Left == ButtonState.Pressed)
                    {
                        objSpaceship.Rotation -= -1.0f * 0.10f;
                    }
                    if (gamePadState.DPad.Right == ButtonState.Pressed)
                    {
                        objSpaceship.Rotation -= 1.0f * 0.10f;
                    }
                    if (gamePadState.DPad.Up == ButtonState.Pressed)
                    {
                        mdlVelocityAdd *= 0.05f;
                        objSpaceship.velocity += mdlVelocityAdd;
                    }
                    if (gamePadState.Buttons.A == ButtonState.Pressed)
                    {
                        //add another bullet.  Find an inactive bullet slot and use it
                        //if all bullets slots are used, ignore the user input

                        //Allows bullet to fire in direction spaceship is facing 
                        laserRotation = objSpaceship.Rotation;
                        Matrix tardisTransform = Matrix.CreateRotationY(laserRotation);
                        objLaser.direction = tardisTransform.Forward;
                        objLaser.speed = GameConstants.LaserSpeedAdjustment;
                        objLaser.position = objSpaceship.position + objLaser.direction;
                        objLaser.isActive = true;
                        firingSound.Play();
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Escape))
                {
                    Exit();
                }
                if (keyboardState.IsKeyDown(Keys.Left))
                {
                    // Rotate left.
                    objSpaceship.Rotation -= -1.0f * 0.10f;
                }

                if (keyboardState.IsKeyDown(Keys.Right))
                {
                    // Rotate right.
                    objSpaceship.Rotation -= 1.0f * 0.10f;
                }

                if (keyboardState.IsKeyDown(Keys.Up))
                {
      
                    // Now scale our direction by how hard the trigger is down.
                    mdlVelocityAdd *= 0.05f;
                    objSpaceship.velocity += mdlVelocityAdd;
                }

                //Selects a camera to view
                if (keyboardState.IsKeyDown(Keys.C) && lastState.IsKeyUp(Keys.C))
                {
                    if (camBool)
                    {
                        camBool = false;
                    }
                    else
                    {

                        camBool = true;
                    }
                }

                //allows user to mute the audio
                if(keyboardState.IsKeyDown(Keys.M)&&lastState.IsKeyUp(Keys.M))
                {
                    //plays the audio
                    if (sound == true)
                    {
                        MediaPlayer.IsMuted = false;
                        SoundEffect.MasterVolume = 1;
                        sound = false;
                    }//Mutes the audio
                    else if (sound == false)
                    {
                        MediaPlayer.IsMuted = true;
                        SoundEffect.MasterVolume = 0;
                        sound = true;
                    }
                }
                

                //are we shooting?
                if (keyboardState.IsKeyDown(Keys.Space)&&lastState.IsKeyUp(Keys.Space))
                {
                    //add another bullet.  Find an inactive bullet slot and use it
                    //if all bullets slots are used, ignore the user input

                    //Allows bullet to fire in direction spaceship is facing 
                    laserRotation = objSpaceship.Rotation;
                    Matrix tardisTransform = Matrix.CreateRotationY(laserRotation);
                    objLaser.direction = tardisTransform.Forward;
                    objLaser.speed = GameConstants.LaserSpeedAdjustment;
                    objLaser.position = objSpaceship.position + objLaser.direction;
                    objLaser.isActive = true;
                    firingSound.Play();
                }

                //Spaceship appears at other end of playfield if it exits the playfield size
                if (objSpaceship.position.X > GameConstants.PlayfieldSizeX)
                    objSpaceship.position.X -= 2 * GameConstants.PlayfieldSizeX;
                if (objSpaceship.position.X < -GameConstants.PlayfieldSizeX)
                    objSpaceship.position.X += 2 * GameConstants.PlayfieldSizeX;
                if (objSpaceship.position.Z > GameConstants.PlayfieldSizeZ)
                    objSpaceship.position.Z -= 2 * GameConstants.PlayfieldSizeZ;
                if (objSpaceship.position.Z < -GameConstants.PlayfieldSizeZ)
                    objSpaceship.position.Z += 2 * GameConstants.PlayfieldSizeZ;
                lastState = keyboardState;
            }
        }

        private void ResetAsteroids()
        {
                float xStart;
                float zStart;
                for (int i = 0; i < GameConstants.NumAsteroids; i++)
                {
                    if (random.Next(2) == 0)
                    {
                        xStart = (float)-GameConstants.PlayfieldSizeX;
                    }
                    else
                    {
                        xStart = (float)GameConstants.PlayfieldSizeX;
                    }
                    zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                    asteroidList[i].position = new Vector3(xStart, 0.0f, zStart);
                    double angle = random.NextDouble() * 2 * Math.PI;
                    asteroidList[i].direction.X = -(float)Math.Sin(angle);
                    asteroidList[i].direction.Z = (float)Math.Cos(angle);
                    asteroidList[i].speed = GameConstants.AsteroidMinSpeed +
                       (float)random.NextDouble() * GameConstants.AsteroidMaxSpeed;
                    asteroidList[i].isActive = true;
                }
        }

        //Resets the alien to a random position within the playfield and moves it around 
        private void ResetAlien()
        {
            float xStart;
            float zStart;
            for(int i =0; i<GameConstants.NumAliens; i++)
            {
                if (random.Next(2) == 0)
                {
                    xStart = (float)-GameConstants.PlayfieldSizeX;
                }
                else
                {
                    xStart = (float)GameConstants.PlayfieldSizeX;
                }
                zStart = (float)random.NextDouble() * GameConstants.PlayfieldSizeZ;
                objAlien.position = new Vector3(xStart, 0.0f, zStart);
                double angle = random.NextDouble() * 2 * Math.PI;
                objAlien.direction.X = -(float)Math.Sin(angle);
                objAlien.direction.Z = (float)Math.Cos(angle);
                objAlien.speed = GameConstants.alienSpeedX +
                   (float)random.NextDouble() * GameConstants.alienSpeedY;
                objAlien.isActive = true;
            }
        }

        private Matrix[] SetupEffectTransformDefaults(Model myModel)
        {
                Matrix[] absoluteTransforms = new Matrix[myModel.Bones.Count];
                myModel.CopyAbsoluteBoneTransformsTo(absoluteTransforms);

                foreach (ModelMesh mesh in myModel.Meshes)
                {
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        if (camBool == false)
                        {
                            effect.Projection = projectionMatrix;
                            effect.View = viewMatrix;
                        }
                        else if (camBool == true)
                        {
                            effect.Projection = projectionMatrix2;
                            effect.View = viewMatrix2;
                        }
                    }
                }

                return absoluteTransforms;   
        }
        
        public void DrawModel(Model model, Matrix modelTransform, Matrix[] absoluteBoneTransforms)
        {
  
                //Draw the model, a model can have multiple meshes, so loop
            foreach (ModelMesh mesh in model.Meshes)
            {
                //This is where the mesh orientation is set
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.World = absoluteBoneTransforms[mesh.ParentBone.Index] * modelTransform;
                    if (camBool == false)
                    {
                        effect.Projection = projectionMatrix;
                        effect.View = viewMatrix;
                    }
                    else if (camBool == true)
                    {
                        effect.Projection = projectionMatrix2;
                        effect.View = viewMatrix2;
                    }
                    
                    //Draw the mesh, will use the effects set above.
                    mesh.Draw();
                }
            }
        }
        
        private void writeText(string msg, Vector2 msgPos, Color msgColour)
        {
            spriteBatch.Begin();
            string output = msg;
            // Find the center of the string
            Vector2 FontOrigin = fontToUse.MeasureString(output) / 2;
            Vector2 FontPos = msgPos;
            // Draw the string
            spriteBatch.DrawString(fontToUse, output, FontPos, msgColour);
            spriteBatch.End();
        }

        #endregion

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            this.IsMouseVisible = false;
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            this.IsMouseVisible = true;
            Window.Title = "Lab 6 - Collision Detection";

            hitCount = 0;
            ResetAsteroids();
            ResetAlien();
            InitCamera();
            InitCamera2();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
          
            startBackground = Content.Load<Texture2D>(".\\Textures\\background");
                mainBackground = Content.Load<Texture2D>(".\\Textures\\sky");
                endBackground = Content.Load<Texture2D>(".\\Textures\\endscreen");
                instructionsBackground = Content.Load<Texture2D>(".\\Textures\\Instructions");
            //Creates a screen that will load in specific aspects such as a background screen
            //these are added to a menu component
            startScreen = new StartScreen(this, spriteBatch,
                    Content.Load<SpriteFont>(".\\Fonts\\DrWho"),
                    startBackground);
            Components.Add(startScreen);
            startScreen.Hide();
          
            actionScreen = new ActionScreen(this, spriteBatch,
                    mainBackground);
            Components.Add(actionScreen);
            actionScreen.Hide();

            endScreen = new EndScreen(this, spriteBatch, 
                Content.Load<SpriteFont>(".\\Fonts\\DrWho"),
                endBackground);
            Components.Add(endScreen);
            endScreen.Hide();

            instructions = new Instructions(this, spriteBatch,
                Content.Load<SpriteFont>(".\\Fonts\\DrWho"),
                instructionsBackground);
            Components.Add(instructions);
            instructions.Hide();

            activescreen = startScreen;
            activescreen.Show();

            string[] menuItems = { "Start Game", "High Scores", "End Game" };

                //-------------------------------------------------------------
                // added to load font
                //-------------------------------------------------------------
            fontToUse = Content.Load<SpriteFont>(".\\Fonts\\DrWho");
                //-------------------------------------------------------------
                // added to load Song
                //-------------------------------------------------------------
            bkgMusic = Content.Load<Song>(".\\Audio\\AsteroidsSong");
            introMusic = Content.Load<Song>(".\\Audio\\AsteroidsTitle");
               

                //songInfo = "Song: " + bkgMusic.Name + " Song Duration: " + bkgMusic.Duration.Minutes + ":" + bkgMusic.Duration.Seconds;
            songInfo = "";
                //-------------------------------------------------------------
                // added to load Model
                //-------------------------------------------------------------
            spaceship = Content.Load<Model>(".\\Models\\wasphunter");
            mdlTardisTransforms = SetupEffectTransformDefaults(spaceship);
            mdlAsteroid = Content.Load<Model>(".\\Models\\asteroid");
            mdAsteroidTransforms = SetupEffectTransformDefaults(mdlAsteroid);
            mdlLaser = Content.Load<Model>(".\\Models\\laser");
            mdlLaserTransforms = SetupEffectTransformDefaults(mdlLaser);
            alien = Content.Load<Model>(".\\Models\\alien");
            mdlAlienTransforms = SetupEffectTransformDefaults(alien);
            //alien = Content.Load<Model>(".\\Models\\Argise The Green Alien");
            //mdDalekTransforms = SetupEffectTransformDefaults(mdlDalek);
                //-------------------------------------------------------------
                // added to load SoundFX's
                //------------------------------------------------------------
            tardisSound = Content.Load<SoundEffect>("Audio\\tardisEdit");
            explosionSound = Content.Load<SoundEffect>("Audio\\explosion2");
            firingSound = Content.Load<SoundEffect>("Audio\\ray_gun");
            alienSound = Content.Load<SoundEffect>("Audio\\alienSound");
            if (activescreen == actionScreen)
            {                    
                tardisSoundInstance = tardisSound.CreateInstance();
                tardisSoundInstance.Play();
            }
                // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            //checks to see if the screen is the start screen then plays the music
            //and switches the screen to the game screen on the press of the 
            //enter button
            if (activescreen == startScreen)
            {
                //MediaPlayer.Resume();
                //MediaPlayer.Play(introMusic);
                //MediaPlayer.IsRepeating = true;

                if (CheckKey(Keys.Enter))
                {
                    if (startScreen.SelectedIndex == 0)
                    {
                        MediaPlayer.Play(bkgMusic);
                        MediaPlayer.IsRepeating = true;
                        activescreen.Hide();
                        activescreen = actionScreen;
                        gameStarted = true;
                    }

                    //switches to the end screen
                    if (startScreen.SelectedIndex == 1)
                    {
                        activescreen.Hide();
                        activescreen = endScreen;
                        gameStarted = false;
                    }
                   
                }
                //Displays instructions screen from start screen at the press
                //of the I button
                if (CheckKey(Keys.I))
                {
                    activescreen.Hide();
                    activescreen = instructions;
                    gameStarted = false;
                }
               
            }
            //press enter while in instructions page to play game
            if (activescreen == instructions)
            {
                if(CheckKey(Keys.Enter))
                {
                    activescreen.Hide();
                    activescreen = actionScreen;
                    gameStarted = true;
                    MediaPlayer.Play(bkgMusic);
                }
            }
            //Press escape to exit game
            if (activescreen == startScreen || activescreen == endScreen)
            {
                if (CheckKey(Keys.Escape))
                {
                    Exit();
                }
            }
            // TODO: Add your update logic here
            //bgLayer1.Update();
            //bgLayer2.Update();

            if(activescreen == endScreen)
            {
                MediaPlayer.Stop();
            }
            //Code for when gameplay actually occurs
            if (activescreen == actionScreen)
            {
                MoveModel();
                InitCamera();
                InitCamera2();
                //int alienTimer = 2000;
                // Add velocity to the current position.
                objSpaceship.position += objSpaceship.velocity;


                // Bleed off velocity over time.
                objSpaceship.velocity *= 0.95f;

                float timeDelta = (float)gameTime.ElapsedGameTime.TotalSeconds;

                for (int i = 0; i < GameConstants.NumAsteroids; i++)
                {
                    asteroidList[i].Update(timeDelta);
                }

                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (objLaser.isActive)
                    {
                        objLaser.Update(timeDelta);
                    }
                }
                for(int i =0; i<GameConstants.NumAliens; i++)
                {
                    if(objAlien.isActive)
                    {
                        objAlien.Update(timeDelta);
                    }
                }
                
                BoundingSphere spaceshipSphere =
                  new BoundingSphere(objSpaceship.position,
                           spaceship.Meshes[0].BoundingSphere.Radius *
                                 GameConstants.SpaceshipBoundingSphereScale);

                BoundingSphere alienSphere = new BoundingSphere(objAlien.position,
                     alien.Meshes[0].BoundingSphere.Radius *
                     GameConstants.AlienBoundingSphereScale);


                //BoundingSphere alienSphere = 
                  //  new BoundingSphere(

                //Creates Bounding Spheres for all Daleks in array list
                for (int i = 0; i < asteroidList.Length; i++)
                {
                    if (asteroidList[i].isActive)
                    {
                        BoundingSphere asteroidSphereA =
                          new BoundingSphere(asteroidList[i].position, mdlAsteroid.Meshes[0].BoundingSphere.Radius *
                                         GameConstants.AsteroidBoundingSphereScale);
                        //Creates bounding Sphere for laser if object is active
                            if (objLaser.isActive)
                            {
                                BoundingSphere laserSphere = new BoundingSphere(
                                objLaser.position, mdlLaser.Meshes[0].BoundingSphere.Radius *
                                     GameConstants.LaserBoundingSphereScale);
                                //If laser hits dalek plays sound destroys dalek and laser
                                //If alien hits a laser
                                if (alienSphere.Intersects(laserSphere))
                                {
                                    //Updates players score and removes both alien and laser
                                    //Resets alien
                                    score = score + 200;
                                    objLaser.isActive = false;
                                    objAlien.isActive = false;
                                    alienSound.Play();
                                    //alienTimer = 2000;
                                    ResetAlien();
                                }

                                if (asteroidSphereA.Intersects(laserSphere))
                                {
                                    explosionSound.Play();
                                    asteroidList[i].isActive = false;
                                    objLaser.isActive = false;
                                    hitCount++;
                                    //Increases score for asteroid hit
                                    //If hit count equals certain amount resets asteroids
                                    score = score + 50;
                                    float asteroidminSpeed = 6.0f;
                                    float asteroidmaxSpeed = 6.0f;
                                    int asteroidNum;
                                    if (hitCount == 15)
                                    {
                                        ResetAsteroids();
                                        //Increases speed of asteroids
                                        asteroidminSpeed = GameConstants.AsteroidMinSpeed + asteroidminSpeed;
                                        asteroidmaxSpeed = GameConstants.AsteroidMaxSpeed + asteroidmaxSpeed;
                                    }
                                    if (hitCount == 30)
                                    {
                                        //increases number of asteroids
                                        asteroidNum = GameConstants.NumAsteroids + 10;
                                        ResetAsteroids();
                                    }
                                    if (hitCount == 45)
                                    {
                                        //increases speed and number
                                        asteroidNum = GameConstants.NumAsteroids + 10;
                                        ResetAsteroids();
                                        asteroidminSpeed = GameConstants.AsteroidMinSpeed + asteroidminSpeed;
                                        asteroidmaxSpeed = GameConstants.AsteroidMaxSpeed + asteroidmaxSpeed;
                                    }

                                    break; //no need to check other bullets
                                }
                        }
                            //Check collision between Dalek and Tardis
                            //When timer reaches zero the asteroids and spaceship can collide again
                            if (timer < 0)
                            {
                                if (asteroidSphereA.Intersects(spaceshipSphere))
                                {
                                    //removes live and resets spaceship back to central position
                                    lives = lives - 1;
                                    objSpaceship.position = Vector3.Zero;
                                    objSpaceship.velocity = Vector3.Zero;
                                    objSpaceship.Rotation = 0.0f;
                                    timer = 5000;
                                    //Game moves onto end screen
                                    if (lives <= 0)
                                    {
                                        explosionSound.Play();
                                        //dalekList[i].direction *= -1.0f;
                                        objSpaceship.isActive = false;
                                        objLaser.isActive = false;
                                        activescreen = endScreen;
                                        activescreen.Show();
                                        break; //no need to check other bullets
                                    }
                                    //if asteroid hits spaceship reduce score
                                    score = score - 100;
                                }
                                if (alienSphere.Intersects(spaceshipSphere))
                                {
                                    lives = 0;
                                    explosionSound.Play();
                                    objSpaceship.isActive = false;
                                    objLaser.isActive = false;
                                    activescreen = endScreen;
                                    activescreen.Show(); 
                                        break;
                                }
                            }
                            //timer reduces for intersection
                            timer--;
                        }
                    }
                base.Update(gameTime);
            }

            oldKeyboardstate = keyboardState;
        }

        private bool CheckKey(Keys theKey)
        {
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardstate.IsKeyDown(theKey);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {           
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //Draws instructions background
            if (activescreen == instructions)
            {
                spriteBatch.Begin();
                spriteBatch.Draw(instructionsBackground, Vector2.Zero, Color.White);
                spriteBatch.End();
            }

            //when action screen draws these functions
            if (activescreen == actionScreen)
            {
                //Draws background image
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);
                spriteBatch.End();
                //Draws asteroids
                for (int i = 0; i < GameConstants.NumAsteroids; i++)
                {
                    if (asteroidList[i].isActive)
                    {
                        Matrix dalekTransform = Matrix.CreateScale(GameConstants.AsteroidScalar) * Matrix.CreateTranslation(asteroidList[i].position);
                        DrawModel(mdlAsteroid, dalekTransform, mdAsteroidTransforms);
                    }
                }
                //Draws lasers
                for (int i = 0; i < GameConstants.NumLasers; i++)
                {
                    if (objLaser.isActive)
                    {
                        Matrix laserTransform = Matrix.CreateScale(GameConstants.LaserScalar) * Matrix.CreateRotationY(laserRotation) *
                            Matrix.CreateTranslation(objLaser.position);
                        DrawModel(mdlLaser, laserTransform, mdlLaserTransforms);
                    }
                }
                //Draws spaceship
                if (objSpaceship.isActive == true)
                {
                    Matrix modelTransform = Matrix.CreateScale(0.09f) * Matrix.CreateRotationY(objSpaceship.Rotation) * Matrix.CreateTranslation(objSpaceship.position);
                    DrawModel(spaceship, modelTransform, mdlTardisTransforms);
                }
                //Draws alien
                for (int i = 0; i < GameConstants.NumAliens; i++)
                {
                    if (objAlien.isActive)
                    {
                        Matrix alientransform = Matrix.CreateScale(GameConstants.AlienScalar) * Matrix.CreateTranslation(objAlien.position);
                        DrawModel(alien, alientransform, mdlAlienTransforms);
                    }
                }
                //Displays both score and lives
                writeText("lives:" + lives, new Vector2(50, 10), Color.Yellow);
                writeText("Score:" + score, new Vector2(600, 10), Color.Chartreuse);
                //Tells player the asteroids have increased speed
                if (hitCount == 15)
                {
                    writeText("Asteroid speed increase", new Vector2(300, 300), Color.White);
                }
            }
           
            base.Draw(gameTime);

            //Displays score on end screen
            if (activescreen == endScreen)
            {
                spriteBatch.Begin();
                spriteBatch.DrawString(fontToUse, "Score:" + score, new Vector2(300, 300), Color.White);
                spriteBatch.End();
            }
        }
    }
}
