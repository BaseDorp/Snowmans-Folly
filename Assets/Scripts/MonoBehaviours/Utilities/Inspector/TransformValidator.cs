using UnityEngine;
using System.Collections.Generic;

#region Transform Change Interfaces
/// <summary>
/// Class exposes OnValidate an requests that it be validated when transform values change.
/// </summary>
public interface IValidateOnTransformChange
{
    void OnValidate();
}
#endregion

[ExecuteInEditMode]
public sealed class TransformValidator : MonoBehaviour
{
    #region Private Fields
    // Store the prior transform state to check against.
    private TransformMatrixNode priorMatrixState;
    private sealed class TransformMatrixNode
    {
        public Matrix4x4 worldToLocalMatrix;
        public List<TransformMatrixNode> children;
        public TransformMatrixNode(Matrix4x4 worldToLocalMatrix)
        {
            this.worldToLocalMatrix = worldToLocalMatrix;
            children = new List<TransformMatrixNode>();
        }
    }
    #endregion
    #region Inspector Fields
    [Tooltip("Enables the functionality to check on each editor frame to validate.")]
    [SerializeField] private bool liveUpdate = false;
    [Tooltip("Extends functionality to check for transform changes in children.")]
    [SerializeField] private bool checkChildren = false;
    #endregion
#if DEBUG
    #region Editor Initialization
    private void Awake()
    {
        priorMatrixState = new TransformMatrixNode(transform.worldToLocalMatrix);
    }
    #endregion
    #region Editor Update Loop
    private void Update()
    {
        // Ensure that the editor state has been initialized.
        if (priorMatrixState == null)
            Awake();
        if (liveUpdate)
        {
            bool requiresValidation = false;
            // Check if the transform matrix has changed.
            if (!transform.worldToLocalMatrix.Equals(priorMatrixState.worldToLocalMatrix))
            {
                priorMatrixState.worldToLocalMatrix = transform.worldToLocalMatrix;
                requiresValidation = true;
            }
            // Check children if user enabled option.
            if (checkChildren)
            {
                // Traverse the hierarchy looking for changes in children.
                if (FindChangesRecursive(priorMatrixState, transform))
                    requiresValidation = true;
            }
            if (requiresValidation)
                Validate();
        }
        bool FindChangesRecursive(TransformMatrixNode rootState, Transform rootTransform)
        {
            // If there is a different child count, reconstruct
            // the state hierarchy and request a validation.
            if (rootTransform.childCount != rootState.children.Count)
            {
                rootState.children = new List<TransformMatrixNode>();
                foreach (Transform child in rootTransform)
                {
                    TransformMatrixNode newNode =
                        new TransformMatrixNode(rootTransform.worldToLocalMatrix);
                    rootState.children.Add(newNode);
                    FindChangesRecursive(newNode, child);
                }
                return true;
            }
            else
            {
                // Check all children to the stored state and request
                // validation if a change is found.
                bool foundChange = false;
                for (int i = 0; i < rootTransform.childCount; i++)
                {
                    if (rootTransform.GetChild(i).worldToLocalMatrix
                        != rootState.children[i].worldToLocalMatrix)
                    {
                        rootState.children[i].worldToLocalMatrix =
                            rootTransform.GetChild(i).worldToLocalMatrix;
                        foundChange = true;
                    }
                    // Bubble up changes found in recursive calls.
                    if (FindChangesRecursive(rootState.children[i], rootTransform.GetChild(i)))
                        foundChange = true;
                }
                return foundChange;
            }
        }
    }
    private void Validate()
    {
        // Notify all scripts with editor features dependent
        // on the transform.
        foreach (IValidateOnTransformChange editorBehavior in
            gameObject.GetComponents<IValidateOnTransformChange>())
        {
            editorBehavior.OnValidate();
        }
    }
    #endregion
#endif
}
