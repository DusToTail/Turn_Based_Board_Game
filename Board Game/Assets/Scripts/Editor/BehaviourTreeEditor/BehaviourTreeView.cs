using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;

public class BehaviourTreeView : GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }
    private BehaviourTree _tree;
    public BehaviourTreeView()
    {
        GridBackground gridBackground = new GridBackground();
        gridBackground.StretchToParentSize();
        Insert(0, gridBackground);

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

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        //base.BuildContextualMenu(evt);
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }

        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
            }
        }


    }

    private void CreateNode(System.Type type)
    {
        Node node = _tree.CreateNode(type);
        CreateNodeView(node);
    }

    private void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }

}
