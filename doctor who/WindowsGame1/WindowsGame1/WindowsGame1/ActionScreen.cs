using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace WindowsGame1
{
    class ActionScreen : GameScreen
    {

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;
        Texture2D image;
        Rectangle imageRectangle;
        SpriteFont fontToUse;
        
        //Creates a new constructor of the action screen which
        //takes in a few variables
        public ActionScreen(Game1 game, SpriteBatch spriteBatch, Texture2D image)
            : base(game, spriteBatch)
        {
            //The image is set to fit the window
            this.image = image;
            imageRectangle = new Rectangle(
                0,
                0,
                Game.Window.ClientBounds.Width,
                Game.Window.ClientBounds.Height);
        }

        public override void Update(GameTime gameTime)
        {
        
            keyboardState = Keyboard.GetState();
            //Ecits the game
            if (keyboardState.IsKeyDown(Keys.Escape))
                game.Exit();

          
            base.Update(gameTime);
            oldKeyboardState = keyboardState;
        }

        private bool CheckKey(Keys theKey)
        {
            //Checks keyboard state
            return keyboardState.IsKeyUp(theKey) &&
                oldKeyboardState.IsKeyDown(theKey);
        }
        

        public override void Draw(GameTime gameTime)
        {
            //Draws an image to the screen
            spriteBatch.Begin();
            spriteBatch.Draw(image, imageRectangle, Color.White);
            spriteBatch.End();

            
             // TODO: Add your drawing code here
              
                base.Draw(gameTime);
        }
        
    }
}