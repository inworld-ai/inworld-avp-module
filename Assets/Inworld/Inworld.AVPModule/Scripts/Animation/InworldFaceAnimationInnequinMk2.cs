/*************************************************************************************************
 * Copyright 2022 Theai, Inc. (DBA Inworld)
 *
 * Use of this source code is governed by the Inworld.ai Software Development Kit License Agreement
 * that can be found in the LICENSE.md file or at https://www.inworld.ai/sdk-license
 *************************************************************************************************/

using Inworld.Assets;
using Inworld.Packet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inworld.Sample.Innequin
{
    /// Simple replica of InworldFaceAnimation that has a separate mesh renderer per face part
    /// (to work with the new Innequin model setup)
    public class InworldFaceAnimationInnequinMk2 : InworldFacialAnimation
    {
        [SerializeField] Animator m_EmoteAnimator;
        [SerializeField] SkinnedMeshRenderer[] m_FaceMeshes;
        [SerializeField] FaceTransformData m_FaceTransformData;
        [SerializeField] LipsyncMap m_FaceAnimData;
        [SerializeField] Texture m_DefaultMouth;
        [SerializeField] Material m_Facial;
        [Range(-1, 1)] [SerializeField] float m_BlinkRate;
        List<Texture> m_LipsyncTextures = new List<Texture>();
        List<PhonemeInfo> m_CurrentPhoneme = new List<PhonemeInfo>();

        Texture m_CurrentEyeOpen;
        Texture m_CurrentEyeClosed;

        Material m_matEyeBlow;
        Material m_matEye;
        Material m_matNose;
        Material m_matMouth;

        bool m_IsBlinking;
        float m_CurrentAudioTime;
        static readonly int s_Emotion = Animator.StringToHash("Emotion");
        const int k_VisemeSil = 0;
        const int k_VisemeCount = 15;

        void Start()
        {
            _InitMaterials();
        }

        void _InitMaterials()
        {
            m_matEyeBlow = _CreateFacialMaterial($"eyeBlow_{m_Character.Data.brainName.GetHashCode()}");
            m_matEye = _CreateFacialMaterial($"eye_{m_Character.Data.brainName.GetHashCode()}");
            m_matNose = _CreateFacialMaterial($"nose_{m_Character.Data.brainName.GetHashCode()}");
            m_matMouth = _CreateFacialMaterial($"mouth_{m_Character.Data.brainName.GetHashCode()}");
            m_FaceMeshes[0].material = m_matEyeBlow;
            m_FaceMeshes[1].material = m_matEye;
            m_FaceMeshes[2].material = m_matNose;
            m_FaceMeshes[3].material = m_matMouth;
            _MorphFaceEmotion(FacialEmotion.Neutral);
        }

        Material _CreateFacialMaterial(string matName)
        {
            Material instance = Instantiate(m_Facial);
            instance.name = matName;
            return instance;
        }

        void _MorphFaceEmotion(FacialEmotion emotion)
        {
            FaceTransform facialData = m_FaceTransformData[emotion.ToString()];
            if (facialData == null)
                return;
            m_matEyeBlow.mainTexture = facialData.eyeBlow;
            m_CurrentEyeOpen = facialData.eye;
            m_CurrentEyeClosed = facialData.eyeClosed;
            m_matNose.mainTexture = facialData.nose;
            m_DefaultMouth = facialData.mouthDefault;
            m_LipsyncTextures = facialData.mouth;
        }

        void _ProcessEmotion(string emotionBehavior)
        {
            EmotionMapData emoMapData = m_EmotionMap[emotionBehavior];
            if (emoMapData == null) {
                Debug.LogError($"Unhandled emotion {emotionBehavior}");
                return;
            }

            if (m_EmoteAnimator)
                m_EmoteAnimator.SetInteger(s_Emotion, (int) emoMapData.emoteAnimation);
            _MorphFaceEmotion(emoMapData.facialEmotion);
        }

        protected override void BlinkEyes()
        {
            m_IsBlinking = Mathf.Sin(Time.time) < m_BlinkRate;
            m_matEye.mainTexture = m_IsBlinking ? m_CurrentEyeClosed : m_CurrentEyeOpen;
        }

        protected override void Reset()
        {
            if (m_LipsyncTextures.Count == k_VisemeCount)
                m_matMouth.mainTexture = m_LipsyncTextures[k_VisemeSil];
            else
                m_matMouth.mainTexture = m_DefaultMouth;
        }

        protected override void ProcessLipSync()
        {
            if (!m_Interaction.IsSpeaking) {
                Reset();
                return;
            }

            m_CurrentAudioTime += Time.deltaTime;
            PhonemeInfo data = m_CurrentPhoneme?.LastOrDefault(p => p.startOffset < m_CurrentAudioTime);
            if (data == null || string.IsNullOrEmpty(data.phoneme)) {
                Reset();
                return;
            }

            PhonemeToViseme p2v = m_FaceAnimData.p2vMap.FirstOrDefault(v => v.phoneme == data.phoneme);
            if (p2v == null) {
                //Debug.LogError($"Not Found! {data.phoneme}");
                return;
            }

            if (p2v.visemeIndex >= 0 && p2v.visemeIndex < m_LipsyncTextures.Count)
                m_matMouth.mainTexture = m_LipsyncTextures[p2v.visemeIndex];
        }

        protected override void HandleLipSync(AudioPacket audioPacket)
        {
            m_CurrentAudioTime = 0;
            //Debug.Log("Handling lip sync");
            //foreach (var p in audioPacket.dataChunk.additionalPhonemeInfo) Debug.Log(p.phoneme);
            m_CurrentPhoneme = audioPacket.dataChunk.additionalPhonemeInfo;
        }

        protected override void HandleEmotion(EmotionPacket packet)
        {
            _ProcessEmotion(packet.emotion.behavior.ToUpper());
        }
    }
}