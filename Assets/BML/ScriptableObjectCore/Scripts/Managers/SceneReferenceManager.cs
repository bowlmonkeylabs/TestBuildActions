﻿using System;
using System.Collections;
using System.Collections.Generic;
using BML.ScriptableObjectCore.Scripts.SceneReferences;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.ScriptableObjectCore.Scripts.Managers
{
    public class SceneReferenceManager : MonoBehaviour
    {
        [SerializeField] [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
        private List<TransformScenePair> TransformSceneList = new List<TransformScenePair>();
        
        [SerializeField] [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
        private List<GameObjectScenePair> GameObjectSceneList = new List<GameObjectScenePair>();
        
        [SerializeField] [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
        private List<ParticleSystemScenePair> ParticleSystemSceneList = new List<ParticleSystemScenePair>();

        [SerializeField] [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
        private List<AnimatorScenePair> AnimatorSceneList = new List<AnimatorScenePair>();

        [SerializeField] [PropertySpace(SpaceBefore = 5, SpaceAfter = 5)]
        private List<CameraScenePair> CameraSceneList = new List<CameraScenePair>();

        private void Awake()
        {
            foreach (var transformPair in TransformSceneList)
            {
                if (transformPair.AreValuesAssigned)
                    transformPair.AssignValueToReference();
                else
                    Debug.LogError("Scene Reference NOT assigned!");
            }
            foreach (var gameObjectScenePair in GameObjectSceneList)
            {
                if (gameObjectScenePair.AreValuesAssigned)
                    gameObjectScenePair.AssignValueToReference();
                else
                    Debug.LogError("Scene Reference NOT assigned!");
            }
            foreach (var particleSystemScenePair in ParticleSystemSceneList)
            {
                if (particleSystemScenePair.AreValuesAssigned)
                    particleSystemScenePair.AssignValueToReference();
                else
                    Debug.LogError("Scene Reference NOT assigned!");
            }
            foreach (var animatorScenePair in AnimatorSceneList)
            {
                if (animatorScenePair.AreValuesAssigned)
                    animatorScenePair.AssignValueToReference();
                else
                    Debug.LogError("Scene Reference NOT assigned!");
            }
            foreach (var cameraScenePair in CameraSceneList)
            {
                if (cameraScenePair.AreValuesAssigned)
                    cameraScenePair.AssignValueToReference();
                else
                    Debug.LogError("Scene Reference NOT assigned!");
            }
        }
    }

    [Serializable]
    public class ScenePair<T, RT> where RT: SceneReference<T>
    {
        [SerializeField] [LabelText("Reference")] [LabelWidth(125f)]
        [Required]
        private RT sceneReference;

        [SerializeField] [LabelText("Scene Value")] [LabelWidth(125f)]
        [Required] [SceneObjectsOnly]
        private T sceneValue;

        public bool AreValuesAssigned =>
            (sceneValue != null && sceneReference != null);
        
        public void AssignValueToReference()
        {
            sceneReference.Value = sceneValue;
        }
    }

    [Serializable]
    public class TransformScenePair : ScenePair<Transform, TransformSceneReference> {}

    [Serializable]
    public class GameObjectScenePair : ScenePair<GameObject, GameObjectSceneReference> {}

    [Serializable]
    public class ParticleSystemScenePair : ScenePair<ParticleSystem, ParticleSystemSceneReference> {}

    [Serializable]
    public class AnimatorScenePair : ScenePair<Animator, AnimatorSceneReference> {}

    [Serializable]
    public class CameraScenePair : ScenePair<Camera, CameraSceneReference> {}

}
