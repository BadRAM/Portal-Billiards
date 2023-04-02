using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Mime;
using System.Security.Cryptography.X509Certificates;
using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace Portal_Billiards;

public class RingPuzzle : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private FontSystem _fontSystem;
    

    private int _CircleRadius = 16;


    private float _currentTime = 0;

    private List<bool> _ring1 = new List<bool>();
    private List<bool> _ring2 = new List<bool>();
    private List<bool> _ring3 = new List<bool>();
    private List<bool> _ring4 = new List<bool>();

    private Vector2 _root = new Vector2();

    private bool _keyDownLeft;
    private bool _keyDownRight;
    private bool _keyDownUp;
    private bool _keyDownDown;




    public RingPuzzle()
    {
        _graphics = new GraphicsDeviceManager(this);
        IsMouseVisible = true;
        
        _graphics.PreferredBackBufferWidth = 800;  // set this value to the desired width of your window
        _graphics.PreferredBackBufferHeight = 800;   // set this value to the desired height of your window
        _graphics.ApplyChanges();
    }

    protected override void Initialize()
    {
        for (int i = 0; i < 8; i++)
        {
            _ring1.Add(Random.Shared.Next(0, 4) == 1);
        }
        
        for (int i = 0; i < 16; i++)
        {
            _ring2.Add(Random.Shared.Next(0, 4) == 1);
        }
        
        for (int i = 0; i < 24; i++)
        {
            _ring3.Add(Random.Shared.Next(0, 4) == 1);
        }
        
        for (int i = 0; i < 32; i++)
        {
            _ring4.Add(Random.Shared.Next(0, 4) == 1);
        }
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        string path = Path.GetFullPath(@"..\..\..\");

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        _fontSystem = new FontSystem();
        //_fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\ModernDOS8x14.ttf"));
        _fontSystem.AddFont(File.ReadAllBytes(path + "fonts\\alagard.ttf"));
    }

    protected override void Update(GameTime gameTime)
    {
        // Poll for current keyboard state
        KeyboardState state = Keyboard.GetState();
            
        // If they hit esc, exit
        if (state.IsKeyDown(Keys.Escape)) Exit();

        
        if (state.IsKeyDown(Keys.Left))
        {
            if (!_keyDownLeft)
            {
                _keyDownLeft = true;
                _ring1.Add(_ring1[0]);
                _ring1.RemoveAt(0);
                _ring2.Add(_ring2[0]);
                _ring2.RemoveAt(0);
                _ring3.Add(_ring3[0]);
                _ring3.RemoveAt(0);
                _ring4.Add(_ring4[0]);
                _ring4.RemoveAt(0);
            }
        }
        else if (_keyDownLeft) _keyDownLeft = false;
        
        
        if (state.IsKeyDown(Keys.Right))
        {
            if (!_keyDownRight)
            {
                _keyDownRight = true;
                _ring1.Insert(0, _ring1[^1]);
                _ring1.RemoveAt(_ring1.Count-1);
                _ring2.Insert(0, _ring2[^1]);
                _ring2.RemoveAt(_ring2.Count-1);
                _ring3.Insert(0, _ring3[^1]);
                _ring3.RemoveAt(_ring3.Count-1);
                _ring4.Insert(0, _ring4[^1]);
                _ring4.RemoveAt(_ring4.Count-1);
            }
        }
        else if (_keyDownRight) _keyDownRight = false;
        
        
        if (state.IsKeyDown(Keys.Up))
        {
            if (!_keyDownUp)
            {
                _keyDownUp = true;
                bool store = _ring2[0];
                _ring2[0] = _ring3[0];
                _ring3[0] = _ring4[0];
                _ring4[0] = store;
            }
        }
        else if (_keyDownUp) _keyDownUp = false;
        
        

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        
        _spriteBatch.Begin();
        
        
        // Draw distance graph
        const int cw = 800; // Chart Width
        const int co = 50; // Chart Offset

        for (int i = 0; i < 8; i++)
        {
            int r = 75;
            Vector2 center = new Vector2(400 + MathF.Sin((i/8.0f) * MathF.PI * 2) * r, 400 + MathF.Cos((i/8.0f) * MathF.PI * 2) * r);
            int thickness = _ring1[i] ? 16 : 2;
            _spriteBatch.DrawCircle(center, 16, 16, Color.Red, thickness);
        }
        
        for (int i = 0; i < 16; i++)
        {
            int r = 150;
            Vector2 center = new Vector2(400 + MathF.Sin((i/16.0f) * MathF.PI * 2) * r, 400 + MathF.Cos((i/16.0f) * MathF.PI * 2) * r);
            int thickness = _ring2[i] ? 16 : 2;
            _spriteBatch.DrawCircle(center, 16, 16, Color.Red, thickness);
        }
        
        for (int i = 0; i < 24; i++)
        {
            int r = 225;
            Vector2 center = new Vector2(400 + MathF.Sin((i/24.0f) * MathF.PI * 2) * r, 400 + MathF.Cos((i/24.0f) * MathF.PI * 2) * r);
            int thickness = _ring3[i] ? 16 : 2;
            _spriteBatch.DrawCircle(center, 16, 16, Color.Red, thickness);
        }
        
        for (int i = 0; i < 32; i++)
        {
            int r = 300;
            Vector2 center = new Vector2(400 + MathF.Sin((i/32.0f) * MathF.PI * 2) * r, 400 + MathF.Cos((i/32.0f) * MathF.PI * 2) * r);
            int thickness = _ring4[i] ? 16 : 2;
            _spriteBatch.DrawCircle(center, 16, 16, Color.Red, thickness);
        }

        SpriteFontBase font18 = _fontSystem.GetFont(30);
        _spriteBatch.DrawString(font18, $"Cool.", new Vector2(0, 0), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }
}