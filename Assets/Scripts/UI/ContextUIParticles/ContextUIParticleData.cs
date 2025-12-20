using System.Collections.Generic;
using UnityEngine;

public class ContextUIParticleData
{
    public readonly ParticlesContextPosition StartPos;
    public readonly ParticlesContextPosition EndPos;
    public readonly ContextUIParticleDesign Design;
    public readonly Sprite Icon;
    public readonly int Amount;
    public readonly string Text;
    public readonly Color TextColor = Color.white;

    public ContextUIParticleData(ParticlesContextPosition startPos, ParticlesContextPosition endPos, 
        ContextUIParticleDesign design, Sprite icon, int amount, string text, Color textColor)
    {
        StartPos = startPos;
        EndPos = endPos;
        Design = design;
        Icon = icon;
        Amount = amount;
        Text = text;
        TextColor = textColor;
    }

    public ContextUIParticleData(ParticlesContextPosition startPos, ParticlesContextPosition endPos,
        ContextUIParticleDesign design, Sprite icon, int amount)
    {
        StartPos = startPos;
        EndPos = endPos;
        Design = design;
        Icon = icon;
        Amount = amount;
        Text = null;
    }
}
