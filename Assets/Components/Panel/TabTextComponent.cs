using System.Collections.Generic;
using Vortex;
using Raylib_cs;

public class TabTextComponent : TextComponent
{
    public bool IsClipping = false;
    public Rectangle ClippingRect = new Rectangle();

    public override void Draw()
    {
        if(Owner.Transform == null)
            return;

        var usingShader = false;

        if(_fontShader != null &&_fontShader.LoadedShader.Id > 0)
        {
            Raylib.BeginShaderMode(_fontShader.LoadedShader);
            usingShader = true;
        }

        if(IsClipping)
        {
            Raylib.BeginScissorMode(
                (int)ClippingRect.X,
                (int)ClippingRect.Y,
                (int)ClippingRect.Width,
                (int)ClippingRect.Height
            );
        }

        if(_normalFont != null && _normalFont.LoadedFont.Texture.Id > 0)
            Raylib.DrawTextEx(_activeFont, Text, Owner.Transform.Position, FontSize, 1, FontColor);
        else
            Raylib.DrawText(Text, (int)Owner.Transform.Position.X, (int)Owner.Transform.Position.Y, FontSize, FontColor);

        if(IsClipping)
            Raylib.EndScissorMode();

        if(usingShader)
            Raylib.EndShaderMode();
    }
}