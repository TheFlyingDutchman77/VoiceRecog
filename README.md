This repository is a Voice Recognition program to control your aircraft in MSFS (either 2020 or 2024). 

The voice commands are defined using a text file voice_commands.yml. A couple of examples are given in there:

<code>
VOICE COMMANDS
gear up: Mobiflight.GEAR_UP
gear down: Mobiflight.GEAR_DOWN
flaps up: Mobiflight.FLAPS_DECR
flaps down: Mobiflight.FLAPS_INCR
autopilot on: AUTOPILOT_ON
autopilot off: AUTOPILOT_OFF
</code>
<br>
The first part of the line contains the actual voice command, after the : follows the event to be send to the simulator. This can be Mobiflight events or standard simconnect events. FSUIPC events will follow in a later version.
