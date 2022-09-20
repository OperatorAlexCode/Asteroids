using Asteroids.gameobjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Threading;
using System.Windows.Forms;
using ButtonState = Microsoft.Xna.Framework.Input.ButtonState;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Asteroids
{
    public class Game1 : Game
    {
        // Int
        public int Height = 2050;
        public int Width = 2500;
        public int Margin = 75;
        public int SpawnDelay = 200;
        public int PointsGain = 10;
        public int MaxAsteroids = 20;
        public int Points;

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
            Window.Title = $"Asteroids | {Points} Points";

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.R))
                Restart();

            foreach (Asteroid asteroid in SpawnedAsteroids)
                asteroid.Update();

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
                        AsteroidDestroy(asteroid);
                        break;
                    }
            }

            foreach(Asteroid asteroid in SpawnedAsteroids)
            {
                List<Asteroid> intersecting = SpawnedAsteroids.FindAll(ast => ast != asteroid && asteroid.Hitbox.Intersects(ast.Hitbox));
                if (intersecting.Count > 0)
                {
                    foreach (Asteroid ast in intersecting)
                        AsteroidPhysics2D(asteroid,ast);
                }
            }

            if (!GameOver && SpawnedAsteroids.Count <= MaxAsteroids)
                SpawnedAsteroids.Add(CreateAsteroid());

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

        /// <summary>
        /// Creates an asteroid
        /// </summary>
        /// <param name="asteroidSplit">If the asteroid is to be created of a destroyed asteroid</param>
        /// <param name="splitAsteroid">Asteroid to be split</param>
        /// <param name="splitScale">The scale of the split asteroid</param>
        /// <returns></returns>
        public Asteroid CreateAsteroid(bool asteroidSplit = false, Asteroid splitAsteroid = null, float splitScale = .4f)
        {
            int speedMargin = 5;
            int spawnMargin = 30;

            Vector2 spawnPos = new();
            Vector2 velocity = new();

            Asteroid asteroidToSpawn = new(Content.Load<Texture2D>("Asteroid"), spawnPos, velocity, 0.5f, Height, Width, Margin);

            if (asteroidSplit)
            {
                spawnPos = splitAsteroid.Pos;

                velocity = new(new Random().Next(1-speedMargin,speedMargin-1), new Random().Next(1-speedMargin, speedMargin-1));

                asteroidToSpawn = new(Content.Load<Texture2D>("Asteroid"), spawnPos, velocity, splitScale, Height, Width, Margin);
            }
            else
            {
                switch (new Random().Next(1, 4))
                {
                    case 1:
                        spawnPos = new(spawnMargin - Margin, spawnMargin - Margin);
                        velocity = new(new Random().Next(1, speedMargin), new Random().Next(1, speedMargin));
                        break;
                    case 2:
                        spawnPos = new(Width + Margin, spawnMargin - Margin);
                        velocity = new(new Random().Next(-speedMargin, -1), new Random().Next(-speedMargin, -1));
                        break;
                    case 3:
                        spawnPos = new(spawnMargin - Margin, Height + Margin - spawnMargin);
                        velocity = new(new Random().Next(-speedMargin, -1), new Random().Next(1, speedMargin));
                        break;
                    case 4:
                        spawnPos = new(Width + Margin - spawnMargin, Height + Margin - spawnMargin);
                        velocity = new(new Random().Next(1, speedMargin), new Random().Next(-speedMargin, -1));
                        break;
                }

                asteroidToSpawn = new(Content.Load<Texture2D>("Asteroid"), spawnPos, velocity, 0.5f, Height, Width, Margin);
            }

            return asteroidToSpawn;
        }

        public Spaceship CreateSpaceship()
        {
            Vector2 spawnPos = new(new Random().Next(Margin, Width - Margin), new Random().Next(Margin, Height - Margin));

            Spaceship spaceshipToSpawn = new(Content.Load<Texture2D>("Spaceship"), spawnPos, new Random().NextSingle()+.1f, Height, Width, Margin);

            return spaceshipToSpawn;
        }

        /// <summary>
        /// Does velocity calcuations of intersecting asteroids
        /// </summary>
        /// <param name="obj1">Asteroid 1</param>
        /// <param name="obj2">Asteroid 2</param>
        public void AsteroidPhysics2D(Asteroid obj1, Asteroid obj2)
        {
            Vector2 velRelative = Vector2.Multiply(obj2.Velocity - obj1.Velocity, obj2.Center - obj1.Center);
            if (Vector2.Distance(obj1.Center, obj2.Center) < obj1.HitboxRadius + obj2.HitboxRadius)
            {
                if (velRelative.X + velRelative.Y < 0 || velRelative.Y + velRelative.X < 0)
                {
                    Vector2 v1 = obj1.Velocity;
                    Vector2 v2 = obj2.Velocity;

                    Vector2 newVelocity1 = (v1 + v2 + (v2 - v1)) / 2;
                    Vector2 newVelocity2 = (v1 + v2 + (v1 - v2)) / 2;

                    obj1.Velocity = newVelocity1;
                    obj2.Velocity = newVelocity2;
                }
            }
        }

        /// <summary>
        /// Splits  asteroid
        /// </summary>
        /// <param name="asteroidToBeDestroyed">asteroid that's to be destroyed</param>
        public void AsteroidDestroy(Asteroid asteroidToBeDestroyed)
        {
            if (asteroidToBeDestroyed.Size > .4f)
            {
                switch (new Random().Next(1, 4))
                {
                    case 2:
                        for (int x = 0; x < 2; x++)
                        {
                            SpawnedAsteroids.Add(CreateAsteroid(true, asteroidToBeDestroyed));
                        }
                        break;
                    case 3:
                        for (int x = 0; x < 3; x++)
                        {
                            SpawnedAsteroids.Add(CreateAsteroid(true, asteroidToBeDestroyed, .3f));
                        }
                        break;
                }
            }

            SpawnedAsteroids.Remove(asteroidToBeDestroyed);
            Points += PointsGain;
        }

        /// <summary>
        /// Restarts game
        /// </summary>
        public void Restart()
        {
            SpawnedAsteroids = new();
            SpawnedSpaceships = new Spaceship[5];

            GameOver = false;
            MousePressed = false;
            AsteroidSpawnReady = true;

            Points = 0;

            for (int x = 0; x < SpawnedSpaceships.Length; x++)
                SpawnedSpaceships[x] = CreateSpaceship();
        }
    }
}