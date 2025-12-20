using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayContextParticlesEvent
{
    public readonly List<ContextUIParticleData> ParticlesData;
    public readonly UniTaskCompletionSource<bool> Animation;

    public PlayContextParticlesEvent(List<ContextUIParticleData> particlesData, UniTaskCompletionSource<bool> animation)
    {
        ParticlesData = particlesData;
        Animation = animation;
    }
}
