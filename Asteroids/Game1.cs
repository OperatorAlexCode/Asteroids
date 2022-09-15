using Asteroids.gameobjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Asteroids
{
    public class Game1 : Game
    {
        // Int
        public int Height = 850;
        public int Width = 1300;
        public int Margin = 50;
        public int SpawnDelay = 200;

        // Bool
        public bool GameOver;
        public bool MousePressed;
        public bool AsteroidSpawnReady;

        // Other
        private GraphicsDeviceManager graphics;
        private SpriteBatch spritebatch;
        public List<Asteroid> SpawnedAsteroids;
        public Spaceship[] SpawnedSpaceships;
        public MouseState CurrentMouseState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spritebatch = new SpriteBatch(GraphicsDevice);
            graphics.PreferredBackBufferHeight = 750;
            graphics.PreferredBackBufferWidth = 1200;

            Height = Window.ClientBounds.Height;
            Width = Window.ClientBounds.Width;

            SpawnedAsteroids = new();
            SpawnedSpaceships = new Spaceship[5];

            GameOver = false;
            MousePressed = false;
            AsteroidSpawnReady = true;

            for (int x = 0; x < SpawnedSpaceships.Length; x++)
                SpawnedSpaceships[x] = CreateSpaceship();

            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            CurrentMouseState = Mouse.GetState();

            if (CurrentMouseState.LeftButton == ButtonState.Released)
                MousePressed = false;

            if (CurrentMouseState.LeftButton == ButtonState.Pressed && !MousePressed)
            {
                MousePressed = true;
                Point mousePos = new(CurrentMouseState.X, CurrentMouseState.Y); 
                foreach(Asteroid asteroid in SpawnedAsteroids)
                    if (asteroid.Hitbox.Contains(mousePos))
                    {
                        SpawnedAsteroids.Remove(asteroid);
                        break;
                    }
            }

            while (!GameOver && SpawnedAsteroids.Count <= 10)
                SpawnedAsteroids.Add(CreateAsteroid());

            foreach (Asteroid asteroid in SpawnedAsteroids)
                asteroid.Update();
            

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spritebatch.Begin();

            foreach (Spaceship spaceship in SpawnedSpaceships)
                spaceship.Draw(spritebatch);

            foreach (Asteroid asteroid in SpawnedAsteroids)
                asteroid.Draw(spritebatch);

            // TODO: Add your drawing code here

            spritebatch.End();

            base.Draw(gameTime);
        }

        public Asteroid CreateAsteroid()
        {
            Vector2 spawnPos = new();

            switch (new Random().Next(1,4))
            {
                case 1:
                    spawnPos = new(25-Margin,25-Margin);
                    break;
                case 2:
                    spawnPos = new(Width+Margin, 25-Margin);
                    break;
                case 3:
                    spawnPos = new(25-Margin, Height+Margin-25);
                    break;
                case 4:
                    spawnPos = new(Width+Margin-25, Height+Margin-25);
                    break;
            }

            Vector2 velocity = new(new Random().Next(-3, 3), new Random().Next(-3, 5));

            Asteroid asteroidToSpawn = new(Content.Load<Texture2D>("Asteroid"), spawnPos, velocity, 0.5f, Height, Width, Margin);

            return asteroidToSpawn;
        }

        public Spaceship CreateSpaceship()
        {
            Vector2 spawnPos = new(new Random().Next(Margin, Width - Margin), new Random().Next(Margin, Height - Margin));

            Spaceship spaceshipToSpawn = new(Content.Load<Texture2D>("Spaceship"), spawnPos, new Random().NextSingle(), Height, Width, Margin);

            return spaceshipToSpawn;
        }
    }
}