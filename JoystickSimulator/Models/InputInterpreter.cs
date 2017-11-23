using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace JoystickSimulator.Models
{
    /// <summary>
    /// Classe permettant d'interpreter les inputs
    /// </summary>
    public class InputInterpreter
    {
        public InputInterpreter() { }

        public InputAction GetAction(Dictionary<JoystickOffset, double> inputs) {
            if (inputs.Count == 0) //Pas de boutons pressés --> On retourne l'action par défaut
            {
                return ActionList.GetActionByName("MoveNeutralPoint");
            }

            //On trouve la bonne action (normalement il n'en retournera qu'une)
            IEnumerable<InputAction> results = ActionList.List.Where(action => 
                new HashSet<JoystickOffset>(inputs.Keys.ToList()).SetEquals(
                    new HashSet<JoystickOffset>(action.ButtonNeeded)) &&
                inputs.Values.ToList().Min() >= action.TimeNeeded);

            //Si on a un résultat, on le renvoie sinon, action par défaut
            return results.Any() ? results.First() : ActionList.GetActionByName("MoveNeutralPoint");
        }
    }
    public class InputAction
    {
        public double TimeNeeded { get; set; }
        public List<JoystickOffset> ButtonNeeded { get; set; }
        public string Name { get; set; }

        public InputAction(double timeNeeded, string name, params JoystickOffset[] buttonsNeeded)
        {
            TimeNeeded = timeNeeded;
            Name = name;
            ButtonNeeded = buttonsNeeded.ToList();
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Liste des actions possibles avec le Joystick
    /// </summary>
    public static class ActionList
    {
        public static List<InputAction> List { get; private set; } = new List<InputAction> {
            new InputAction(100,"SwitchSimulatorState",JoystickOffset.Buttons4,JoystickOffset.Buttons10, JoystickOffset.Buttons7),
            new InputAction(0,"MoveSimulator",JoystickOffset.Buttons1),
            new InputAction(0,"MoveRotationPoint",JoystickOffset.Buttons0),
            new InputAction(0, "MoveNeutralPoint")
        };

        public static InputAction GetActionByName(string name)
        {
            var ctnr = List.Single(action => action.Name.ToLower().Equals(name.ToLower()));
            return ctnr;
        }
    }
}