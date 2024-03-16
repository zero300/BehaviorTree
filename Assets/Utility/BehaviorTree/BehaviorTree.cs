using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Behavior
{
    [CreateAssetMenu()]
    public class BehaviorTree : ScriptableObject
    {
        public BTNode root;
        public BTNodeState treeState = BTNodeState.Running;
        public List<BTNode> nodes = new List<BTNode>();
        public BehaviorTree() { }
        public BTNodeState Update()
        {
            if (treeState == BTNodeState.Running)
            {
                treeState = root.Update();
            }
            return treeState;
        }
        public BehaviorTree Clone()
        {
            BehaviorTree newTree = Instantiate(this);
            newTree.root = root.Clone();
            newTree.nodes = new List<BTNode>();
            Traverse(newTree.root, n => newTree.nodes.Add(n));

            return newTree;
        }
        /// <summary>
        /// 做添加node 到 tree 的 nodes中
        /// </summary>
        /// <param name="btNode">當前node</param>
        /// <param name="visitor">添加的Action</param>
        private void Traverse(BTNode btNode, Action<BTNode> visitor)
        {
            if (btNode)
            {
                visitor(btNode);
                var children = GetChildren(btNode);
                children.ForEach(child => Traverse(child, visitor) );
            }
        }
        public List<BTNode> GetChildren(BTNode btNode)
        {
            if (btNode is CompositeNode compositeNode) return compositeNode.children;

            List<BTNode> list = new List<BTNode>();
            if (btNode is RootNode rootNode && rootNode.child != null) list.Add(rootNode.child);
            if (btNode is DecoratorNode decoratorNode && decoratorNode.child != null) list.Add(decoratorNode.child);

            return list;
        }
#if UNITY_EDITOR
        public BTNode CreateNode(Type type) {
            var newNode = ScriptableObject.CreateInstance(type) as BTNode;
            newNode.name = type.Name;
            newNode.guid = GUID.Generate().ToString();
            Undo.RecordObject(this, "Behavior Tree (CreateNode)");
            nodes.Add(newNode);

            if (!Application.isPlaying) AssetDatabase.AddObjectToAsset(newNode, this);

            Undo.RegisterCreatedObjectUndo(newNode, "Behavior Tree (CreateNode)");
            AssetDatabase.SaveAssets();
            return newNode;
        }
        public void RemoveNode(BTNode node)
        {
            Undo.RecordObject(this, "Behavior Tree (RemoveNode)");
            nodes.Remove(node);

            
            // AssetDatabase.RemoveObjectFromAsset(node);
            Undo.DestroyObjectImmediate(node);
            AssetDatabase.SaveAssets();
        }
        public void AddChild(BTNode parent, BTNode child) {
            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behavior Tree (Add Child)");
                rootNode.child = child;
                EditorUtility.SetDirty(rootNode);
            }
            if (parent is DecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behavior Tree (Add Child)");
                decoratorNode.child = child;
                EditorUtility.SetDirty(decoratorNode);
            }
            if (parent is CompositeNode compositeNode)
            {
                Undo.RecordObject(compositeNode, "Behavior Tree (Add Child)");
                compositeNode.children.Add(child);
                EditorUtility.SetDirty(compositeNode);
            }
        }
        public void RemoveChild(BTNode parent, BTNode child)
        {
            if (parent is RootNode rootNode)
            {
                Undo.RecordObject(rootNode, "Behavior Tree (Remove Child)");
                rootNode.child = null;
                //EditorUtility.SetDirty(rootNode);
            }
            if (parent is DecoratorNode decoratorNode)
            {
                Undo.RecordObject(decoratorNode, "Behavior Tree (Remove Child)");
                decoratorNode.child = null;
                //EditorUtility.SetDirty(decoratorNode);
            }
            if (parent is CompositeNode compositeNode)
            {
                Undo.RecordObject(compositeNode, "Behavior Tree (Remove Child)");
                compositeNode.children.Remove(child);
                //EditorUtility.SetDirty(compositeNode);
            }
        }
        #endif
    }

}
