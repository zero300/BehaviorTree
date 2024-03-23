using Behavior;
using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviorTreeEditor : EditorWindow
{
    BehaviorTreeView treeView;
    InspectorView inspectorView;
    IMGUIContainer blackboardview;

    SerializedObject treeObject;
    SerializedProperty blackBoardProp;

    [MenuItem("DataStructure/BehaviorTreeEditor")]
    public static void OpenWindow()
    {
        BehaviorTreeEditor wnd = GetWindow<BehaviorTreeEditor>();
        wnd.titleContent = new GUIContent("BehaviorTreeEditor");
    }
    [OnOpenAsset]
    public static bool OnOpenAsset(int instanceId, int line)
    {
        if (Selection.activeObject is BehaviorTree)
        {
            OpenWindow();
            return true;
        }
        return false;
    }
    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualTreeAsset m_VisualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Editor/BehaviorTree/BehaviorTreeEditor.uxml");
        //TemplateContainer container = m_VisualTreeAsset.Instantiate();
        //root.Add(container);
        m_VisualTreeAsset.CloneTree(root);

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviorTree/BehaviorTreeEditor.uss");
        root.styleSheets.Add(styleSheet);

        treeView = root.Q<BehaviorTreeView>();
        inspectorView = root.Q<InspectorView>();
        blackboardview = root.Q<IMGUIContainer>();
        blackboardview.onGUIHandler = () =>
        {
            if (treeObject is null) return ;
            treeObject.Update();
            EditorGUILayout.PropertyField(blackBoardProp);
            treeObject.ApplyModifiedProperties();
        };

        treeView.OnNodeSelected = OnNodeSelectionChanged;
        OnSelectionChange();
    }
    private void OnEnable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }
    private void OnDisable()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
    }
    private void OnPlayModeStateChanged(PlayModeStateChange change)
    {
        switch (change)
        {
            case PlayModeStateChange.EnteredEditMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingEditMode:
                break;
            case PlayModeStateChange.EnteredPlayMode:
                OnSelectionChange();
                break;
            case PlayModeStateChange.ExitingPlayMode:
                break;
        }
    }
   
    private void OnSelectionChange()
    {
        // ��OnEnabled �� OnDisabled ��  
        // �����D��ԣŪ�������treeView
        if (treeView is null) {
            if (treeView is null) Debug.Log("Tree View is Null"); 
            return; 
        }

        BehaviorTree tree = Selection.activeObject as BehaviorTree;

        if (!tree)
        {
            // ��оǼv���ϥ� active Object
            // ���ڳo��|�����~ Not Supported Error  ( GETCOMPONENT ) 
            // �ҥH�o���� active GameObject
            if (Selection.activeGameObject)
            {
                BehaviorRunner runner = Selection.activeGameObject.GetComponent<BehaviorRunner>();
                if (runner) tree = runner.tree;
            }
        }

        if (Application.isPlaying)
        {
            
            if (tree) treeView.PopulateTree(tree);
        }
        else
        {
            if (tree && AssetDatabase.CanOpenAssetInEditor(tree.GetInstanceID())) treeView.PopulateTree(tree);
        }

        if(tree != null)
        {
            treeObject = new SerializedObject(tree);
            blackBoardProp = treeObject.FindProperty("blackboard");
        }
    }
    void OnNodeSelectionChanged(NodeView nodeView) {
        inspectorView.UpdateSelection(nodeView);
    }
    private void OnInspectorUpdate()
    {
        treeView?.UpdateNodeStates();
    }
}
