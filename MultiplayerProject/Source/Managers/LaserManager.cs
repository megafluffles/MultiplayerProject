﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace MultiplayerProject.Source
{
    class LaserManager
    {
        public List<Laser> Lasers
        {
            get { return _laserBeams; }
        }

        private List<Laser> _laserBeams;

        // texture to hold the laser.
        private Texture2D _laserTexture;

        // govern how fast our laser can fire.
        private TimeSpan _laserSpawnTime;
        private TimeSpan _previousLaserSpawnTime;

        private int _screenWidth;
        private int _screenHeight;

        const float SECONDS_IN_MINUTE = 60f;
        const float RATE_OF_FIRE = 200f;
        const float LASER_SPAWN_DISTANCE = 70f;

        public LaserManager(int screenWidth, int screenHeight)
        {
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
        }

        public void Initalise(ContentManager content)
        {
            // Init our laser
            _laserBeams = new List<Laser>();

            _laserSpawnTime = TimeSpan.FromSeconds(SECONDS_IN_MINUTE / RATE_OF_FIRE);
            _previousLaserSpawnTime = TimeSpan.Zero;

            // Load the texture to serve as the laser
            _laserTexture = content.Load<Texture2D>("laser");
        }

        public void Update(GameTime gameTime)
        {
            // Update laserbeams
            for (var i = 0; i < _laserBeams.Count; i++)
            {
                _laserBeams[i].Update(gameTime);
                // Remove the beam when its deactivated or is at the end of the screen.
                if (!_laserBeams[i].Active || _laserBeams[i].Position.X > _screenWidth)
                {
                    _laserBeams.Remove(_laserBeams[i]);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // Draw the lasers.
            foreach (var l in _laserBeams)
            {
                l.Draw(spriteBatch);
            }
        }

        public void FireLaser(GameTime gameTime, Vector2 position, float rotation)
        {
            // Govern the rate of fire for our lasers
            if (gameTime.TotalGameTime - _previousLaserSpawnTime > _laserSpawnTime)
            {
                _previousLaserSpawnTime = gameTime.TotalGameTime;
                // Add the laer to our list.
                AddLaser(position, rotation);
            }
        }

        public void AddLaser(Vector2 position, float rotation)
        {
            Animation laserAnimation = new Animation();
            // Initlize the laser animation
            laserAnimation.Initialize(_laserTexture,
                position,
                rotation,
                46,
                16,
                1,
                30,
                Color.White,
                1f,
                true);

            Laser laser = new Laser();
            Vector2 direction = new Vector2((float)Math.Cos(rotation),
                                     (float)Math.Sin(rotation));
            direction.Normalize();

            // Move the starting position to be slightly in front of the cannon
            var laserPostion = position;
            laserPostion += direction * LASER_SPAWN_DISTANCE;

            // Init the laser
            laser.Initialize(laserAnimation, laserPostion, rotation);
            _laserBeams.Add(laser);
        }
    }
}