using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private float footstepTimer;
    private float footstepTimerMax = .5f;
    private Character character;
    private MovementComponent movementComponent;

    private void Awake()
    {
        character = GetComponent<Character>();
        movementComponent = GetComponent<MovementComponent>();
    }

    private void Update()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer < 0f)
        {
            footstepTimer = footstepTimerMax;
            float volume = 0.3f;
            if (movementComponent.MovementVector != Vector3.zero)
            {
                SoundManager.instance.PlayFootstepsSound(character.transform.position, volume);
            }
        }
    }

}
