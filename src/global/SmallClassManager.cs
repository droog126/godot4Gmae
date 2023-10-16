using Godot;
using System;
using System.Collections.Generic;
using GlobalSpace;
using System.Reflection;

public partial class SmallClassManager : Godot.Node
{
    private List<Action> stepActions = new List<Action>();

    public override void _Ready()
    {


        string[] classNames = { "InputControl", "Test" };
        foreach (string className in classNames)
        {
            Type type = Type.GetType(className);
            if (type != null)
            {
                object classInstance = Activator.CreateInstance(type);



                // 中文注释不能打吗？
                MethodInfo stepMethod = type.GetMethod("step");

                if (stepMethod != null)
                {
                    Action stepAction = (Action)Delegate.CreateDelegate(typeof(Action), classInstance, stepMethod);
                    stepActions.Add(stepAction);

                }
                Global.GlobalSingleClass[className] = classInstance;
            }
        }


    }

    public override void _Process(double delta)
    {


        foreach (var stepAction in stepActions)
        {
            stepAction();
        }
    }
}



public enum InputMode { Menu, Game }

public partial class InputControl
{
    public InputMode inputMode = 0;

    public void step()
    {

        if (Input.IsActionJustPressed("f4"))
        {
            inputMode = (InputMode)(((int)inputMode + 1) % Enum.GetValues(typeof(InputMode)).Length);

        }
    }
}


public partial class Test
{


    private InputMode inputMode = 0;

    public Test()
    {


        GD.Print("global", Global.GlobalVariable);



    }

    public void step()
    {

        var inputControl = Global.GetSingleClass<InputControl>("InputControl");

        // GD.Print("here", inputControl.inputMode);



    }
}




