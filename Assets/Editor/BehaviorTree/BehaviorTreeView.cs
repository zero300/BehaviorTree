using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using System;
using Behavior;
using UnityEngine;

public class BehaviorTreeView : GraphView
{
    public Action<NodeView> OnNodeSelected;
    public new class UxmlFactory : UxmlFactory<BehaviorTreeView, GraphView.UxmlTraits>{ };
    BehaviorTree tree;

    public BehaviorTreeView()
    {
        Insert(0, new GridBackground());

        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());


        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTree/BehaviorTreeEditor.uss");
        styleSheets.Add(styleSheet);

        focusable = true;
        this.Focus();

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    private void OnUndoRedo()
    {
        PopulateTree(tree);

        // 需要先存嗎?
        // AssetDatabase.SaveAssets();
    }

    internal void PopulateTree(BehaviorTree tree)
    {
        this.tree = tree;

        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;

        if (tree.root is null)
        {
            tree.root = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }

        this.tree.nodes.ForEach(n => CreateNodeView(n));

        this.tree.nodes.ForEach(n =>
        {
            var children = tree.GetChildren(n);
            children.ForEach(child =>
            {
                NodeView parentView = GetNodeViewByNode(n);
                NodeView childView = GetNodeViewByNode(child);

                Edge edge =  parentView.output.ConnectTo(childView.input);
                AddElement(edge);
            });
        });
    }
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        //return base.GetCompatiblePorts(startPort, nodeAdapter);
        return ports.ToList()
            .Where(endpoints => endpoints.direction != startPort.direction && endpoints.node != startPort.node)
            .ToList();
    }
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        if (graphViewChange.elementsToRemove != null){
            // 這版本通過Clone 的方式 好像會有鍵盤按鍵讀取不到的Bugˊˇˋ
            graphViewChange.elementsToRemove.ForEach( item => {
                if (item is NodeView nodeView) tree.RemoveNode (nodeView.node);

                if (item is Edge edge){
                    Debug.Log("Delete Edge");
                    NodeView parentNodeView = edge.output.node as NodeView;
                    NodeView childNodeView = edge.input.node as NodeView;
                    tree.RemoveChild(parentNodeView.node, childNodeView.node);
                }
                //if (item is Edge edge) tree.RemoveNode(edge.input, edge.output);
            });
        }

        if (graphViewChange.edgesToCreate != null)
        {
            // 這版本通過Clone 的方式 好像會有鍵盤按鍵讀取不到的Bugˊˇˋ
            graphViewChange.edgesToCreate.ForEach(item => {
                if (item is Edge edge)
                {
                    NodeView parentNodeView = edge.output.node as NodeView;
                    NodeView childNodeView = edge.input.node as NodeView;
                    tree.AddChild(parentNodeView.node, childNodeView.node);
                } 
                //if (item is Edge edge) tree.RemoveNode(edge.input, edge.output);
            });
        }
        return graphViewChange;
    }

    /// <summary>
    /// Create Menu Item
    /// </summary>
    /// <param name="evt"></param>
    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        evt.menu.AppendAction("Delete", a => DeleteSomething());
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType}:{type.Name}", a => CreateNewNode(type));
            }
        }
        evt.menu.AppendSeparator();
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType}:{type.Name}", a => CreateNewNode(type));
            }
        }
        evt.menu.AppendSeparator();
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"{type.BaseType}:{type.Name}", a => CreateNewNode(type));
            }
        }
    }

    /// <summary>
    /// 通過DELETE按鈕進行刪除無反應
    /// 故這邊通過建立選單來做刪除動作
    /// </summary>
    private void DeleteSomething()
    {
        var selections = selection.ToArray();
        if(selections.Length != 0)
        {
            
            for (int i = 0; i < selections.Length; i++)
            {
                if(selections[i] is NodeView nodeView)
                {
                    if (nodeView.node is RootNode) continue;

                    RemoveElement(nodeView);
                    tree.RemoveNode(nodeView.node);
                    continue;
                }

                // 通過這邊刪除 Port 的孔 會顯示還有東西，
                // 但實際上有刪掉
                if (selections[i] is Edge edge)
                {
                    RemoveElement(edge);

                    edge.output.Disconnect(edge);
                    edge.input.Disconnect(edge);
                    NodeView parentNodeView = edge.output.node as NodeView;
                    NodeView childNodeView = edge.input.node as NodeView;
                    tree.RemoveChild(parentNodeView.node, childNodeView.node);
                    continue;
                }
            }
        }
        
    }

    /// <summary>
    /// 通過Type 建立節點
    /// </summary>
    /// <param name="type"></param>
    void CreateNewNode(Type type)
    {
        BTNode node = tree.CreateNode(type);
        CreateNodeView(node);
    }
    void CreateNodeView(BTNode node)
    {
        NodeView nodeView = new NodeView(node);
        nodeView.OnNodeSelected = OnNodeSelected;
        AddElement(nodeView);
    }
    private NodeView GetNodeViewByNode(BTNode btNode)
    {
        return GetNodeByGuid(btNode.guid) as NodeView;
    }
}
