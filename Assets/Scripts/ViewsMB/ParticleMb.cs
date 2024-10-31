using System.Collections;
using UnityEngine;

public class ParticleMb : MonoBehaviour, IParticles
{
    public string Id => _id;

    [SerializeField] private string _id;
    [SerializeField] private ParticleSystem[] _particles;
    [SerializeField] private float _timeBeforeDestroy = 2f;

    public void PlayParticles()
    {
        foreach(var particle in _particles)
            particle.Play();

        StartCoroutine(DelayBeforeDestroy());
    }

    private IEnumerator DelayBeforeDestroy()
    {
        yield return new WaitForSeconds(_timeBeforeDestroy);
        Destroy(this.gameObject);
    }
}