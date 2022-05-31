using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    private BehaviourTree _tree;
    public BehaviourTreeView()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);
        gridBackground.BringToFront();


        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/BehaviourTreeEditor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);

    }

    internal void PopulateView(BehaviourTree tree)
    {
        _tree = tree;

        DeleteElements(graphElements);

        tree.nodes.ForEach(node => CreateNodeView(node));
    }

    private void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }

}
