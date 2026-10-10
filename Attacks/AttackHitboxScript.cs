using System.Collections.Generic;
using UnityEngine;

public class AttackHitboxScript : MonoBehaviour
{
    private List<CharacterBehaviour> _charactersInHitbox = new List<CharacterBehaviour>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CharacterBehaviour character = collision.GetComponent<CharacterBehaviour>();

        if(character != null)
        {
            _charactersInHitbox.Add(character);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CharacterBehaviour character = collision.GetComponent<CharacterBehaviour>();

        if (character != null)
        {
            _charactersInHitbox.Remove(character);
        }
    }

    private void CleanListFromDestroyedObjects()
    {
        _charactersInHitbox.RemoveAll(s => s == null);
    }

    public CharacterBehaviour[] GetCharactersInHitbox()
    {
        CleanListFromDestroyedObjects();
        return _charactersInHitbox.ToArray();
    }
}
