using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class RotationHandle : MonoBehaviour
    {
        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<RotationAxis> _axes;

        public RotationHandle Initialize(RuntimeTransformHandle p_parentTransformHandle)
        {
            _parentTransformHandle = p_parentTransformHandle;
            transform.SetParent(_parentTransformHandle.transform, false);

            _axes = new List<RotationAxis>();
            
            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.up, Color.red));
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.right, Color.green));

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<RotationAxis>()
                    .Initialize(_parentTransformHandle, Vector3.forward, Vector3.up, Color.blue));

            // The handles are rendered by a separate camera so that they are always rendered above other objects. To
            // facilitate this we put them in a render separate layer.
            int gizmoLayer = LayerMask.NameToLayer("Gizmos");
            foreach (Transform transform in GetComponentsInChildren<Transform>(true))
                transform.gameObject.layer = gizmoLayer;

            return this;
        }

        public void Destroy()
        {
            foreach (RotationAxis axis in _axes)
                Destroy(axis.gameObject);
            
            Destroy(this);
        }
    }
}