using System.Collections;
using UnityEngine;

namespace Core.Interfaces.Gameplay
{
    public interface ICoroutineRunner
    {
        Coroutine RunCoroutine(IEnumerator routine);
        void StopCoroutine(Coroutine coroutine);
    }
}
