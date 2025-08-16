using System.Collections;
using Core.Interfaces.Gameplay;
using UnityEngine;

namespace Core.Components.Gameplay
{
    public class CoroutineRunner : MonoBehaviour, ICoroutineRunner
    {

        public Coroutine RunCoroutine(IEnumerator routine)
        {
            return StartCoroutine(routine);
        }
        public new void StopCoroutine(Coroutine coroutine)
        {
            if (coroutine != null)
            {
                base.StopCoroutine(coroutine);
            }
        }
    }
}

