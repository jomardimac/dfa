﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomataEngine;

namespace AutomataApp {
    class App {
        static void Main(string[] args) {

            AutomataGraph automata = null;



            Console.WriteLine("Finite Automata Demo");
            Console.WriteLine("Type 'load' to load sample automata, or type 'start' to create automata");
            string selection = null;
            while (true) {
                Console.Write(":> ");
                selection = Console.ReadLine();
                if (selection == "load") {
                    Console.WriteLine();
                    break;
                }
                if (selection == "start") {
                    Console.WriteLine();
                    break;
                }
            }

            if (selection == "load") {
                Console.WriteLine("Loading demo.xml");
                automata = new AutomataGraph("../../../demo.xml");
            }
            else if (selection == "start") {
                automata = new AutomataGraph();
                Console.WriteLine("Creating new Deterministic Finite Automata");
                Console.WriteLine("Type 'done' to finish building");
                Console.WriteLine("Type 'add state' to create new state");
                Console.WriteLine("Type 'add path' to create new path between states");
                while (true) {
                    Console.Write(":> ");
                    string sel = Console.ReadLine();
                    if (sel == "done") {

                        foreach (char c in automata.States.Keys) {
                            Console.WriteLine(c);
                        }

                        break;
                    }
                    switch (sel) {
                        case "add state":
                            Console.WriteLine("Enter a letter for state: ( ex. 'A', 'B' ...)");
                            Console.Write(":> ");
                            char statename = Convert.ToChar(Console.ReadLine());
                            Console.WriteLine("Enter the state type: ('Starting', 'Transition' or 'Accepting'");
                            Console.Write(":> ");
                            string type = Console.ReadLine();
                            Console.WriteLine(automata.AddState(statename, type));
                            break;
                        case "add path":
                            Console.WriteLine("Enter the starting state for the path: ( ex. 'A', 'B' ...)");
                            Console.Write(":> ");
                            char pathstart = Convert.ToChar(Console.ReadLine());
                            Console.WriteLine("Enter the stopping state for the path: ( ex. 'C', 'D' ...)");
                            Console.Write(":> ");
                            char pathstop = Convert.ToChar(Console.ReadLine());
                            Console.WriteLine("Enter the path weight: ('1' or '0')");
                            Console.Write(":> ");
                            char pathweight = Convert.ToChar(Console.ReadLine());

                            Console.WriteLine(automata.AddPath(pathstart, pathweight, pathstop));
    
                            break;
                    }
                }

            }

            // Hardcoded Example:

            // XML contains states, and a string

            //EX:
            //State A:
            //    Starting
            //    Path: 1, B
            //    Path: 0, C

            //State B:
            //    Transition
            //    Path: 0, A
            //    Path: 1, C

            //State C:
            //    Accepting

            //        Open XML file, connect states with each other




            
            Console.WriteLine("\nType 'q' to quit");
            Console.WriteLine("Type 'save' to save");
            while (true) {
                Console.Write("Enter a string to check in DFA: ");
                string input = Console.ReadLine();

                if (input == "q") {
                    break;
                }
                if (input == "save") {
                    automata.Save("thiscanbechanged");
                    Console.WriteLine("FA Saved.");
                    continue;
                }

                // 1011 works
                Console.WriteLine(automata.TestString(input));

            }
        }
    }
}


////            States = new Dictionary<char, State>();

//            ////////////////////////////////////////
//            if (!States.ContainsKey('A')) {
//                States['A'] = new State();
//            }
//            States['A'].Type = "Starting";
//            States['A'].Paths[0] ='C';
//            States['A'].Paths[1] = 'B';
//            ////////////////////////////////////////
//            if (!States.ContainsKey('B')) {
//                States['B'] = new State();
//            }
//            States['B'].Type = "Transition";
//            States['B'].Paths[0] = 'A';
//            States['B'].Paths[1] = 'C';
//            ////////////////////////////////////////
//            if (!States.ContainsKey('C')) {
//                States['C'] = new State();
//            }
//            States['C'].Type = "Accepting";
//        }

