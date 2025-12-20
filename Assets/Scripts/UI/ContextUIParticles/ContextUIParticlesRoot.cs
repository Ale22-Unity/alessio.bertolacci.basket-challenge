using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ContextUIParticlesRoot : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Canvas _canvas;
    [SerializeField] private ContextUIParticle _particlePrefab;
    [SerializeField] private Queue<ContextUIParticle> _particleQueue = new Queue<ContextUIParticle>();
    [SerializeField] private int _queueSize = 20;

    private void Start()
    {
        InitializeParticlesQueue().Forget();
    }

    private void OnDestroy()
    {
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Unsubscribe<PlayContextParticlesEvent>(On);
        }
    }

    private void On(PlayContextParticlesEvent e)
    {
        SpawnParticlesAsync(e).Forget();
    }

    public async UniTask InitializeParticlesQueue()
    {
        AsyncInstantiateOperation<ContextUIParticle> queueInstantiation = InstantiateAsync<ContextUIParticle>(_particlePrefab, _queueSize, transform);
        await UniTask.WaitUntil(() => queueInstantiation.isDone);
        foreach (ContextUIParticle particle in queueInstantiation.Result)
        {
            _particleQueue.Enqueue(particle);
            particle.Initialize(_camera, _canvas, GetComponent<RectTransform>());
            particle.gameObject.SetActive(false);
        }
        if (GameClient.Client != null)
        {
            GameClient.Client.EventBus.Subscribe<PlayContextParticlesEvent>(On);
        }
    }

    public async UniTask SpawnParticlesAsync(PlayContextParticlesEvent data)
    {
        List<UniTask> allTasks = new List<UniTask>();

        foreach (ContextUIParticleData group in data.ParticlesData)
        {
            allTasks.Add(SpawnGroupAsync(group));
        }

        await UniTask.WhenAll(allTasks);
        data.Animation.TrySetResult(true);
    }

    private async UniTask SpawnGroupAsync(ContextUIParticleData data)
    {
        List<UniTask> particleTasks = new List<UniTask>();

        for (int i = 0; i < data.Amount; i++)
        {
            ContextUIParticle newParticle = _particleQueue.Dequeue();
            newParticle.gameObject.SetActive(true);
            newParticle.Setup(data);
            _particleQueue.Enqueue(newParticle);

            particleTasks.Add(newParticle.Completion);

            await UniTask.Delay(TimeSpan.FromSeconds(data.Design.ParticleDelay));
        }

        await UniTask.WhenAll(particleTasks);
    }
}
