using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace BSA
{
    public class OptionsManager : MonoBehaviour
    {
        // --- Fields -------------------------------------------------------------------------------------------------
        [SerializeField] AudioMixer _audioMixer;
        private Options _options;
        // --- Properties ---------------------------------------------------------------------------------------------
        public static OptionsManager Instance { get; private set; }
        public static Options Options => Instance._options;
        // --- Events -------------------------------------------------------------------------------------------------

        // --- Unity Functions ----------------------------------------------------------------------------------------
        private void Awake()
        {
            if(Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;

            DontDestroyOnLoad(this.gameObject);

            LoadOptions();
        }

        // --- Interface implementations ------------------------------------------------------------------------------

        // --- Event callbacks ----------------------------------------------------------------------------------------

        // --- Public/Internal Methods --------------------------------------------------------------------------------
        public void LoadOptions()
        {
            string optionsPath = GetPath();
            if(File.Exists(optionsPath))
            {
                try
                {
                    string json = File.ReadAllText(optionsPath);
                    _options = JsonUtility.FromJson<Options>(json);
                }
                catch(System.Exception e)
                {
                    _options = new Options();
                    Debug.LogError($"Failed to load Options. Creating new." +
                        $"\n{e}");
                }
            }
            else
            {
                _options = new Options();
            }

            this.DoAfter(0.5f, SetAudioDelayed);
        }

        public void SaveOptions()
        {
            if(_options == null)
                throw new UnassignedReferenceException("Options is NULL.");

            try
            {
                Debug.Log("Saving Options!");
                string json = JsonUtility.ToJson(_options, true);
                File.WriteAllText(GetPath(), json);
            }
            catch(System.Exception e)
            {
                Debug.LogError($"Failed to write Options." +
                    $"\n{e}");
            }
        }

        // --- Protected/Private Methods ------------------------------------------------------------------------------
        private string GetPath()
        {
            return Path.Combine(Application.persistentDataPath, "options.json");
        }

        private void SetAudioDelayed()
        {
            _audioMixer.SetFloat("MasterVolume", -80f + 10 * Options.masterVolume);
            _audioMixer.SetFloat("MusicVolume", -80f + 10 * Options.musicVolume);
            _audioMixer.SetFloat("SfxVolume", -80f + 10 * Options.sfxVolume);
        }

        // ----------------------------------------------------------------------------------------
    }
}