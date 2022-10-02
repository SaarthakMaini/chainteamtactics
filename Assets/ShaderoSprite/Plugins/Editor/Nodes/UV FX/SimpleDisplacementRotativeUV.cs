using UnityEngine;
using UnityEditor;
using System.Collections;
using _ShaderoShaderEditorFramework;
using _ShaderoShaderEditorFramework.Utilities;
namespace _ShaderoShaderEditorFramework
{
[Node(false, "UV/FX (UV)/Simple Displacement Rotative UV")]
public class SimpleDisplacementRotativeUV : Node
{
    [HideInInspector]
    public const string ID = "SimpleDisplacementRotativeUV";
    [HideInInspector]
    public override string GetID { get { return ID; } }
    [HideInInspector]
    public float Variable = 0.05f;
    [HideInInspector]
    public float Variable2 = 0.05f;
    public static int count = 1;
    public static bool tag = false;
    public static string code;

    [HideInInspector]
    public bool AddParameters = true;

    public static void Init()
    {
        tag = false;
        count = 1;
    }

    public void Function()
    {
            code = "";
            code += "float2 SimpleDisplacementRotativeUV(float2 uv, float4 rgba, float value, float value2)\n";
            code += "{\n";
            code += "float angle = value2 * 3.1415926;\n";
            code += "float dist = rgba.r;\n";
            code += "#define rot(n) mul(n, float2x2(cos(angle), -sin(angle), sin(angle), cos(angle)))\n";
            code += "float2 uv2 = uv+rot(float2(dist-0.5, dist-0.5));\n";
            code += "return lerp(uv, uv2, value);\n";
            code += "}\n";
    }


    public override Node Create(Vector2 pos)
    {
        Function();

            SimpleDisplacementRotativeUV node = ScriptableObject.CreateInstance<SimpleDisplacementRotativeUV>();

        node.name = "Simple Displacement Rotative UV";
        node.rect = new Rect(pos.x, pos.y, 172, 300);
        node.CreateInput("UV", "SuperFloat2");
        node.CreateInput("RGBA", "SuperFloat4");
        node.CreateOutput("UV", "SuperFloat2");

        return node;
    }

    protected internal override void NodeGUI()
    {
        Texture2D preview = ResourceManager.LoadTexture("Textures/previews/nid_simpledistortion.jpg");
        GUI.DrawTexture(new Rect(2, 0, 172, 46), preview);
        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        Inputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        Outputs[0].DisplayLayout(new GUIContent("UV", "UV"));
        GUILayout.EndHorizontal();
        Inputs[1].DisplayLayout(new GUIContent("RGBA", "RGBA"));

        if (GUILayout.Button("Reset"))
        {
                Variable = 0;
                Variable2 = 0;
        }

            if (NodeEditor._Shadero_Material != null)
            {
                NodeEditor._Shadero_Material.SetFloat(FinalVariable, Variable);
                NodeEditor._Shadero_Material.SetFloat(FinalVariable2, Variable2);
            }

            AddParameters = GUILayout.Toggle(AddParameters, "Add Parameters");

        GUILayout.Label("Value: (-0.3/0.3) " + Variable.ToString("0.00"));
        Variable =HorizontalSlider(Variable, -0.3f, 0.3f);
            GUILayout.Label("Angle: (-1/1) " + Variable.ToString("0.00"));
            Variable2 = HorizontalSlider(Variable2, -1f, 1f);


   

    }

        private string FinalVariable;
        private string FinalVariable2;
        [HideInInspector]
        public int MemoCount = -1;

    public override bool FixCalculate()
    {
        MemoCount = count;
        count++;
        return true;
    }

    public override bool Calculate()
    {
        tag = true;
        SuperFloat2 s_in = Inputs[0].GetValue<SuperFloat2>();
        SuperFloat4 s_in2 = Inputs[1].GetValue<SuperFloat4>();
        SuperFloat2 s_out = new SuperFloat2();

        string NodeCount = MemoCount.ToString();
        string DefaultName = "_Simple_Displacement_Rotative_" + NodeCount;
        string DefaultNameVariable1 = "_Simple_Displacement_Rotative_Value_" + NodeCount;
        string DefaultNameVariable2 = "_Simple_Displacement_Rotative_Rotation_" + NodeCount;
        string DefaultParameters1 = ", Range(-0.3, 0.3)) = " + Variable.ToString();
        string DefaultParameters2 = ", Range(-1, 1)) = " + Variable2.ToString();
        string uv = s_in.Result;
        string RGBA = s_in2.Result;
        string Source = "";

        FinalVariable = DefaultNameVariable1;
        FinalVariable2 = DefaultNameVariable2;



            if (s_in2.Result == null)
        {
            RGBA = "float4(0,0,0,1)";
        }
        else
        {
            RGBA = s_in2.Result;
        }


        // source
        if (s_in.Result == null)
        {
            uv = "i.texcoord";
            if (Source == "_MainTex") uv = "i.texcoord";
            if (Source == "_GrabTexture") uv = "i.screenuv";
        }
        else
        {
            uv = s_in.Result;
        }

        s_out.StringPreviewLines = s_in.StringPreviewNew + s_in2.StringPreviewNew + s_in2.StringPreviewNew;

        if (AddParameters)
        {
            s_out.ValueLine = "float2 " + DefaultName + " = SimpleDisplacementRotativeUV(" + uv + "," + RGBA + "," + DefaultNameVariable1 + "," + DefaultNameVariable2 + ");\n";
        }
        else
        {
            s_out.ValueLine = "float2 " + DefaultName + " = SimpleDisplacementRotativeUV(" + uv + "," + RGBA + "," + Variable.ToString() + "," + Variable2.ToString() + ");\n";

        }

        s_out.StringPreviewNew = s_out.StringPreviewLines + s_out.ValueLine;
        s_out.ParametersLines += s_in.ParametersLines + s_in2.ParametersLines + s_in2.ParametersLines;

        s_out.Result = DefaultName;
        s_out.ParametersDeclarationLines += s_in.ParametersDeclarationLines + s_in2.ParametersDeclarationLines + s_in2.ParametersDeclarationLines;

            if (AddParameters)
            {
                s_out.ParametersLines += DefaultNameVariable1 + "(\"" + DefaultNameVariable1 + "\"" + DefaultParameters1 + "\n";
                s_out.ParametersLines += DefaultNameVariable2 + "(\"" + DefaultNameVariable2 + "\"" + DefaultParameters2 + "\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable1 + ";\n";
                s_out.ParametersDeclarationLines += "float " + DefaultNameVariable2 + ";\n";
            }
            Outputs[0].SetValue<SuperFloat2>(s_out);

            count++;
        return true;
    }
}
}