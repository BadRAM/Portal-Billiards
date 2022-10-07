using System;
using System.Collections.Generic;
using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

public class Game1 : Game
{
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;
    private FontSystem _smallFont;

    private Texture2D _ballTexture;
    private Texture2D _brickTexture;

    private List<Ball> Balls = new List<Ball>();
    private List<Ball> SunkBalls = new List<Ball>();
    private List<Vector2> _pockets = new List<Vector2>();

    private float _aimDir = 0f;
    private float _aimPow = 10f;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        int scale = 1;
        _graphics.PreferredBackBufferWidth = 960 * scale;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 720 * scale; // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        
        base.Initialize();
        
        Balls.Add(new Ball(new Vector2(300,300), 0)); // Add Cueball to index 0
        Balls.Add(new Ball(new Vector2(700,300), 9));
        Balls.Add(new Ball(new Vector2(720, 315), 15));
        Balls.Add(new Ball(new Vector2(720,285), 3));
        Balls.Add(new Ball(new Vector2(740,330), 5));
        Balls.Add(new Ball(new Vector2(740, 300), 8)); // 8 ball
        Balls.Add(new Ball(new Vector2(740,270), 1));
        Balls.Add(new Ball(new Vector2(760,345), 2));
        Balls.Add(new Ball(new Vector2(760,315), 11));
        Balls.Add(new Ball(new Vector2(760,285), 4));
        Balls.Add(new Ball(new Vector2(760,255), 12));
        Balls.Add(new Ball(new Vector2(780,360), 7));
        Balls.Add(new Ball(new Vector2(780,330), 6));
        Balls.Add(new Ball(new Vector2(780, 300), 14));
        Balls.Add(new Ball(new Vector2(780,270), 13));
        Balls.Add(new Ball(new Vector2(780,240), 10));
        
        
        _pockets.Add(new Vector2(102, 102));
        _pockets.Add(new Vector2(102, 498));
        _pockets.Add(new Vector2(500, 100));
        _pockets.Add(new Vector2(500, 500));
        _pockets.Add(new Vector2(898, 102));
        _pockets.Add(new Vector2(898, 498));
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Debug.WriteLine("#######");
        // Debug.WriteLine("gonna load some file, Environment.CurrentDirectory = " + Environment.CurrentDirectory );
        string path = Path.GetFullPath(@"..\..\..\");
        // Debug.WriteLine("Path.GetFullPath = " + path);
        // Debug.WriteLine("#######");
        // Console.WriteLine("Where does this go?");

        // _ballTexture = Texture2D.FromFile(GraphicsDevice, path + "pinball.png");
        // _brickTexture = Texture2D.FromFile(GraphicsDevice, path + "Brick.png");
        
        _fontSystem = new FontSystem();
        _fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\alagard.ttf"));

        _smallFont = new FontSystem();
        _smallFont.AddFont(File.ReadAllBytes(path + "fonts\\pixels.ttf"));

        // using (var fileStream = new FileStream("Content/ball.png", FileMode.Open))
        // {
        //     ballTexture = Texture2D.FromStream(GraphicsDevice, fileStream);
        // }

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        // Poll for current keyboard state
        KeyboardState state = Keyboard.GetState();
            
        // If they hit esc, exit
        if (state.IsKeyDown(Keys.Escape))
            Exit();

        float fine = state.IsKeyDown(Keys.LeftShift) ? 0.5f : 2f;
        
        // control aim direction
        if (state.IsKeyDown(Keys.Right)) { _aimDir += gameTime.GetElapsedSeconds() * fine; }
        if (state.IsKeyDown(Keys.Left))  { _aimDir -= gameTime.GetElapsedSeconds() * fine; }
        
        // control aim power
        if (state.IsKeyDown(Keys.Up))   { _aimPow += gameTime.GetElapsedSeconds() * 2f * fine; }
        if (state.IsKeyDown(Keys.Down)) { _aimPow -= gameTime.GetElapsedSeconds() * 2f * fine; }

        if (Balls[0].Velocity == Vector2.Zero && state.IsKeyDown(Keys.Space))
        {
            Balls[0].Velocity += new Vector2(MathF.Cos(_aimDir), MathF.Sin(_aimDir)) * _aimPow;
        }

        // update balls
        for (int i = 0; i < Balls.Count; i++)
        {
            Balls[i].Update(Balls, i);
        }
        
        // Check if balls are sinking into pockets
        for (int i = 0; i < Balls.Count; i++)
        {
            for (int j = 0; j < _pockets.Count; j++)
            {
                if (Vector2.Distance(Balls[i].Position, _pockets[j]) < 20)
                {
                    Balls[i].Velocity = Vector2.Zero;
                    Balls[i].Position = new Vector2(400 + SunkBalls.Count * 24, 650);
                    SunkBalls.Add(Balls[i]);
                    Balls.RemoveAt(i);
                    break;
                }
            }
        }
        
        // Check ball collisions
        for (int i = 0; i < Balls.Count-1; i++)
        {
            for (int j = i+1; j < Balls.Count; j++)
            {
                Balls[i].CollideWith(Balls[j]);
            }
        }
        
        for (int i = 0; i < Balls.Count; i++)
        {
            Balls[i].LateUpdate();
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // _spriteBatch = new SpriteBatch(GraphicsDevice);
        // RenderTarget2D target = new RenderTarget2D(GraphicsDevice, 820, 615);
        // GraphicsDevice.SetRenderTarget(target);
        
        
        GraphicsDevice.Clear(Color.Black);

        // TODO: Add your drawing code here

        _spriteBatch.Begin();
        //_spriteBatch.Draw(_ballTexture, new Vector2(0, 0), Color.White);

        // draw the table and pockets
        _spriteBatch.DrawRectangle(50, 50, 900, 500, Color.Brown, 400);
        _spriteBatch.DrawRectangle(100, 100, 800, 400, Color.DarkGreen, 400);

        for (int i = 0; i < _pockets.Count; i++)
        {
            _spriteBatch.DrawCircle(_pockets[i], 18, 16, Color.Black, 18);
        }

        for (int i = 0; i < Balls.Count; i++)
        {
            Balls[i].Draw(_spriteBatch, _smallFont);
        }        
        
        for (int i = 0; i < SunkBalls.Count; i++)
        {
            SunkBalls[i].Draw(_spriteBatch, _smallFont);
        }

        if (Balls[0].Velocity == Vector2.Zero)
        {
            Vector2 aimDir = new Vector2(MathF.Cos(_aimDir), MathF.Sin(_aimDir));
            for (int i = 1; i <= 5; i++)
            {
                _spriteBatch.DrawCircle(Balls[0].Position + aimDir * _aimPow * i * 4f, 4, 8, Color.White, 1);
            }
        }
        
        SpriteFontBase font18 = _fontSystem.GetFont(30);
        _spriteBatch.DrawString(font18, $"Power: {Math.Round(_aimPow, 2)}\nSkill: 0", new Vector2(50, 600), Color.White);
        SpriteFontBase font5 = _smallFont.GetFont(12);
        _spriteBatch.DrawString(font5, "1234567890", new Vector2(600, 600), Color.White);


        _spriteBatch.End();

        base.Draw(gameTime);
        
        //set rendering back to the back buffer
        GraphicsDevice.SetRenderTarget(null);

        // //render target to back buffer
        // _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
        // _spriteBatch.Draw(target, new Rectangle(0, 0, GraphicsDevice.DisplayMode.Width, GraphicsDevice.DisplayMode.Height), Color.White);
        // _spriteBatch.End();
    }
}