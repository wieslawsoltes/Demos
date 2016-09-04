
#region Test1

static void Test1(bool s1_state, bool s2_state)
{
    string test_name = string.Format("--- test1 --- s1_state={0} --- s2_state={1} ---", s1_state, s2_state);

    System.Diagnostics.Debug.Print("");
    System.Diagnostics.Debug.Print(test_name);
    System.Diagnostics.Debug.Print("");

    System.Console.WriteLine("");
    System.Console.WriteLine(test_name);
    System.Console.WriteLine("");

    //
    //TEST1 MODEL
    //

    #region Test1 Model

    var context = new Context("context_test1");
    var e = context.Children;

    var p1_1 = new Pin("p1_1", PinType.Input); e.Add(p1_1); //P1_1
    var p1_2 = new Pin("p1_2", PinType.Output); e.Add(p1_2); //P1_2
    var s1 = new Signal("s1"); e.Add(s1); s1.Input = p1_1; s1.Output = p1_2; //S1
    s1.Parent = context; p1_1.Parent = s1; p1_2.Parent = s1;

    var p2_1 = new Pin("p2_1", PinType.Input); e.Add(p2_1); //P2_1
    var p2_2 = new Pin("p2_2", PinType.Output); e.Add(p2_2); //P2_2
    var s2 = new Signal("s2"); e.Add(s2); s2.Input = p2_1; s2.Output = p2_2; //S2
    s2.Parent = context; p2_1.Parent = s2; p2_2.Parent = s2;

    var p3_1 = new Pin("p3_1", PinType.Input); e.Add(p3_1); //P3_1
    var p3_2 = new Pin("p3_2", PinType.Output); e.Add(p3_2); //P3_2
    var s3 = new Signal("s3"); e.Add(s3); s3.Input = p3_1; s3.Output = p3_2; //S3
    s3.Parent = context; p3_1.Parent = s3; p3_2.Parent = s3;

    var pag5_1 = new Pin("pag5_1", PinType.Undefined); e.Add(pag5_1); //PAG5_1
    var pag5_2 = new Pin("pag5_2", PinType.Undefined); e.Add(pag5_2); //PAG5_2
    var pag5_3 = new Pin("pag5_3", PinType.Undefined); e.Add(pag5_3); //PAG5_3
    var pag5_4 = new Pin("pag5_4", PinType.Undefined); e.Add(pag5_4); //PAG5_4
    var ag1 = new AndGate("ag1"); e.Add(ag1); //AG1
    ag1.Children.Add(pag5_1); ag1.Children.Add(pag5_2);
    ag1.Children.Add(pag5_3); ag1.Children.Add(pag5_4);
    pag5_1.Parent = ag1; pag5_2.Parent = ag1;
    pag5_3.Parent = ag1; pag5_4.Parent = ag1;
    ag1.Parent = context;

    var p4_0 = new Pin("p4_0", PinType.Undefined); e.Add(p4_0); //P4_0
    p4_0.Parent = context;

    var l1 = new Line("l1"); e.Add(l1); l1.Start = p1_2; l1.End = p4_0; //L1
    l1.Parent = context;

    var l2 = new Line("l2"); e.Add(l2); l2.Start = p2_2; l2.End = pag5_3; //L2
    l2.Parent = context;

    var l3 = new Line("l3"); e.Add(l3); l3.Start = p4_0; l3.End = pag5_1; //L3
    l3.Parent = context;

    var l4 = new Line("l4"); e.Add(l4); l4.Start = pag5_4; l4.End = p3_1; //L4
    l4.Parent = context;

    #endregion

    //
    //INITIALIZE STATE
    //

    s1.State = s1_state;
    s2.State = s2_state;
    s3.State = null;
    ag1.State = null;

    //p1_1.State
    //p1_2.State = false;
    //p2_1.State
    //p2_2.State = false;
    //p3_1.State = false;
    //p3_2.State
    //p4_0.State = false;
    //pag5_1.State = false;
    //pag5_2.State
    //pag5_3.State = false;
    //pag5_4.State = false;

    //
    // UPDATE CONNECTIONS
    //

    Simulation.UpdatePins(e);

    var pins = e.Where(x => x is Pin).Cast<Pin>();

    System.Console.WriteLine("--- pin connections ---");
    System.Console.WriteLine("");

    foreach (var pin in pins)
    {
        PrintPin(pin);
    }

    //
    // RUN SIMULATION
    //

    var simulations = e.Where(x => x is ISimulation).Cast<ISimulation>();

    foreach (var sim in simulations)
    {
        System.Console.WriteLine("--- simulation: {0} | Type: {1} ---", (sim as Element).Name, sim.GetType());
        System.Console.WriteLine("");

        sim.Calculate();
    }

    //
    // SHOW RESULTS
    //

    System.Console.WriteLine("--- simulation result ---");
    System.Console.WriteLine("");

    var states = e.Where(x => x is IState);
    foreach (var state in states)
    {
        System.Console.WriteLine("Name: {0}, State: {1}",
            state.Name,
            (state as IState).State);
    }

    Serializer.SaveJson<Context>(context, test_name + ".json");

    //Console.ReadKey();
}

