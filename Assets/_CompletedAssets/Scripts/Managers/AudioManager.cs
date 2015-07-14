using UnityEngine;
using System.Collections;
using FMOD.Studio;
using System;

namespace CompleteProject
{
	public class AudioManager : MonoBehaviour 
	{
		public static GameObject caveVerbZone;
		FMOD.Studio.EventInstance ambientSoundEvent; 

		
		void Start() 
		{
			// Set up and start ambience audio
			ambientSoundEvent = FMOD_StudioSystem.instance.GetEvent("event:/ambience/wind"); 
			var attributes = FMOD.Studio.UnityUtil.to3DAttributes(transform.position);
			ambientSoundEvent.set3DAttributes(attributes);
			ambientSoundEvent.start();

			// Set up reverb zone for the cave area
			caveVerbZone = GameObject.Find("verbZone");
		}

		// Starts or stops a looping audio event depending on whether the play condition is met
		// Prevents retriggering when play should be continuous
		public static void startStopLoop(FMOD.Studio.EventInstance loopingSound, Boolean playConditionMet, GameObject soundSource)
		{
			var attributes = FMOD.Studio.UnityUtil.to3DAttributes(soundSource.transform.position);
			loopingSound.set3DAttributes(attributes);
			FMOD.Studio.PLAYBACK_STATE state;
			loopingSound.getPlaybackState(out state);

			// Stop looping sound if already playing and play condition is no longer true
			if (state == PLAYBACK_STATE.PLAYING)
			{
				if (!playConditionMet) 
				{
					loopingSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
				}
			}
			// Start looping sound if play condition is met and sound not already playing
			else 
			{
				if(playConditionMet) 
				{
					loopingSound.start();
				}
			}
		}

		// Set reverb amount based on the distance from the reverb zone
		public static void SetReverb(FMOD.Studio.ParameterInstance soundEventReverb, GameObject soundSource)
		{
			float distance = Vector3.Distance(soundSource.transform.position, caveVerbZone.transform.position);
			soundEventReverb.setValue(distance);
		}
		
		void OnDisable()
		{
			ambientSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
		}
	}	
}
