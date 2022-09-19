﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

public class Game1 : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private Texture2D _ballTexture;
    private Texture2D _brickTexture;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        Debug.WriteLine("#######");
        Debug.WriteLine("gonna load some file, Environment.CurrentDirectory = " + Environment.CurrentDirectory );
        string path = Path.GetFullPath(@"..\..\..\");
        Debug.WriteLine("Path.GetFullPath = " + path);
        Debug.WriteLine("#######");
        Console.WriteLine("Where does this go?");

        _ballTexture = Texture2D.FromFile(GraphicsDevice, path + "pinball.png");
        _brickTexture = Texture2D.FromFile(GraphicsDevice, path + "Brick.png");
        
        // using (var fileStream = new FileStream("Content/ball.png", FileMode.Open))
        // {
        //     ballTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
        // }

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();
        
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        //_spriteBatch.Draw(_ballTexture, new Vector2(0, 0), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}