#endregion

#region Test2

static void Test2(bool s1_state, bool s2_state)
{
    string test_name = string.Format("--- test2 --- s1_state={0} --- s2_state={1} ---", s1_state, s2_state);

    System.Diagnostics.Debug.Print("");
    System.Diagnostics.Debug.Print(test_name);
    System.Diagnostics.Debug.Print("");

    System.Console.WriteLine("");
    System.Console.WriteLine(test_name);
    System.Console.WriteLine("");

    //
    //TEST1 MODEL
    //

    #region Test1 Model

    var context = new Context("context_test2");
    var e = context.Children;

    var p1_1 = new Pin("p1_1", PinType.Input); e.Add(p1_1); //P1_1
    var p1_2 = new Pin("p1_2", PinType.Output); e.Add(p1_2); //P1_2
    var s1 = new Signal("s1"); e.Add(s1); s1.Input = p1_1; s1.Output = p1_2; //S1
    s1.Parent = context; p1_1.Parent = s1; p1_2.Parent = s1;

    var p2_1 = new Pin("p2_1", PinType.Input); e.Add(p2_1); //P2_1
    var p2_2 = new Pin("p2_2", PinType.Output); e.Add(p2_2); //P2_2
    var s2 = new Signal("s2"); e.Add(s2); s2.Input = p2_1; s2.Output = p2_2; //S2
    s2.Parent = context; p2_1.Parent = s2; p2_2.Parent = s2;

    var p3_1 = new Pin("p3_1", PinType.Input); e.Add(p3_1); //P3_1
    var p3_2 = new Pin("p3_2", PinType.Output); e.Add(p3_2); //P3_2
    var s3 = new Signal("s3"); e.Add(s3); s3.Input = p3_1; s3.Output = p3_2; //S3
    s3.Parent = context; p3_1.Parent = s3; p3_2.Parent = s3;

    var pog5_1 = new Pin("pog5_1", PinType.Undefined); e.Add(pog5_1); //POG5_1
    var pog5_2 = new Pin("pog5_2", PinType.Undefined); e.Add(pog5_2); //POG5_2
    var pog5_3 = new Pin("pog5_3", PinType.Undefined); e.Add(pog5_3); //POG5_3
    var pog5_4 = new Pin("pog5_4", PinType.Undefined); e.Add(pog5_4); //POG5_4
    var og1 = new OrGate("og1"); e.Add(og1); //OG1
    og1.Children.Add(pog5_1); og1.Children.Add(pog5_2);
    og1.Children.Add(pog5_3); og1.Children.Add(pog5_4);
    pog5_1.Parent = og1; pog5_2.Parent = og1;
    pog5_3.Parent = og1; pog5_4.Parent = og1;
    og1.Parent = context;

    var p4_0 = new Pin("p4_0", PinType.Undefined); e.Add(p4_0); //P4_0
    p4_0.Parent = context;

    var l1 = new Line("l1"); e.Add(l1); l1.Start = p1_2; l1.End = p4_0; //L1
    l1.Parent = context;

    var l2 = new Line("l2"); e.Add(l2); l2.Start = p2_2; l2.End = pog5_3; //L2
    l2.Parent = context;

    var l3 = new Line("l3"); e.Add(l3); l3.Start = p4_0; l3.End = pog5_1; //L3
    l3.Parent = context;

    var l4 = new Line("l4"); e.Add(l4); l4.Start = pog5_4; l4.End = p3_1; //L4
    l4.Parent = context;

    #endregion

    //
    //INITIALIZE STATE
    //

    s1.State = s1_state;
    s2.State = s2_state;
    s3.State = null;
    og1.State = null;

    //p1_1.State
    //p1_2.State = false;
    //p2_1.State
    //p2_2.State = false;
    //p3_1.State = false;
    //p3_2.State
    //p4_0.State = false;
    //pag5_1.State = false;
    //pag5_2.State
    //pag5_3.State = false;
    //pag5_4.State = false;

    //
    // UPDATE CONNECTIONS
    //

    Simulation.UpdatePins(e);

    var pins = e.Where(x => x is Pin).Cast<Pin>();

    System.Console.WriteLine("--- pin connections ---");
    System.Console.WriteLine("");

    foreach (var pin in pins)
    {
        PrintPin(pin);
    }

    //
    // RUN SIMULATION
    //

    var simulations = e.Where(x => x is ISimulation).Cast<ISimulation>();

    foreach (var sim in simulations)
    {
        System.Console.WriteLine("--- simulation: {0} | Type: {1} ---", (sim as Element).Name, sim.GetType());
        System.Console.WriteLine("");

        sim.Calculate();
    }

    //
    // SHOW RESULTS
    //

    System.Console.WriteLine("--- simulation result ---");
    System.Console.WriteLine("");

    var states = e.Where(x => x is IState);
    foreach (var state in states)
    {
        System.Console.WriteLine("Name: {0}, State: {1}",
            state.Name,
            (state as IState).State);
    }

    Serializer.SaveJson<Context>(context, test_name + ".json");

    //Console.ReadKey();
}

#endregion
