using UnityEngine;
using System.Collections;

public class SoundEffectManager: MonoBehaviour{

	private string[] abilityNames = new string[] {"heal", "bomb", "supFire"};

	public void UpdateSound() 
	{
        if (BattleManager.ActiveCharacter.IsDead || BattleManager.ActiveCharacter == null)
            return;
       
		switch (BattleManager.ActiveCharacter.soundStatus)
		{
			case SoundStatus.Hit:
				Debug.Log("POW");
				foreach (AudioSource soundComp in BattleManager.ActiveCharacter.GetComponents<AudioSource>())
				{
					if (soundComp.clip.name.Equals("pow")) {
						soundComp.Play();
						break;
					}
				}

				BattleManager.ActiveCharacter.soundStatus = SoundStatus.None;
				break;

			case SoundStatus.Miss:
				Debug.Log("MISS");
				foreach (AudioSource soundComp in BattleManager.ActiveCharacter.GetComponents<AudioSource>())
				{
					if (soundComp.clip.name.Equals("miss")) {
						soundComp.Play();
						break;
					}
				}
			
				BattleManager.ActiveCharacter.soundStatus = SoundStatus.None;
				break;

			case SoundStatus.Critical:
				Debug.Log("CRIT");
				foreach (AudioSource soundComp in BattleManager.ActiveCharacter.GetComponents<AudioSource>())
				{
					if (soundComp.clip.name.Equals("crit")) {
						soundComp.Play();
						break;
					}
				}
			
				BattleManager.ActiveCharacter.soundStatus = SoundStatus.None;
				break;

			case SoundStatus.Ability:
				Debug.Log("WOW");
				foreach (AudioSource soundComp in BattleManager.ActiveCharacter.GetComponents<AudioSource>())
				{
					foreach (string abilityName in abilityNames)
					{
						if (soundComp.clip.name.Equals(abilityName)) {
							if (soundComp.isPlaying == false)
								soundComp.Play();
							break;
						}
					}
				}
				
				BattleManager.ActiveCharacter.soundStatus = SoundStatus.None;
				break;

			case SoundStatus.Walk:
				Debug.Log("WALK");
				foreach (AudioSource soundComp in BattleManager.ActiveCharacter.GetComponents<AudioSource>())
				{
					if (soundComp.clip.name.Equals("walk")) {
						if (soundComp.isPlaying == false)
							soundComp.Play();
						break;
					}
				}

				break;
		}
	}
}
