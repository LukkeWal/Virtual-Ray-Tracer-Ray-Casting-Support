using System.Collections.Generic;
using _Project.Ray_Tracer.Scripts.RT_Scene.RT_Area_Light;
using _Project.Ray_Tracer.Scripts.RT_Scene.RT_Camera;
using _Project.Ray_Tracer.Scripts.RT_Scene.RT_Light;
using _Project.Ray_Tracer.Scripts.RT_Scene.RT_Point_Light;
using UnityEngine;

namespace _Project.Ray_Tracer.Scripts.RT_Scene
{
    /// <summary>
    /// A simple class that stores all components of a ray tracer scene. A ray tracer scene consists of an
    /// <see cref="RTCamera"/>, a list of <see cref="RTPointLight"/>s,  <see cref="RTAreaLight"/>s and a list of <see cref="RTMesh"/>es. All of these
    /// objects refer to "real" objects in the Unity scene. This class simply collects those references for easy
    /// access.
    /// </summary>
    public class RTScene
    {
        public delegate void SceneChanged();
        /// <summary>
        /// An event invoked whenever a object in this scene is changed.
        /// </summary>
        public event SceneChanged OnSceneChanged;

        private RTCamera camera;
        /// <summary>
        /// This ray tracer scene's camera. There can only be one camera in a scene.
        /// </summary>
        public RTCamera Camera
        {
            get { return camera; }
            set
            {
                if (camera == value) return;

                if (camera != null)
                    camera.OnCameraChanged -= SceneObjectChanged;

                camera = value;
                camera.OnCameraChanged += SceneObjectChanged;
                OnSceneChanged?.Invoke();
            }
        }

        private bool enablePointLights = true;
        public bool EnablePointLights
        {
            get { return enablePointLights; }
            set
            {
                if (value == enablePointLights) return;
                enablePointLights = value;

                foreach (RTPointLight pointLight in pointLights)
                    pointLight.gameObject.SetActive(value);

                OnSceneChanged?.Invoke();
            }
        }

        private bool enableAreaLights = true;
        public bool EnableAreaLights
        {
            get { return enableAreaLights; }
            set
            {
                if (value == enableAreaLights) return;
                enableAreaLights = value;

                foreach (RTAreaLight areaLight in areaLights)
                    areaLight.gameObject.SetActive(value);

                OnSceneChanged?.Invoke();
            }
        }

        private List<RTPointLight> pointLights;
        /// <summary>
        /// This ray tracers scene's list of lights.
        /// </summary>        
        public List<RTPointLight> PointLights
        {
            get
            {
                if (EnablePointLights) return pointLights;
                return new List<RTPointLight>(0);
            }
            private set => pointLights = value;
        }

        private List<RTAreaLight> areaLights;
        /// <summary>
        /// This ray tracers scene's list of area lights.
        /// </summary>
        public List<RTAreaLight> AreaLights
        {
            get
            {
                if (EnableAreaLights) return areaLights;
                return new List<RTAreaLight>(0);
            }
            private set => areaLights = value;
        }

        /// <summary>
        /// This ray tracers scene's list of meshes.
        /// </summary>
        public List<RTMesh> Meshes { get; }

        public RTScene(RTCamera camera) : this(camera, new List<RTPointLight>(), new List<RTAreaLight>(), new List<RTMesh>()) { }

        public RTScene(RTCamera camera, List<RTPointLight> lights, List<RTAreaLight> arealights, List<RTMesh> meshes)
        {
            Camera = camera;
            
            PointLights = lights;
            foreach (var light in lights)
                light.OnLightChanged += SceneObjectChanged;

            AreaLights = arealights;
            foreach (var arealight in arealights)
                arealight.OnLightChanged += SceneObjectChanged;

            Meshes = meshes;
            foreach (var mesh in meshes)
                mesh.OnMeshChanged += SceneObjectChanged;
        }

        /// <summary>
        /// Add a light to this scene.
        /// </summary>
        /// <param name="light"> The <see cref="RTLight"/> object to add. </param>
        public void AddLight(RTLight light)
        {
            if (light.Type == RTLight.RTLightType.Point)
                PointLights.Add(light as RTPointLight);
            else if (light.Type == RTLight.RTLightType.Area)
                AreaLights.Add(light as RTAreaLight);
            else
                return;

            light.OnLightChanged += SceneObjectChanged;
            OnSceneChanged?.Invoke();
        }

        /// <summary>
        /// Remove a light from this scene.
        /// </summary>
        /// <param name="light"> The <see cref="RTLight"/> object to remove. </param>
        public void RemoveLight(RTLight light)
        {
            if (light.Type == RTLight.RTLightType.Point)
                PointLights.Remove(light as RTPointLight);
            else if (light.Type == RTLight.RTLightType.Area)
                AreaLights.Remove(light as RTAreaLight);
            else
                return;

            light.OnLightChanged -= SceneObjectChanged;

            OnSceneChanged?.Invoke();
        }

        /// <summary>
        /// Add a mesh to this scene.
        /// </summary>
        /// <param name="mesh"> The <see cref="RTMesh"/> object to add. </param>
        public void AddMesh(RTMesh mesh)
        {
            Meshes.Add(mesh);
            mesh.OnMeshChanged += SceneObjectChanged;
            OnSceneChanged?.Invoke();
        }

        /// <summary>
        /// Remove a mesh from this scene.
        /// </summary>
        /// <param name="mesh"> The <see cref="RTMesh"/> object to remove. </param>
        public void RemoveMesh(RTMesh mesh)
        {
            Meshes.Remove(mesh);
            mesh.OnMeshChanged -= SceneObjectChanged;
            OnSceneChanged?.Invoke();
        }
        
        /// <summary>
        /// Destroy all game objects associated with this scene.
        /// </summary>
        public void Destroy()
        {
            if (camera != null)
            {
                camera.OnCameraChanged -= SceneObjectChanged;
                Object.Destroy(camera.gameObject);
            }
            camera = null;

            foreach (var light in PointLights)
            {
                if (light != null)
                {
                    light.OnLightChanged -= SceneObjectChanged;
                    Object.Destroy(light.gameObject);
                }
            }
            PointLights.Clear();

            foreach (var mesh in Meshes)
            {
                if (mesh != null)
                {
                    mesh.OnMeshChanged -= SceneObjectChanged;
                    Object.Destroy(mesh.gameObject);
                }
            }
            Meshes.Clear();
        }

        private void SceneObjectChanged()
        {
            OnSceneChanged?.Invoke();
        }
    }
}
