using System.Collections.Generic;
using UnityEngine;

namespace RuntimeHandle
{
    /**
     * Created by Peter @sHTiF Stefcek 20.10.2020
     */
    public class PositionHandle : MonoBehaviour
    {
        protected RuntimeTransformHandle _parentTransformHandle;
        protected List<PositionAxis> _axes;
        protected List<PositionPlane> _planes;

        public PositionHandle Initialize(RuntimeTransformHandle p_runtimeHandle)
        {
            _parentTransformHandle = p_runtimeHandle;
            transform.SetParent(_parentTransformHandle.transform, false);

            _axes = new List<PositionAxis>();

            if (_parentTransformHandle.axes == HandleAxes.X || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<PositionAxis>()
                    .Initialize(_parentTransformHandle, Vector3.right, -Vector3.forward, Color.red));
            
            if (_parentTransformHandle.axes == HandleAxes.Y || _parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<PositionAxis>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.forward, Color.green));

            if (_parentTransformHandle.axes == HandleAxes.Z || _parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _axes.Add(new GameObject().AddComponent<PositionAxis>()
                    .Initialize(_parentTransformHandle, Vector3.forward, Vector3.right, Color.blue));

            _planes = new List<PositionPlane>();
            
            if (_parentTransformHandle.axes == HandleAxes.XY || _parentTransformHandle.axes == HandleAxes.XYZ)
                _planes.Add(new GameObject().AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.up, -Vector3.forward, new Color(0,0,1,.2f)));

            if (_parentTransformHandle.axes == HandleAxes.YZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _planes.Add(new GameObject().AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.up, Vector3.forward, Vector3.right, new Color(1, 0, 0, .2f)));

            if (_parentTransformHandle.axes == HandleAxes.XZ || _parentTransformHandle.axes == HandleAxes.XYZ)
                _planes.Add(new GameObject().AddComponent<PositionPlane>()
                    .Initialize(_parentTransformHandle, Vector3.right, Vector3.forward, Vector3.up, new Color(0, 1, 0, .2f)));

            // The handles are rendered by a separate camera so that they are always rendered above other objects. To
            // facilitate this we put them in a render separate layer.
            int gizmoLayer = LayerMask.NameToLayer("Gizmos");
            foreach (Transform transform in GetComponentsInChildren<Transform>(true))
                transform.gameObject.layer = gizmoLayer;

            return this;
        }

        public void Destroy()
        {
            foreach (PositionAxis axis in _axes)
                Destroy(axis.gameObject);
            
            foreach (PositionPlane plane in _planes)
                Destroy(plane.gameObject);
            
            Destroy(this);
        }
    }
}