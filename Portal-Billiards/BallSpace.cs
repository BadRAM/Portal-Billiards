using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

public class BallSpace : Game
{
    private List<Ball> _balls = new List<Ball>();
    
    private Texture2D _ballTexture;
    private Texture2D _brickTexture;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    public BallSpace()
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
        for (int i = 0; i < 100; i++)
        {
            _balls.Add(new Ball());
        }

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

    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();


        // foreach (Ball b in _balls)
        // {
        //     b.Update(_balls);
        // }

        for (int i = 0; i < _balls.Count; i++)
        {
            _balls[i].Update(_balls, i);
        }
        
        for (int i = 0; i < _balls.Count; i++)
        {
            _balls[i].LateUpdate();
        }
        
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin();
        //_spriteBatch.Draw(_ballTexture, new Vector2(0, 0), Color.White);
        
        foreach (Ball b in _balls)
        {
            _spriteBatch.DrawCircle(b.Position, 6, 12, Color.Aqua);
        }
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }
}