using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using Behavior;
using UnityEditor;
using UnityEditor.UIElements;

public class NodeView : Node
{
    public Action<NodeView> OnNodeSelected;
    public BTNode node;
    public Port input;
    public Port output;
    public NodeView (BTNode node) : base ("Assets/Editor/BehaviorTree/NodeView.uxml")
    {
        this.node = node;
        this.title = node.name;
        this.viewDataKey = node.guid;

        this.style.left = node.position.x;
        this.style.top = node.position.y;
        
        CreateInputPort();
        CreateOutputPort();
        SetUpClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node) );
    }

    private void SetUpClasses()
    {
        switch (node)
        {
            case RootNode:
                AddToClassList("root-node");
                break;
            case ActionNode:
                AddToClassList("action-node");
                break;
            case DecoratorNode:
                AddToClassList("decorator-node");
                break;
            case CompositeNode:
                AddToClassList("composite-node");
                break;
        }
    }

    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        Undo.RecordObject(node, "Behavior Tree (SetPosition)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;

        EditorUtility.SetDirty(node);
    }

    public void CreateInputPort() {
        switch (node)
        {
            case RootNode:
                break;
            case ActionNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case DecoratorNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
        }

        if(input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }
    public void CreateOutputPort()
    {
        switch (node)
        {
            case RootNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool) );
                break;
            case ActionNode:
                break;
            case DecoratorNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
            case CompositeNode:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
    public void SortChildren()
    {
        if(node is CompositeNode compositeNode) compositeNode.children.Sort(SortByHorizontalPos);
    }
    private int SortByHorizontalPos(BTNode left, BTNode right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }
    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("success");
        RemoveFromClassList("failure");
        if (!Application.isPlaying) return;
        
        switch (node.state)
        {
            case BTNodeState.Running:
                if(node.started) AddToClassList("running");
                break;
            case BTNodeState.Success:
                AddToClassList("success");
                break;
            case BTNodeState.Failure:
                AddToClassList("failure");
                break;
        }
        
    }
}
