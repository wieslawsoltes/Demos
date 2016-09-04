# Logic

Process Function Plan Editor & Simulation Engine

## Editor

- Auto connect elements using Ctrl + Mouse Left Button Click on elements
- Create Signal elements with Ctrl + Mouse Left Button Click on canvas
- Split wires with Ctrl + Mouse Left Button Click on wires

## Simulation Engine

Engine is using synchronous sequential logic with clock signal
to simulate state of all elements every clock cycle.

Logic is first compiled to optimized data structures
and then can be run in step-by-step mode or in perdiodic clock cycles
mode simulation with defined time interval in milliseconds. Each cycle
all states are calculated. Simulation time is virtual and equal to
simulation cycle interval times current cycle number.

## Supported logic elements
- Clock (simple cycle based clock)
- Pin (wire connector, element pin without state)
- Wire (start/end can be inverted)
- Signal (simple input/output)
- AndGate (use Wire inverter for Nand gate)
- OrGate (use Wire inverter for Nor gate)
- TimerOn (on delay timer)
- TimerOff (off delay timer)
- TimerPulse (pulse timer)

## State

Gates, timers and signals have state that is 3-state boolean 
with true/false/null states avaibale.

## TimerOn logic
On Timer waits a Delay after an input goes high before turning the State on.  
The State goes low when the input goes low.

## TimerOff logic
Off Timer causes State to go high when an input goes high 
and keeps it high for a Delay after the input goes low.

## TimerPulse logic
Pulse Timer causes State to go high when an input goes high 
and keeps it high for a Delay and then goes low.

## References
http://en.wikipedia.org/wiki/Three-valued_logic
http://en.wikipedia.org/wiki/Many-valued_logic
http://en.wikipedia.org/wiki/Digital_circuit
http://en.wikipedia.org/wiki/Synchronous_logic
http://en.wikipedia.org/wiki/Sequential_logic
http://en.wikipedia.org/wiki/Logic_gate
http://en.wikipedia.org/wiki/Flip-flop_(electronics)
http://en.wikipedia.org/wiki/Clock_signal
http://en.wikipedia.org/wiki/State_(computer_science)
http://en.wikipedia.org/wiki/Logic_design
http://en.wikipedia.org/wiki/Logic_simulation

## License

Logic is licensed under the [MIT license](LICENSE.TXT).
