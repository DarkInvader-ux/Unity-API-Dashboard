using Core.Components.Gameplay;
using Core.Configuration.Api;
using Core.Controllers.Gameplay;
using Core.Data;
using Core.Interfaces;
using Core.Interfaces.Api;
using Core.Interfaces.Gameplay;
using Core.Services;
using Core.Services.Events;
using Zenject;
using UnityEngine;
using ILogger = Core.Interfaces.Api.ILogger;

namespace Core.Utilities
{
    
    public class GameInstaller : MonoInstaller
    {
        [SerializeField] private CoroutineRunner coroutineRunnerPrefab;

        public override void InstallBindings()
        {
            InstallInfrastructure();
            InstallGameplay();

        }

        private void InstallInfrastructure()
        {
            Container.Bind<CoroutineRunner>()
                .FromComponentInNewPrefab(coroutineRunnerPrefab)
                .AsSingle()
                .NonLazy();
            Container.Bind<IEventsService>().To<EventsService>().AsSingle();
            Container.Bind<IHttpClient>().To<UnityHttpClient>().AsSingle();
            Container.Bind<ILogger>().To<UnityConsoleLogger>().AsSingle();
            Container.Bind<ApiConfiguration>().AsSingle();
            Container.Bind<IScoreManager>().To<ScoreManager>().AsSingle();
            
        }

        private void InstallGameplay()
        {
            Container.Bind<TargetSpawnConfig>()
                .FromScriptableObjectResource("Configs/TargetSpawnConfig") 
                .AsSingle();

            Container.BindFactory<Target, Target.Factory>()
                .FromComponentInNewPrefabResource("Prefabs/Target"); 
            Container.Bind<ITargetHitHandler>().To<TargetHitHandler>().AsSingle();
            Container.BindInterfacesAndSelfTo<TargetSpawner>().AsSingle().NonLazy();
            foreach (var target in FindObjectsOfType<Target>())
            {
                Container.QueueForInject(target);
            }
        }

    }

}
