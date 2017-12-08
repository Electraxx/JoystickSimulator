using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace JoystickSimulator.Models
{
    /// <summary>
    /// Classe permettant de transformer les entrées joystick/boutons en actions
    /// </summary>
    public class InputInterpreter
    {
        public InputInterpreter() { }

        /// <summary>
        /// transforme les inputs, renvoie l'action correspondante
        /// </summary>
        /// <param name="inputs">Entrées [bouton, timestamp originel]</param>
        /// <returns>L'action correspondante</returns>
        public InputAction GetAction(Dictionary<JoystickOffset, double> inputs)
        {
            if (inputs.Count == 0) //Pas de boutons pressés --> On retourne l'action par défaut
                return ActionList.GetActionByName("MoveNeutralPoint");

            //On trouve la bonne action (normalement il n'en retournera qu'une)
            //Requête Linq comparant 2 hashset pour regarder à quel action il correspond, on vérifie aussi le temps minimal de la pression du bouton
            IEnumerable<InputAction> results = ActionList.List.Where(action =>
                new HashSet<JoystickOffset>(inputs.Keys.ToList()).SetEquals(
                    new HashSet<JoystickOffset>(action.ButtonNeeded)) &&
                inputs.Values.ToList().Min() >= action.TimeNeeded);
           
            //Si on a un résultat, on le renvoie sinon, action par défaut
            return results.Any() ? results.First() : ActionList.GetActionByName("MoveNeutralPoint");
        }
    }

    /// <summary>
    /// Représente une action possible sur le simulateur
    /// </summary>
    public class InputAction
    {
        /// <summary>
        /// Temps minimal pour l'action
        /// </summary>
        public double TimeNeeded { get; set; }
        /// <summary>
        /// Boutons demandés pour l'action
        /// </summary>
        public List<JoystickOffset> ButtonNeeded { get; set; }
        /// <summary>
        /// Nom de l'action
        /// </summary>
        public string Name { get; set; }

        public InputAction(double timeNeeded, string name, params JoystickOffset[] buttonsNeeded)
        {
            //On regarde si c'est null à cause de la désérialisation
            ButtonNeeded = buttonsNeeded != null ? buttonsNeeded.ToList() : new List<JoystickOffset>();
            TimeNeeded = timeNeeded;
            Name = name;
        }

        public override string ToString() => Name;
    }

    /// <summary>
    /// Liste des actions possibles avec le Joystick, comparable à une énumération en Java
    /// </summary>
    public static class ActionList
    {
        /// <summary>
        /// Liste des actions possibles
        /// </summary>
        public static List<InputAction> List { get; private set; } = new List<InputAction> {
            new InputAction(100,"SwitchSimulatorState",JoystickOffset.Buttons0,JoystickOffset.Buttons10, JoystickOffset.Buttons7),
            new InputAction(0,"MoveSimulator",JoystickOffset.Buttons1),
            new InputAction(0,"MoveRotationPoint",JoystickOffset.Buttons0),
            new InputAction(0, "MoveNeutralPoint")
        };

        /// <summary>
        /// Renvoie l'action avec le nom correspondant
        /// </summary>
        /// <param name="name">Nom de l'action voulue</param>
        /// <returns>L'action désirée</returns>
        public static InputAction GetActionByName(string name) //TODO default action/exception
        {
            var ctnr = List.Single(action => action.Name.ToLower().Equals(name.ToLower()));
            return ctnr;
        }
    }
}